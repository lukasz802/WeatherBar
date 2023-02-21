using WeatherBar.Model;

namespace WeatherBar.Application.Services.Interfaces
{
    public interface IConfigurationService
    {
        AppSettings GetAppSettings();

        void UpdateAndSaveConfiguration(AppSettings appSettings);
    }
}
