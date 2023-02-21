using WeatherBar.Model.Enums;

namespace WeatherBar.Model.Interfaces
{
    public interface IMultiLanguage
    {
        #region Properties

        Language Language { get; }

        #endregion

        #region Methods

        void ChangeLanguage(Language language);

        #endregion
    }
}
