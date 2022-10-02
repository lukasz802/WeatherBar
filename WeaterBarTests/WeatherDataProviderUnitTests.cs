using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;

namespace WeatherBarTests
{
    [TestClass]
    public class WeatherDataProviderUnitTests
    {

        [TestMethod]
        public void HttpOperationException_TestMethod_1()
        {
            //Arrange
            WeatherDataProvider.WeatherDataProvider provider = new WeatherDataProvider.WeatherDataProvider("c5976f0996947c1488798209b0bc3f77");

            //Assert
            Assert.ThrowsException<HttpException>(() => provider.GetCurrentForecast("AA"));
        }

        [TestMethod]
        public void HttpOperationException_TestMethod_2()
        {
            //Arrange
            WeatherDataProvider.WeatherDataProvider provider = new WeatherDataProvider.WeatherDataProvider("c5976f0996947c1488798209b0bc3f77");

            //Assert
            Assert.ThrowsException<HttpException>(() => provider.GetFourDaysForecast("AA"));
        }
    }
}
