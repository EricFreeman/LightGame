using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

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

        public static Vector3 MoveTowards(this Vector3 pos, Vector3 end, float speed)
        {
            return Vector3.MoveTowards(pos, end, speed);
        }
    }
}