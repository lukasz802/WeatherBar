using WeatherBar.Model.Enums;

namespace WeatherBar.Model.Interfaces
{
    public interface IUnits
    {
        #region Properties

        Units Units { get; }

        #endregion

        #region Methods

        void ChangeUnits(Units units);

        #endregion
    }
}
