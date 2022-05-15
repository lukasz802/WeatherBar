using Microsoft.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApi;

namespace WeatherBarTests
{
    [TestClass]
    public class WebApiUnitTests
    {

        [TestMethod]
        public void HttpOperationException_TestMethod_1()
        {
            //Arrange
            WeatherApi client = new WeatherApi("c5976f0996947c1488798209b0bc3f77", "756135", 15);

            //Assert
            Assert.ThrowsException<HttpOperationException>(() => client.GetCurrentWeatherData("AA"));
        }

        [TestMethod]
        public void HttpOperationException_TestMethod_2()
        {
            //Arrange
            WeatherApi client = new WeatherApi("c5976f0996947c1488798209b0bc3f77", "756135", 15);

            //Assert
            Assert.ThrowsException<HttpOperationException>(() => client.GetFourDaysForecastData("AA"));
        }
    }
}
