using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GraphEditor.Interfaces.Utils
{
    public static class Helper
    {
        public static byte[] LoadGraphicFromResource(string resourcePath, Assembly assembly)
        {
            var src = new BitmapImage();
            try
            {
                src.BeginInit();
                src.UriSource = new Uri($"pack://application:,,,/{assembly.GetName()};component{resourcePath}");
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
            }
            catch (IOException)
            {
                Console.WriteLine($"Resource '{resourcePath}' not found");
                return null;
            }

            using (var ms = new MemoryStream())
            {
                var pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(src));
                pngEncoder.Save(ms);
                return ms.GetBuffer();
            }
        }

        public static Point ToPoint(this string point, char seperator = ';')
        {
            return Point.Parse(string.Join(",", point.Split(seperator)));
        }
    }
}
