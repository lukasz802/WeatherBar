using System.IO;
using System.Windows.Media.Imaging;

namespace WeatherBar.Utils
{
    public static class SharedFunctions
    {
        #region Public methods

        public static BitmapImage LoadImage(Stream imageStream)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = imageStream;
            image.EndInit();
            image.Freeze();

            return image;
        }

        #endregion
    }
}
