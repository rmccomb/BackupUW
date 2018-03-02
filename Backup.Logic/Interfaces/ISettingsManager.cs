using System.Threading.Tasks;

namespace Backup.Logic
{
    public interface ISettingsManager
    {
        Task<Settings> GetSettingsAsync();
        Task<Settings> WriteSettingsAsync(Settings settings);
        void WriteSettings2(Settings settings);
        Task<Settings> SaveSettingsAsync2(Settings settings);
        Task<Settings> GetSettingsAsync2();
        Settings ReadSettings2();

    }
}