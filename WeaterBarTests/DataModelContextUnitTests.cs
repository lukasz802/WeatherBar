using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using WeatherBar.Models.Repositories;

namespace WeatherBarTests
{
    [TestClass]
    public class DataModelContextUnitTests
    {
        [TestMethod]
        public void GetAllCities_TestMethod()
        {
            //Arrange
            CityRepository repository = new CityRepository();

            // Act
            var cityList = repository.GetAll();

            //Assert
            Assert.IsTrue(cityList.ToList().Count != 0);
        }

        [TestMethod]
        public void GetCityByName_TestMethod_1()
        {
            //Arrange
            CityRepository repository = new CityRepository();

            // Act
            var cityList = repository.GetAllWithName(" gdansk ");

            //Assert
            Assert.IsTrue(cityList.First().Name == "Gdańsk");
            Assert.IsTrue(cityList.First().Country == "PL");
        }

        [TestMethod]
        public void GetCityByName_TestMethod_2()
        {
            //Arrange
            CityRepository repository = new CityRepository();

            // Act
            var cityList = repository.GetAllWithName("Lodz");

            //Assert
            Assert.IsTrue(cityList.First().Name == "Łódź");
            Assert.IsTrue(cityList.First().Country == "PL");
        }

        [TestMethod]
        public void GetCityByName_TestMethod_3()
        {
            //Arrange
            CityRepository repository = new CityRepository();

            // Act
            var cityList = repository.GetAllWithName("Zuromin");

            //Assert
            Assert.IsTrue(cityList.First().Name == "Żuromin");
            Assert.IsTrue(cityList.First().Country == "PL");
        }

        [TestMethod]
        public void GetCityByName_TestMethod_4()
        {
            //Arrange
            CityRepository repository = new CityRepository();

            // Act
            var cityList = repository.GetAllWithName("gmd");

            //Assert
            Assert.IsTrue(cityList.ToList().Count == 0);
        }
    }
}
