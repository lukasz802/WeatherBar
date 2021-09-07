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
            WeatherApi client = new WeatherApi();

            //Assert
            Assert.ThrowsException<HttpOperationException>(() => client.GetCurrentWeatherData("AA"));
        }

        [TestMethod]
        public void HttpOperationException_TestMethod_2()
        {
            //Arrange
            WeatherApi client = new WeatherApi();

            //Assert
            Assert.ThrowsException<HttpOperationException>(() => client.GetFourDaysForecastData("AA"));
        }
    }
}
