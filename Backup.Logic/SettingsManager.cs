using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace Backup.Logic
{
    public class SettingsManager : ISettingsManager
    {
        public static string SettingsName { get { return "tiz.digital.backup.settings"; } }

        public SettingsManager()
        {
        }

        private Settings Create()
        {
            return new Settings
            {
                FileSystemDirectory = Path.Combine(
                    ApplicationData.Current.LocalFolder.Path, 
                    FileManager.ArchiveFolder)
            };
        }

        public async Task<Settings> WriteSettingsAsync(Settings settings)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync(SettingsName, CreationCollisionOption.ReplaceExisting);

            //await FileIO.WriteBufferAsync(file, buffer);

            var formatter = new DataContractSerializer(typeof(Settings));
            var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
            using (var outputStream = stream.GetOutputStreamAt(0))
            {
                using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                {
                    dataWriter.WriteString(settings.FileSystemDirectory);
                    dataWriter.WriteBoolean(settings.IsFileSystemEnabled);
                    await dataWriter.StoreAsync();
                    await outputStream.FlushAsync();
                }
            }

            //formatter.WriteObject(outStream, settings);

            Debug.WriteLine("SavedSettings");
            return settings;
        }

        public void WriteSettings2(Settings settings)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            //StorageFile file = await localFolder.CreateFileAsync(SettingsName, CreationCollisionOption.ReplaceExisting);
            var filePath = Path.Combine(localFolder.Path, SettingsName);
            var bytes = ObjectToByteArray(settings);
            File.WriteAllBytes(filePath, bytes);

            //var buffer = bytes.AsBuffer();
            //await FileIO.WriteBufferAsync(file, buffer);
            //return settings;
        }

        public Settings ReadSettings2()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var filePath = Path.Combine(localFolder.Path, SettingsName);
            if (!File.Exists(filePath))
            {
                var defaultSettings = Create();
                WriteSettings2(defaultSettings);
                return defaultSettings;
            }

            var bytes = File.ReadAllBytes(filePath);
            var settings = (Settings)BytesToObject(bytes);
            return settings;
        }

        public async Task<Settings> SaveSettingsAsync2(Settings settings)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var formatter = new DataContractSerializer(typeof(Settings));
            StorageFile file = await localFolder.CreateFileAsync(SettingsName, CreationCollisionOption.ReplaceExisting);
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.WriteObject(stream, settings);
                await FileIO.WriteBytesAsync(file, stream.ToArray());
                await stream.FlushAsync();
            }
            return settings;
        }
        public async Task<Settings> GetSettingsAsync2()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            Debug.WriteLine(localFolder.Path);
            var settingsFilePath = Path.Combine(localFolder.Path, SettingsName);
            if (!File.Exists(settingsFilePath))
                return new Settings();

            var formatter = new DataContractSerializer(typeof(Settings));
            StorageFile file = await localFolder.GetFileAsync(SettingsName);
            using (var stream = new FileStream(settingsFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                var settings = (Settings)formatter.ReadObject(stream);
                stream.Flush();
                return settings;
            }
        }

        public async Task<Settings> GetSettingsAsync()
        {
            var formatter = new DataContractSerializer(typeof(Settings));
            var filePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, SettingsName);
            Settings settings = new Settings();
            if (!File.Exists(filePath))
            {
                var defaultSettings = Create();
                return await WriteSettingsAsync(defaultSettings);
            }

            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(SettingsName);
            var stream = await file.OpenAsync(FileAccessMode.Read);
            ulong size = stream.Size;
            using (var inputStream = stream.GetInputStreamAt(0))
            {
                using (var dataReader = new Windows.Storage.Streams.DataReader(inputStream))
                {
                    uint numBytesLoaded = await dataReader.LoadAsync((uint)size);
                    settings.FileSystemDirectory = dataReader.ReadString(numBytesLoaded); // BUG
                    settings.IsFileSystemEnabled = dataReader.ReadBoolean();
                }
            }

            //var lines = await FileIO.ReadTextAsync(file);
            //settings.FileSystemDirectory = lines;
            //var stream = new FileStream(settingsFileName, FileMode.Open, FileAccess.Read, FileShare.None);
            //settings = (Settings)formatter.ReadObject(stream);


            Debug.WriteLine("Read Settings");
            return settings;
        }

        byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        object BytesToObject(byte[] bytes)
        {
            if (bytes == null)
                return null;

            MemoryStream memStream = new MemoryStream();
            var binForm = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            memStream.Write(bytes, 0, bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = binForm.Deserialize(memStream);
            return obj;
        }

    }
}
