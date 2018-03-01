using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Backup.Logic
{
    /// <summary>
    /// Create, save and load Source items (directory info)
    /// </summary>
    public class SourcesManager
    {
        // The list of target directories
        public static string SourcesFileName { get { return "sources.dat"; } }

        /// <summary>
        /// Create the sources file
        /// </summary>
        static private void CreateSourcesFile(IEnumerable<Source> sources = null)
        {
            string tempDir = ApplicationData.Current.LocalFolder.Path;
            Debug.WriteLine(tempDir);

            var sourcesPath = Path.Combine(tempDir, SourcesFileName);
            if (File.Exists(sourcesPath)) return;

            using (var writer = File.CreateText(sourcesPath))
            {
                writer.WriteLine("Directory, Pattern, ModifiedOnly, LastBackup");

                if (sources != null)
                {
                    foreach (var s in sources)
                    {
                        writer.WriteLine($"{s.Directory}, {s.Pattern}, {s.ModifiedOnly}, {s.LastBackup.ToString()}");
                    }
                }
            }
        }

        /// <summary>
        /// Get the Sources from file
        /// </summary>
        static public IEnumerable<Source> GetSources()
        {
            CreateSourcesFile();
            var lines = File.ReadAllLines(
                Path.Combine(ApplicationData.Current.LocalFolder.Path, SourcesFileName));
            
            return new List<Source>(
                (from l in lines.Skip(1)
                 select new Source(
                    path: l.Split(',')[0].Trim(),
                    pattern: l.Split(',')[1].Trim(),
                    modifiedOnly: l.Split(',')[2].Trim(),
                    lastBackup: DateTime.Parse(l.Split(',')[3].Trim()))
                 ));
        }

        /// <summary>
        /// Save the Sources to a file
        /// </summary>
        static public void SaveSources(IEnumerable<Source> sources)
        {
            File.Delete(Path.Combine(ApplicationData.Current.LocalFolder.Path, SourcesFileName));
            CreateSourcesFile(sources);
        }
    }
}
