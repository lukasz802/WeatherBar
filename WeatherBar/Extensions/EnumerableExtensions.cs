using System.Collections;

namespace WeatherBar.Extensions
{
    public static class EnumerableExtensions
    {
        public static int Count(this IEnumerable source)
        {
            int result = 0;

            foreach (var item in source)
            {
                result++;
            }

            return result;
        }
    }
}
