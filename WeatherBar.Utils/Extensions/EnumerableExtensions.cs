using System.Collections;

namespace WeatherBar.Utils.Extensions
{
    public static class EnumerableExtensions
    {
        #region Public methods

        public static int Count(this IEnumerable source)
        {
            int result = 0;

            foreach (var item in source)
            {
                result++;
            }

            return result;
        }

        #endregion
    }
}
