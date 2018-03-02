using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Backup.Logic
{
    [Serializable]
    public class Settings
    {
        public string FileSystemDirectory { get; set; }
        public bool IsFileSystemEnabled { get; set; }
        public string AWSS3Bucket { get; set; }
        public bool IsS3BucketEnabled { get; set; }

        public bool IsGlacierEnabled { get; set; }
        public string AWSGlacierVault { get; set; }

        public string AWSAccessKeyID { get; set; }
        public string AWSSecretAccessKey { get; set; }

        public AWSRegionEndPoint AWSS3Region { get; set; }
        public AWSRegionEndPoint AWSGlacierRegion { get; set; }
        public DateTime InventoryUpdateRequested { get; set; }

        public bool IsBackupOnLogoff { get; set; }
        public bool IsLaunchOnLogon { get; set; }
        public bool CreateBackupOnStart { get; set; }

    }
}
