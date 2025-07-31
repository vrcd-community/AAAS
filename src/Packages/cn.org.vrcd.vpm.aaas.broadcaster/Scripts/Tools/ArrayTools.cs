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
    }
}
