using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherBar.Models.Repositories
{
    public class CityRepository : IDisposable
    {
        #region Fields

        private const string databaseName = "CityList.db";

        private readonly string databaseConnection = $"Data Source = {Path.Combine(Directory.GetCurrentDirectory(), databaseName)}";

        private readonly SQLiteConnection sqliteConnection;

        #endregion

        #region Constructor

        public CityRepository()
        {
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

        public IEnumerable<City> GetAllWithName(string cityName)
        {
            return !string.IsNullOrEmpty(cityName) ? GetSqliteCommandResult(PrepareGetAllWithNameCommand(cityName.ToLower().Trim())) : Enumerable.Empty<City>();
        }

        public async Task<IEnumerable<City>> GetAllWithNameAsync(string cityName)
        {
            return !string.IsNullOrEmpty(cityName) ? await Task.Run(() => GetSqliteCommandResult(PrepareGetAllWithNameCommand(cityName.ToLower().Trim()))) : Enumerable.Empty<City>();
        }

        public void Dispose()
        {
            sqliteConnection.Dispose();
        }

        #endregion

        #region Private methods

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
                            Longtitude = dataReader.GetDecimal(4),
                            Latitude = dataReader.GetDecimal(5),
                        });
                    }

                    return result;
                }
            }
        }

        private string PrepareGetAllWithNameCommand(string cityName)
        {
            var charsToCheck = new List<char>() { 'l', 'a', 'c', 'e', 'o', 'n', 's', 'z', };
            var rootCommand = "SELECT * FROM CITYLIST WHERE";
            var tempCommand = "LOWER(name)";

            cityName = cityName.Replace('ł', 'l')
                               .Replace('ą', 'a')
                               .Replace('ć', 'c')
                               .Replace('ę', 'e')
                               .Replace('ó', 'o')
                               .Replace('ń', 'n')
                               .Replace('ś', 's')
                               .Replace('ź', 'z')
                               .Replace('ż', 'z');

            for (int i = 0; i < charsToCheck.Count; i++)
            {
                if (cityName.Contains(charsToCheck[i]))
                {
                    switch (charsToCheck[i])
                    {
                        case 'l':
                            tempCommand = $@"REPLACE({tempCommand}, ""ł"", ""l"")";
                            tempCommand = $@"REPLACE({tempCommand}, ""Ł"", ""l"")";
                            break;
                        case 'a':
                            tempCommand = $@"REPLACE({tempCommand}, ""ą"", ""a"")";
                            tempCommand = $@"REPLACE({tempCommand}, ""Ą"", ""a"")";
                            break;
                        case 'c':
                            tempCommand = $@"REPLACE({tempCommand}, ""ć"", ""c"")";
                            tempCommand = $@"REPLACE({tempCommand}, ""Ć"", ""c"")";
                            break;
                        case 'e':
                            tempCommand = $@"REPLACE({tempCommand}, ""ę"", ""e"")";
                            tempCommand = $@"REPLACE({tempCommand}, ""Ę"", ""e"")";
                            break;
                        case 'o':
                            tempCommand = $@"REPLACE({tempCommand}, ""ó"", ""o"")";
                            tempCommand = $@"REPLACE({tempCommand}, ""Ó"", ""o"")";
                            break;
                        case 'n':
                            tempCommand = $@"REPLACE({tempCommand}, ""ń"", ""n"")";
                            tempCommand = $@"REPLACE({tempCommand}, ""Ń"", ""n"")";
                            break;
                        case 's':
                            tempCommand = $@"REPLACE({tempCommand}, ""ś"", ""s"")";
                            tempCommand = $@"REPLACE({tempCommand}, ""Ś"", ""s"")";
                            break;
                        case 'z':
                            tempCommand = $@"REPLACE({tempCommand}, ""ź"", ""z"")";
                            tempCommand = $@"REPLACE({tempCommand}, ""Ź"", ""z"")";
                            tempCommand = $@"REPLACE({tempCommand}, ""ż"", ""z"")";
                            tempCommand = $@"REPLACE({tempCommand}, ""Ż"", ""z"")";
                            break;
                    }
                }
            }

            return string.Concat(rootCommand, " ", tempCommand, $@" = ""{cityName}""");
        }

        #endregion
    }
}
