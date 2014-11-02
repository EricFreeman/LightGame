using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Util
{
    public static class Extensions
    {
        public static T Random<T>(this IEnumerable<T> collection)
        {
            var rand = new Random();
            var enumerable = collection as T[] ?? collection.ToArray();
            return enumerable.ToList()[rand.Next(0, enumerable.Count() - 1)];
        }
    }
}
