using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherBar.Utils.Extensions;

namespace WeatherBar.Tests
{
    [TestClass]
    public class ObjectExtensionsUnitTests
    {
        private class TestClass
        {
            public bool TestProperty_1 { get; set; }

            public string TestProperty_2 { get; set; }
        }

        [TestMethod]
        public void DeepCompare_TestMethod()
        {
            //Arrange
            TestClass testObject_1 = new TestClass()
            {
                TestProperty_2 = "AAA"
            };
            TestClass testObject_2 = new TestClass()
            { 
                TestProperty_2 = "AAA"
            };

            // Act
            var result = testObject_1.DeepCompare(testObject_2);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DeepCompare_TestMethod_2()
        {
            //Arrange
            TestClass testObject_1 = new TestClass()
            {
                TestProperty_2 = "AAA"
            };
            TestClass testObject_2 = new TestClass();

            // Act
            var result = testObject_1.DeepCompare(testObject_2);

            //Assert
            Assert.IsTrue(!result);
        }

        [TestMethod]
        public void DeepCompare_TestMethod_3()
        {
            //Arrange
            TestClass testObject_1 = new TestClass()
            {
                TestProperty_1 = true
            };
            TestClass testObject_2 = new TestClass();

            testObject_2.TestProperty_1 = true;

            // Act
            var result = testObject_1.DeepCompare(testObject_2);

            //Assert
            Assert.IsTrue(result);
        }
    }
}
