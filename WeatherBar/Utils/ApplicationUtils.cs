using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WeatherBar.Model.Enums;

namespace WeatherBar.Utils
{
    public static class ApplicationUtils
    {
        #region Public methods

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(unixTimeStamp);
        }

        public static string ConvertCoordinatesFromDecToDeg(double decValue, bool isLongitude)
        {
            string direction = decValue > 0 ? isLongitude ? "E" : "N" : isLongitude ? "W" : "S";
            string[] temp = Math.Round(decValue > 0 ? decValue : -decValue, 2).ToString().Split('.', ',');
            string minutesValue = Math.Round(double.Parse(temp.Last()) * 60 / 100).ToString();

            return string.Concat(temp.First(), "° ", minutesValue.Length != 1 ? minutesValue : $"0{minutesValue}", $"' {direction}");
        }

        public static T FindVisualParent<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(depObj);

                if (parent == null)
                {
                    return null;
                }
                else if (parent is T)
                {
                    return (T)parent;
                }
                else
                {
                    return FindVisualParent<T>(parent);
                }
            }
            else
            {
                return null;
            }
        }

        public static string GetDescriptionFromId(string descriptionId)
        {
            switch (descriptionId)
            {
                case "200":
                    return "Thunderstorm with light rain";
                case "201":
                    return "Thunderstorm with rain";
                case "202":
                    return "Thunderstorm with heavy rain";
                case "210":
                    return "Light thunderstorm";
                case "211":
                    return "Thunderstorm";
                case "212":
                    return "Heavy thunderstorm";
                case "221":
                    return "Ragged thunderstorm";
                case "230":
                    return "Thunderstorm with light drizzle";
                case "231":
                    return "Thunderstorm with drizzle";
                case "232":
                    return "Thunderstorm with heavy drizzle";
                case "300":
                    return "Light intensity drizzle";
                case "301":
                    return "Drizzle";
                case "302":
                    return "Heavy intensity drizzle";
                case "310":
                    return "Light intensity drizzle rain";
                case "311":
                    return "Drizzle rain";
                case "312":
                    return "Heavy intensity drizzle rain";
                case "313":
                    return "Shower rain and drizzle";
                case "314":
                    return "Heavy shower rain and drizzle";
                case "321":
                    return "Shower drizzle";
                case "500":
                    return "Light rain";
                case "501":
                    return "Moderate rain";
                case "502":
                    return "Heavy intensity rain";
                case "503":
                    return "Very heavy rain";
                case "504":
                    return "Extreme rain";
                case "511":
                    return "Freezing rain";
                case "520":
                    return "Light intensity shower rain";
                case "521":
                    return "Shower rain";
                case "522":
                    return "Heavy intensity shower rain";
                case "531":
                    return "Ragged shower rain";
                case "600":
                    return "Light snow";
                case "601":
                    return "Snow";
                case "602":
                    return "Heavy snow";
                case "611":
                    return "Sleet";
                case "612":
                    return "Light shower sleet";
                case "613":
                    return "Shower sleet";
                case "615":
                    return "Light rain and snow";
                case "616":
                    return "Rain and snow";
                case "620":
                    return "Light shower snow";
                case "621":
                    return "Shower snow";
                case "622":
                    return "Heavy shower snow";
                case "701":
                    return "Mist";
                case "711":
                    return "Smoke";
                case "721":
                    return "Haze";
                case "731":
                    return "Dust whirls";
                case "741":
                    return "Fog";
                case "751":
                    return "Sand";
                case "761":
                    return "Dust";
                case "762":
                    return "Volcanic ash";
                case "771":
                    return "Squalls";
                case "781":
                    return "Tornado";
                case "800":
                    return "Clear sky";
                case "801":
                    return "Few clouds";
                case "802":
                    return "Scattered clouds";
                case "803":
                    return "Broken clouds";
                case "804":
                    return "Overcast clouds";
                default:
                    throw new ArgumentException($"Invalid input data. There is no appropriate content for Id: {descriptionId}");
            }
        }

        public static void SwapUnits(Units units)
        {
            switch (units)
            {
                case Units.Metric:
                    Application.Current.Resources["WindSpeed"] = "km/h";
                    Application.Current.Resources["Degress"] = "°";
                    Application.Current.Resources["TempUnit"] = "°C";
                    break;
                case Units.Imperial:
                    Application.Current.Resources["WindSpeed"] = "mph";
                    Application.Current.Resources["Degress"] = "°";
                    Application.Current.Resources["TempUnit"] = "°F";
                    break;
                case Units.Standard:
                    Application.Current.Resources["WindSpeed"] = "km/h";
                    Application.Current.Resources["Degress"] = " K";
                    Application.Current.Resources["TempUnit"] = " K";
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
                Application.Current.Resources["NotFound"] = "Unfortunately, the city with the given name could not be found";
                Application.Current.Resources["BackDescription"] = "Go back to the previous panel";
                Application.Current.Resources["Back"] = "Back";
                Application.Current.Resources["TryAgain"] = "Try again";
                Application.Current.Resources["ConnectionFailed"] = "An error occurred while trying to connect to the Openweathermap.org server";
                Application.Current.Resources["StandardUnits"] = "Standard";
                Application.Current.Resources["ImperialUnits"] = "Imperial";
                Application.Current.Resources["MetricUnits"] = "Metric";
                Application.Current.Resources["UnitsComboBox"] = "Select units";
                Application.Current.Resources["Minutes15RefreshTime"] = "15 minutes";
                Application.Current.Resources["Minutes30RefreshTime"] = "30 minutes";
                Application.Current.Resources["Minutes45RefreshTime"] = "45 minutes";
                Application.Current.Resources["Hour1RefreshTime"] = "1 hour";
                Application.Current.Resources["RefreshTimeComboBox"] = "Select refresh time";
                Application.Current.Resources["StartingLocationTextBox"] = "Select starting location";
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
                Application.Current.Resources["NotFound"] = "Niestety, nie udało się odnaleźć miasta o podanej nazwie";
                Application.Current.Resources["BackDescription"] = "Wróć do poprzedniego panelu";
                Application.Current.Resources["Back"] = "Wróć";
                Application.Current.Resources["TryAgain"] = "Spróbuj ponownie";
                Application.Current.Resources["ConnectionFailed"] = "Wystąpił błąd przy próbie połączenia z serwerem Openweathermap.org";
                Application.Current.Resources["StandardUnits"] = "Standardowe";
                Application.Current.Resources["ImperialUnits"] = "Imperialne";
                Application.Current.Resources["MetricUnits"] = "Metryczne";
                Application.Current.Resources["UnitsComboBox"] = "Wybierz jednostki";
                Application.Current.Resources["Minutes15RefreshTime"] = "15 minut";
                Application.Current.Resources["Minutes30RefreshTime"] = "30 minut";
                Application.Current.Resources["Minutes45RefreshTime"] = "45 minut";
                Application.Current.Resources["Hour1RefreshTime"] = "1 godzina";
                Application.Current.Resources["RefreshTimeComboBox"] = "Wybierz czas odświeżana";
                Application.Current.Resources["StartingLocationTextBox"] = "Wybierz lokalizacje startową";
            }
        }

        #endregion
    }
}
