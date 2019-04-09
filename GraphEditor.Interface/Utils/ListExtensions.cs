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

        public static void InverseFor<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            var list = collection.ToList();
            for (var index = list.Count - 1; index >= 0; index--)
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

        public static void InverseFor<T>(this ObservableCollection<T> collection, Action<T, int> action)
        {
            var list = collection.ToList();
            for (var index = list.Count - 1; index >= 0; index--)
            {
                action?.Invoke(list[index], index);
            }
        }
    }
}
