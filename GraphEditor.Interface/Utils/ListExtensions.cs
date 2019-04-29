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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GraphEditor.Interface.Utils
{
    public static class ListExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action?.Invoke(item);
            }
        }

        public static void For<T>(this IEnumerable<T> enumerable, Action<T,int> action, int from = -1, int to = -1)
        {
            var list = enumerable.ToList();

            var firstIdx = from < 0 ? 0 : from;
            var lastidx = to < 0 ? list.Count - 1 : to;

            for (var index = firstIdx; index <= lastidx; index++)
            {
                action?.Invoke(list[index], index);
            }
        }

        public static void ForEach<T>(this ObservableCollection<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action?.Invoke(item);
            }
        }

        public static void For<T>(this ObservableCollection<T> collection, Action<T, int> action, int from = -1, int to = -1)
        {
            var list = collection.ToList();

            var firstIdx = from < 0 ? 0 : from;
            var lastIdx = to < 0 ? list.Count - 1 : to;

            for (var index = firstIdx; index <= lastIdx; index++)
            {
                action?.Invoke(list[index], index);
            }
        }
    }
}
