using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backup.Logic
{
    public class Source
    {
        public static string NeverText => "Never";
        public string Directory { get; set; }
        public string Pattern { get; set; }
        public string ModifiedOnly { get; set; }
        public bool IsModifiedOnly => ModifiedOnly == "Yes";
        public DateTime LastBackup { get; set; }
        public string LastBackupText => LastBackup == DateTime.MinValue ? NeverText : LastBackup.ToString();
        public Source(string path, string pattern, string modifiedOnly, DateTime? lastBackup = null)
        {
            Directory = path;
            Pattern = pattern;
            ModifiedOnly = modifiedOnly;
            LastBackup = lastBackup == null ? DateTime.MinValue : lastBackup.Value;
        }
        public Source(string path, string pattern, string modifiedOnly, string lastBackupText)
        {
            Directory = path;
            Pattern = pattern;
            ModifiedOnly = modifiedOnly;
            LastBackup = lastBackupText == NeverText ? DateTime.MinValue : DateTime.Parse(lastBackupText);
        }
    }

}
