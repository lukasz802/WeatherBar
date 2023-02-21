using Newtonsoft.Json;

namespace WeatherBar.Utils.Extensions
{
    public static class ObjectExtensions
    {
        #region Public methods

        public static bool DeepCompare<T>(this T objA, T objB)
        {
            if (objA == null || objB == null)
            {
                return false;
            }

            return Serialize(objA).Equals(Serialize(objB));
        }

        #endregion

        #region Private methods

        private static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        #endregion
    }
}
