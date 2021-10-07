using System;
using System.Windows;
using WebApi.Model.Enums;
using WebApi.Model.Interfaces;

namespace WeatherBar.Utils
{
    public static class ApplicationUtils
    {
        #region Public methods

        public static void SetUnits(Units toUnits, Units fromUnits, IHourlyData currentWeatherData, IFourDaysData weatherForecastData)
        {
            switch (toUnits)
            {
                case Units.Metric:
                    Application.Current.Resources["WindSpeed"] = "km/h";
                    Application.Current.Resources["Degress"] = "°";
                    Application.Current.Resources["TempUnit"] = "°C";

                    if (fromUnits == Units.Imperial)
                    {
                        CalculateFromMetricToImperial(currentWeatherData, weatherForecastData);
                    }
                    else if (fromUnits == Units.Standard)
                    {
                        CalculateFromMetricToStandard(currentWeatherData, weatherForecastData);
                    }
                    break;
                case Units.Imperial:
                    Application.Current.Resources["WindSpeed"] = "mph";
                    Application.Current.Resources["Degress"] = "°";
                    Application.Current.Resources["TempUnit"] = "°F";

                    if (fromUnits == Units.Metric)
                    {
                        CalculateFromImperialToMetric(currentWeatherData, weatherForecastData);
                    }
                    else if (fromUnits == Units.Standard)
                    {
                        CalculateFromImperialToMetric(currentWeatherData, weatherForecastData);
                        CalculateFromMetricToStandard(currentWeatherData, weatherForecastData);
                    }
                    break;
                case Units.Standard:
                    Application.Current.Resources["WindSpeed"] = "km/h";
                    Application.Current.Resources["Degress"] = "K";
                    Application.Current.Resources["TempUnit"] = "K";

                    if (fromUnits == Units.Metric)
                    {
                        CalculateFromStandardToMetric(currentWeatherData, weatherForecastData);
                    }
                    else if (fromUnits == Units.Imperial)
                    {
                        CalculateFromStandardToMetric(currentWeatherData, weatherForecastData);
                        CalculateFromMetricToStandard(currentWeatherData, weatherForecastData);
                    }
                    break;
            }
        }

        public static void TranslateResources(Language language)
        {
            if (language == Language.English)
            {
                Application.Current.Resources["Close"] = "Close";
                Application.Current.Resources["Minimalize"] = "Minimalize";
                Application.Current.Resources["SearchClick"] = "Search";
                Application.Current.Resources["Refresh"] = "Refresh";
                Application.Current.Resources["RemovePhrase"] = "Remove the phrase";
                Application.Current.Resources["Options"] = "Options";
                Application.Current.Resources["ForecastType"] = "Forecast type";
                Application.Current.Resources["Map"] = "Show on the map";
                Application.Current.Resources["Pressure"] = "Atmospheric pressure";
                Application.Current.Resources["Humidity"] = "Humidity";
                Application.Current.Resources["Wind"] = "Wind speed and direction";
                Application.Current.Resources["Return"] = "Return to the main panel";
                Application.Current.Resources["Update"] = "Updated at:";
                Application.Current.Resources["Sunrise"] = "Sunrise";
                Application.Current.Resources["Sunset"] = "Sunset";
                Application.Current.Resources["Latitiude"] = "Latitiude";
                Application.Current.Resources["Longtitiude"] = "Longtitiude";
                Application.Current.Resources["Rain"] = "The amount of rainfall in the last 24 hours";
                Application.Current.Resources["Snow"] = "The amount of snowfall in the last 24 hours";
                Application.Current.Resources["FeelTemp"] = "Feel";
                Application.Current.Resources["EmptySearchText"] = "Search";
                Application.Current.Resources["Next"] = "Next";
                Application.Current.Resources["Previous"] = "Prevoius";
                Application.Current.Resources["RefreshTime"] = "Refresh time";
                Application.Current.Resources["Language"] = "Language";
                Application.Current.Resources["Units"] = "Units";
                Application.Current.Resources["Location"] = "Starting location";
                Application.Current.Resources["LanguageDescription"] = "Default application language";
                Application.Current.Resources["LanguageComboBox"] = "Select language";
                Application.Current.Resources["UnitsDescription"] = "Units of measurement of physical quantities";
                Application.Current.Resources["RefreshTimeDescription"] = "Frequency of refreshing the weather data";
                Application.Current.Resources["LocationDescription"] = "Application startup location settings";
                Application.Current.Resources["PolishLanguage"] = "Polish";
                Application.Current.Resources["EnglishLanguage"] = "Language";
                Application.Current.Resources["Daily"] = "Daily";
                Application.Current.Resources["Hourly"] = "Hourly";
                Application.Current.Resources["FeelTempDescription"] = "Feel temperature: ";
            }
            else
            {
                Application.Current.Resources["Close"] = "Zamknij";
                Application.Current.Resources["Minimalize"] = "Minimalizuj";
                Application.Current.Resources["SearchClick"] = "Szukaj";
                Application.Current.Resources["Refresh"] = "Odśwież";
                Application.Current.Resources["RemovePhrase"] = "Usuń frazę";
                Application.Current.Resources["Options"] = "Opcje";
                Application.Current.Resources["ForecastType"] = "Typ prognozy";
                Application.Current.Resources["Map"] = "Pokaż na mapie";
                Application.Current.Resources["Pressure"] = "Ciśnienie atmosferyczne";
                Application.Current.Resources["Humidity"] = "Wilgotność";
                Application.Current.Resources["Wind"] = "Prędkość i kierunek wiatru";
                Application.Current.Resources["Return"] = "Wróc do panel głownego";
                Application.Current.Resources["Update"] = "Zaktualizowano o:";
                Application.Current.Resources["Sunrise"] = "Wschód słońca";
                Application.Current.Resources["Sunset"] = "Zachód słońca";
                Application.Current.Resources["Latitiude"] = "Szerokość geograficzna";
                Application.Current.Resources["Longtitiude"] = "Długość geograficzna";
                Application.Current.Resources["Rain"] = "Wielkość opadów deszczu w ciągu ostatniej doby";
                Application.Current.Resources["Snow"] = "Wielkość opadów śniegu w ciągu ostatniej doby";
                Application.Current.Resources["FeelTemp"] = "Odczuwalna";
                Application.Current.Resources["EmptySearchText"] = "Wyszukaj";
                Application.Current.Resources["Next"] = "Następny";
                Application.Current.Resources["Previous"] = "Poprzedni";
                Application.Current.Resources["RefreshTime"] = "Czas odświeżania";
                Application.Current.Resources["Language"] = "Język";
                Application.Current.Resources["Units"] = "Jednostki";
                Application.Current.Resources["Location"] = "Lokalizacja startowa";
                Application.Current.Resources["LanguageDescription"] = "Domyślny język aplikacji";
                Application.Current.Resources["LanguageComboBox"] = "Wybierz język";
                Application.Current.Resources["UnitsDescription"] = "Jednostki miary wielkości fizycznych";
                Application.Current.Resources["LocationDescription"] = "Ustawienia lokalizacji startowej aplikacji";
                Application.Current.Resources["RefreshTimeDescription"] = "Częstotliwość odświeżania danych pogodowych";
                Application.Current.Resources["PolishLanguage"] = "Polski";
                Application.Current.Resources["EnglishLanguage"] = "Angielski";
                Application.Current.Resources["Daily"] = "Dzienna";
                Application.Current.Resources["Hourly"] = "Godzinowa";
                Application.Current.Resources["FeelTempDescription"] = "Temperatura odczuwalna: ";
            }
        }

        #endregion

        #region Private methods

        private static void CalculateFromMetricToImperial(IHourlyData currentWeatherData, IFourDaysData weatherForecastData)
        {
            //currentWeatherData.WindSpeed = Convert.ToInt32(Math.Round(0.621371192 * currentWeatherData.WindSpeed, MidpointRounding.AwayFromZero));
            //currentWeatherData.AvgTemp = Convert.ToInt32(Math.Round((currentWeatherData.AvgTemp * 9 / 5) + 32D, MidpointRounding.AwayFromZero));
            //currentWeatherData.FeelTemp = Convert.ToInt32(Math.Round((currentWeatherData.FeelTemp * 9 / 5) + 32D, MidpointRounding.AwayFromZero));

            //foreach (var data in weatherForecastData.HourlyData)
            //{
            //    data.AvgTemp = Convert.ToInt32(Math.Round((data.AvgTemp * 9 / 5) + 32D, MidpointRounding.AwayFromZero));
            //    data.FeelTemp = Convert.ToInt32(Math.Round((data.FeelTemp * 9 / 5) + 32D, MidpointRounding.AwayFromZero));
            //    data.WindSpeed = Convert.ToInt32(Math.Round(0.621371192 * data.WindSpeed, MidpointRounding.AwayFromZero));
            //}

            //foreach (var data in weatherForecastData.DailyData)
            //{
            //    data.MaxTemp = Convert.ToInt32(Math.Round((data.MaxTemp * 9 / 5) + 32D, MidpointRounding.AwayFromZero));
            //    data.MinTemp = Convert.ToInt32(Math.Round((data.MinTemp * 9 / 5) + 32D, MidpointRounding.AwayFromZero));
            //}
        }

        private static void CalculateFromImperialToMetric(IHourlyData currentWeatherData, IFourDaysData weatherForecastData)
        {
            //currentWeatherData.WindSpeed = Convert.ToInt32(Math.Round(1.609344 * currentWeatherData.WindSpeed, MidpointRounding.AwayFromZero));
            //currentWeatherData.AvgTemp = Convert.ToInt32(Math.Round((currentWeatherData.AvgTemp - 32) * 5 / 9D, MidpointRounding.AwayFromZero));
            //currentWeatherData.FeelTemp = Convert.ToInt32(Math.Round((currentWeatherData.FeelTemp - 32) * 5 / 9D, MidpointRounding.AwayFromZero));

            //foreach (var data in weatherForecastData.HourlyData)
            //{
            //    data.AvgTemp = Convert.ToInt32(Math.Round((data.AvgTemp - 32) * 5 / 9D, MidpointRounding.AwayFromZero));
            //    data.FeelTemp = Convert.ToInt32(Math.Round((data.FeelTemp - 32) * 5 / 9D, MidpointRounding.AwayFromZero));
            //    data.WindSpeed = Convert.ToInt32(Math.Round(1.609344 * data.WindSpeed, MidpointRounding.AwayFromZero));
            //}

            //foreach (var data in weatherForecastData.DailyData)
            //{
            //    data.MaxTemp = Convert.ToInt32(Math.Round((data.MaxTemp - 32) * 5 / 9D, MidpointRounding.AwayFromZero));
            //    data.MinTemp = Convert.ToInt32(Math.Round((data.MinTemp - 32) * 5 / 9D, MidpointRounding.AwayFromZero));
            //}
        }

        private static void CalculateFromMetricToStandard(IHourlyData currentWeatherData, IFourDaysData weatherForecastData)
        {
            //currentWeatherData.AvgTemp += 273;
            //currentWeatherData.FeelTemp += 273;

            //foreach (var data in weatherForecastData.HourlyData)
            //{
            //    data.AvgTemp += 273;
            //    data.FeelTemp += 273;
            //}

            //foreach (var data in weatherForecastData.DailyData)
            //{
            //    data.MaxTemp += 273;
            //    data.MinTemp += 273;
            //}
        }

        private static void CalculateFromStandardToMetric(IHourlyData currentWeatherData, IFourDaysData weatherForecastData)
        {
            //currentWeatherData.AvgTemp -= 273;
            //currentWeatherData.FeelTemp -= 273;

            //foreach (var data in weatherForecastData.HourlyData)
            //{
            //    data.AvgTemp -= 273;
            //    data.FeelTemp -= 273;
            //}

            //foreach (var data in weatherForecastData.DailyData)
            //{
            //    data.MaxTemp -= 273;
            //    data.MinTemp -= 273;
            //}
        }

        #endregion
    }
}
