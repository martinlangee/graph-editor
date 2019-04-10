using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GraphEditor.Interface.Utils
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

        public static Point Round(this Point point)
        {
            return new Point(Math.Round(point.X), Math.Round(point.Y));
        }

        public static bool IsEqual(this Point p1, Point p2)
        {
            return p1.X.Equals(p2.X) && p1.Y.Equals(p2.Y);
        }
    }
}
