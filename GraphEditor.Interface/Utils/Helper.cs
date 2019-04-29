#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
#endregion

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GraphEditor.Interface.Utils
{
    public static class Helper
    {
        public static byte[] LoadGraphicFromResource(string resourceName, Assembly assembly)
        {
            var src = new BitmapImage();
            try
            {
                src.BeginInit();
                src.UriSource = new Uri($"pack://application:,,,/{assembly.GetName()};component/{resourceName}");
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
            }
            catch (IOException)
            {
                Console.WriteLine($"Resource '{resourceName}' not found");
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

        public static Color ToColor(this uint color)
        {
            var a = (byte) ((color >> 24) & 0xFF);
            var r = (byte) ((color >> 16) & 0xFF);
            var g = (byte) ((color >> 8) & 0xFF);
            var b = (byte) (color & 0xFF);

            return Color.FromArgb(a, r, g, b);
        }

        public static uint ToUint(this Color color)
        {
            uint a = ((uint) color.A) << 24;
            uint r = ((uint) color.R) << 16;
            uint g = ((uint) color.G) << 8;
            uint b = ((uint) color.B);

            return a | r | g | b;
        }
    }
}
