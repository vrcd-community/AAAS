using System;

namespace AAAS.Broadcaster.Tools {
    internal static class ArrayTools {
        public static T[] Add<T>(T[] array, T item) {
            if (array == null) {
                return new[] { item };
            }

            var newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            newArray[array.Length] = item;
            return newArray;
        }

        public static T[] AddRange<T>(T[] array, T[] items) {
            if (array == null || array.Length == 0) {
                return items ?? new T[0];
            }

            if (items == null || items.Length == 0) {
                return array;
            }

            var newArray = new T[array.Length + items.Length];
            Array.Copy(array, newArray, array.Length);
            Array.Copy(items, 0, newArray, array.Length, items.Length);
            return newArray;
        }
    }
}
