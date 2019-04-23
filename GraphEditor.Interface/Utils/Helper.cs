#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
// License for the specific language governing rights and limitations under the License.
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
