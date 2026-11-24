using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChudoObuv_Project
{
    public static class ImageLoader
    {
        public static ImageSource? Load(string? imagePath)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            string path = Path.Combine(dir, imagePath ?? "");

            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(path))
            {
                path = Path.Combine(dir, "picture.png");
            }

            if (!File.Exists(path))
            {
                return null;
            }

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
    }
}
