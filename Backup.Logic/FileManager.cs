using System;
using System.Collections.Generic;
using System.Diagnostics;
using IO = System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Backup.Logic
{
    public class FileManager : IFileManager
    {
        public static string ArchiveFolder => "Archive";
        public static string DiscoveredFilesName => "disc.dat";

        private ISettingsManager settingsManager;

        public FileManager(ISettingsManager settingsManager)
        {
            this.settingsManager = settingsManager;
        }

        /// <summary>
        /// Write the paths of files to backup to the disco file
        /// </summary>
        /// <param name="tempDir">Path to disco file</param>
        /// <param name="sources">List of source directories and search patterns</param>
        /// <param name="fromDate">File must have changed since this date to be included</param>
        public IEnumerable<FileDetail> DiscoverFiles(
            IEnumerable<Source> sources,
            string tempDir = null)
        {
            //var settings = settingsManager.GetSettings();
            var files = new List<FileDetail>();

            foreach(var s in sources)
            {
                files.AddRange(GetFiles(s.Directory, s.Pattern, s.IsModifiedOnly, s.LastBackup));
            }

            return files;
        }

        /// <summary>
        /// Scan for files to backup
        /// </summary>
        /// <param name="directory">Root directory</param>
        /// <param name="pattern">File extension e.g. *.*</param>
        /// <param name="isModifiedOnly">Get only modified files</param>
        /// <param name="fromDate">The date of the last backup</param>
        /// <returns>List of files</returns>
        static private IEnumerable<FileDetail> GetFiles(
            string directory,
            string pattern,
            bool isModifiedOnly,
            DateTime fromDate)
        {
            var changed = new List<FileDetail>();
            var files = new List<string>();
            GetFiles(directory, pattern, files);

            if (isModifiedOnly)
            {
                foreach (var file in files)
                {
                    var lastWrite = IO.File.GetLastWriteTime(file);
                    if (fromDate < lastWrite)
                        changed.Add(new FileDetail(file, directory));
                }
            }
            else
            {
                changed.AddRange(files.Select(f => new FileDetail(f, directory)));
            }

            return changed;
        }

        static void GetFiles(string path, string pattern, List<string> list)
        {
            try
            {
                IO.DirectoryInfo info = new IO.DirectoryInfo(path);
                var isDir = info.Attributes.HasFlag(IO.FileAttributes.Directory);
                var isHidden = info.Attributes.HasFlag(IO.FileAttributes.Hidden);
                var isSys = info.Attributes.HasFlag(IO.FileAttributes.System);
                var isReparse = info.Attributes.HasFlag(IO.FileAttributes.ReparsePoint);
                if (isSys || isHidden /*|| isReparse*/) return;

                list.AddRange(IO.Directory.GetFiles(path, pattern));

                var dirs = IO.Directory.GetDirectories(path);
                foreach (var d in dirs)
                {
                    GetFiles(d, pattern, list);
                }
            }
            catch (Exception ex)
            {
                IO.DirectoryInfo info = new IO.DirectoryInfo(path);
                var att1 = info.Attributes.HasFlag(FileAttributes.Directory);

                Debug.WriteLine(ex.Message);
                //BackupError?.Invoke(ex.Message);
            }
        }

        /// <summary>
        /// Invoke the whole backup process - get sources, discover files, copy to destination
        /// </summary>
        public void InvokeBackup()
        {
            try
            {
                //BackupStarted?.Invoke();

                //while (!cts.IsCancellationRequested)
                //{
                //    Thread.Sleep(1000);
                //    Debug.WriteLine("WORKING");
                //}
                //Debug.WriteLine("COMPLETE");
                //isWorking = false;
                //return;

                var sources = SourcesManager.GetSources();
                var files = DiscoverFiles(sources);
                SaveDiscoveredFiles(files);
                CopyFilesAsync();
                UpdateTimestamp(sources);

                //BackupCompleted?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //BackupError?.Invoke(ex.Message);
            }
        }

        public static void SaveDiscoveredFiles(IEnumerable<FileDetail> fileDetails)
        {
            var discFile = IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, DiscoveredFilesName);

            if (IO.File.Exists(discFile))
                IO.File.Delete(discFile);

            IO.File.AppendAllLines(discFile, fileDetails.Select(f => $"{f.FilePath}, {f.SubPath}"));
        }

        /// <summary>
        /// Do the backing up of the discovered files
        /// </summary>
        /// <param name="archiveDir">A directory to backup to</param>
        public async void CopyFilesAsync(string archiveDir = null)
        {
            var settings = await settingsManager.GetSettingsAsync();
            var tempDir = ApplicationData.Current.LocalFolder.Path;

            var filesName = IO.Path.Combine(tempDir, DiscoveredFilesName);
            if (!IO.File.Exists(filesName))
            {
                //BackupWarning?.Invoke("No new or modified files were discovered");
                return; // Nothing to do
            }
            var files = IO.File.ReadAllLines(filesName);
            if (files.Length == 0)
            {
                //BackupWarning?.Invoke("No new or modified files were discovered");
                return; // Nothing to do
            }

            if (archiveDir == null)
                archiveDir = settings.FileSystemDirectory;

            // Has user set an archive folder?
            if (String.IsNullOrEmpty(archiveDir))
                archiveDir = IO.Path.Combine(tempDir, ArchiveFolder);

            // Make the archive
            var fileSource = DoCopy(archiveDir, files);

            // Upload the zip archive
            //if (settings.IsS3BucketEnabled)
            //{
            //    await UploadS3ArchiveAsync(settings, fileSource.ZipFileArchive);

            //    BackupSuccess?.Invoke($"Backup uploaded to S3 Bucket {settings.AWSS3Bucket}");

            //    // Clean up if the user didn't want the File System option
            //    if (!settings.IsFileSystemEnabled)
            //    {
            //        File.Delete(fileSource.ZipFileArchive);
            //    }
            //}

            //if (settings.IsGlacierEnabled)
            //{
            //    UploadGlacierArchiveAsync(settings, fileSource.ZipFileArchive);

            //    BackupSuccess?.Invoke($"Backup uploaded to Glacier Vault {settings.AWSGlacierVault}");

            //    // Clean up if the user didn't want the File System option
            //    if (!settings.IsFileSystemEnabled)
            //    {
            //        File.Delete(fileSource.ZipFileArchive);
            //    }
            //}

            //try
            //{
            //    // Clean up source directory created for zip archive
            //    DeleteZipSource(fileSource.UniqueArchiveDir);
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex.Message);
            //    BackupWarning?.Invoke($"An error occurred removing the temporary archive files: {fileSource.UniqueArchiveDir}");
            //}
        }

        /// <summary>
        /// Copy files to the given directory and return the zip file name
        /// </summary>
        private static FileSource DoCopy(string archiveDir, string[] files)
        {
            var fileSource = new FileSource { UniqueArchiveDir = GetArchiveUniqueName(archiveDir) };

            foreach (var line in files)
            {
                try
                {
                    // Copy file and append to processing file
                    var filepath = (from f in line.Split(',') select f.Trim()).First();
                    var subpath = (from f in line.Split(',') select f.Trim()).Last();

                    // Create the subpath in archive
                    var splits = subpath.Split('\\');
                    var subpathDir = "";
                    for (int i = 0; i < splits.Length - 1; i++)
                        subpathDir += "\\" + splits[i];

                    var newDir = IO.Path.Combine(fileSource.UniqueArchiveDir, subpathDir.TrimStart('\\'));
                    var di = IO.Directory.CreateDirectory(newDir);
                    //Debug.Write(di);

                    try
                    {
                        var dest = IO.Path.Combine(fileSource.UniqueArchiveDir, subpath.TrimStart('\\'));
                        IO.File.Copy(filepath, dest, true);
                        IO.File.SetAttributes(dest, IO.FileAttributes.Normal);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Debug.WriteLine("UnauthorizedAccessException: " + ex.Message);
                        //BackupWarning?.Invoke($"Unauthorized access error for {filepath}. File will be skipped.");
                    }
                    catch (IO.IOException io)
                    {
                        Debug.WriteLine("IOException: " + io.Message);
                        //BackupWarning?.Invoke($"IO Error for {filepath}. File will be skipped.");
                    }

                    fileSource.FileCount++;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw ex;
                }
            }

            if (fileSource.FileCount > 0)
            {
                try
                {
                    IO.Compression.ZipFile.CreateFromDirectory(
                        fileSource.UniqueArchiveDir, 
                        fileSource.ZipFileArchive, 
                        IO.Compression.CompressionLevel.Optimal, 
                        false);

                    // Clean up folders used to make the zip archive
                    //DeleteZipSource(fileSource.UniqueArchiveDir);

                    //BackupSuccess?.Invoke($"{fileSource.FileCount} file {(fileSource.FileCount == 1 ? String.Empty : "s")} copied to {fileSource.ZipFileArchive}");
                    Debug.WriteLine($"{fileSource.FileCount} file {(fileSource.FileCount == 1 ? String.Empty : "s")} copied to {fileSource.ZipFileArchive}");
                    return fileSource;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw ex;
                }
            }

            return null;
        }

        static internal string GetArchiveUniqueName(string archiveDir = null)
        {
            return IO.Path.Combine(archiveDir, $"archive_{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}");
        }

        /// <summary>
        /// Update the timestamp on the sources file
        /// </summary>
        public static void UpdateTimestamp(IEnumerable<Source> sources)
        {
            foreach(var s in sources)
                s.LastBackup = DateTime.Now;

            SourcesManager.SaveSources(sources);
        }
    }
}
