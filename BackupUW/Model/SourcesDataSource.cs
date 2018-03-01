using Backup.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupUW.Model
{
    public class SourcesDataSource : ObservableCollection<Source>
    {
        private static readonly object _lock = new object();

        static SourcesDataSource()
        {
            // Initialise
            //this.Add(new SourceViewModel("C:\\My Documents", "*.*", "Yes"));
            //this.Add(new SourceViewModel("C:\\Temp", "*.cs", "Yes"));
            //this.Add(new SourceViewModel("C:\\XX\\Y", "*.txt", "No"));

            Instance = new SourcesDataSource();
        }

        public static SourcesDataSource Instance;

        private SourcesDataSource() { }

        public void GetItems()
        {
            // Call to File Manager to get saved sources
            var sources = SourcesManager.GetSources();
            lock (_lock)
            {
                this.ClearItems();
                foreach (var source in sources)
                {
                    this.Add(source);
                }
            }
        }

        public void SaveItems()
        {
            SourcesManager.SaveSources(this);
        }

    }


}
