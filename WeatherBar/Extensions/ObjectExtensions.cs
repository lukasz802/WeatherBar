using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WeatherBar.Extensions
{
    public static class ObjectExtensions
    {
        public static bool DeepCompare(this object obj, object another)
        {
            if (ReferenceEquals(obj, another))
            {
                return true;
            }

            if (obj == null || another == null)
            {
                return false;
            }

            if (obj.GetType() != another.GetType())
            {
                return false;
            }

            string objJson = JsonConvert.SerializeObject(obj);
            string anotherObjJson = JsonConvert.SerializeObject(another);

            return objJson == anotherObjJson;
        }
    }
}
