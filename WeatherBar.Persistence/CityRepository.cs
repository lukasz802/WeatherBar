using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WeatherBar.Model;
using WeatherBar.Persistence.Interfaces;

namespace WeatherBar.Persistence
{
    public class CityRepository : ICityRepository, IDisposable
    {
        #region Fields

        private readonly string databaseConnection;

        private readonly SQLiteConnection sqliteConnection;

        #endregion

        #region Constructor

        public CityRepository()
        {
            databaseConnection = GetDatabaseConnection();
            sqliteConnection = new SQLiteConnection(databaseConnection);
            sqliteConnection.Open();
        }

        #endregion

        #region Public methods

        public IEnumerable<City> GetAll()
        {
            return GetSqliteCommandResult("SELECT * FROM CITYLIST");
        }

        public async Task<IEnumerable<City>> GetAllAsync()
        {
            return await Task.Run(() => GetSqliteCommandResult("SELECT * FROM CITYLIST"));
        }

        public City GetWithId(string cityId)
        {
            return GetSqliteCommandResult($"SELECT * FROM CITYLIST WHERE id = {cityId}").FirstOrDefault();
        }

        public async Task<City> GetWithIdAsync(string cityId)
        {
            return await Task.Run(() => GetWithId(cityId));
        }

        public IEnumerable<City> GetAllWithName(string cityName)
        {
            cityName = cityName.ToLower().Trim();

            var tempResult = GetSqliteCommandResult(PrepareGetAllWithNameCommand(cityName));

            return tempResult.Where(x => RemoveAccents(x.Name.ToLower()) == RemoveAccents(cityName));
        }

        public async Task<IEnumerable<City>> GetAllWithNameAsync(string cityName)
        {
            return await Task.Run(() => GetAllWithName(cityName));
        }

        public void Dispose()
        {
            sqliteConnection.Dispose();
        }

        #endregion

        #region Private methods

        private string GetDatabaseConnection([CallerFilePath] string sourceFilePath = "")
        {
            return $"Data Source = {Path.Combine(Path.GetDirectoryName(sourceFilePath), "CityList.db")}";
        }

        private IEnumerable<City> GetSqliteCommandResult(string commandToExecute)
        {
            using (var command = new SQLiteCommand(commandToExecute, sqliteConnection))
            {
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    var result = new List<City>();

                    while (dataReader.Read())
                    {
                        result.Add(new City()
                        {
                            Id = dataReader.GetInt32(0),
                            Name = dataReader.GetString(1),
                            Country = dataReader.GetString(3),
                            Longtitude = dataReader.GetDouble(4),
                            Latitude = dataReader.GetDouble(5),
                        });
                    }

                    return result;
                }
            }
        }

        private string PrepareGetAllWithNameCommand(string cityName)
        {
            var charsToCheck = new List<char>() { 'l', 'a', 'c', 'e', 'o', 'n', 's', 'z', 'u', 'y', 'i' };
            var rootCommand = "SELECT * FROM CITYLIST WHERE LOWER(name) LIKE ";

            cityName = RemoveAccents(cityName);

            for (int i = 0; i < charsToCheck.Count; i++)
            {
                if (cityName.Contains(charsToCheck[i]))
                {
                    cityName = cityName.Replace(charsToCheck[i], '_');
                }
            }

            return string.Concat(rootCommand, $"'{cityName}'");
        }

        private string RemoveAccents(string input)
        {
            return input.Replace('ł', 'l')
                        .Replace('ą', 'a')
                        .Replace('ć', 'c')
                        .Replace('ę', 'e')
                        .Replace('ó', 'o')
                        .Replace('ń', 'n')
                        .Replace('ś', 's')
                        .Replace('ź', 'z')
                        .Replace('ż', 'z')
                        .Replace('á', 'a')
                        .Replace('à', 'a')
                        .Replace('â', 'a')
                        .Replace('ã', 'a')
                        .Replace('ä', 'a')
                        .Replace('é', 'e')
                        .Replace('è', 'e')
                        .Replace('ê', 'e')
                        .Replace('ë', 'e')
                        .Replace('í', 'i')
                        .Replace('ì', 'i')
                        .Replace('î', 'i')
                        .Replace('ï', 'i')
                        .Replace('ò', 'o')
                        .Replace('ô', 'o')
                        .Replace('õ', 'o')
                        .Replace('ö', 'o')
                        .Replace('ú', 'u')
                        .Replace('ù', 'u')
                        .Replace('û', 'u')
                        .Replace('ü', 'u')
                        .Replace('ý', 'y')
                        .Replace('ñ', 'n')
                        .Replace('ç', 'c');
        }

        #endregion
    }
}
