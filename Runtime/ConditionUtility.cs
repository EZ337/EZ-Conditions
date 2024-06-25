using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EZConditions
{
    public static class ConditionUtility
    {
        /// <summary>
        /// A collection of cached methods for a higher hit rate. (Not implemented yet)
        /// </summary>
        public static Dictionary<string, MethodInfo> ConditionCache = new Dictionary<string, MethodInfo>();

        /// <summary>
        /// Returns a random float from [min, max]
        /// </summary>
        /// <param name="minInclusive">Minimum number to consider</param>
        /// <param name="maxInclusive">Maximum number to consider</param>
        /// <returns>Random valuue between min (inclusive) and max (inclusive)</returns>
        public static float GetRandomFloat(float minInclusive, float maxInclusive)
        {
            return UnityEngine.Random.Range(minInclusive, maxInclusive);
        }

        /// <summary>
        /// Returns a random int from [min, max)
        /// </summary>
        /// <param name="minInclusive">Minimum number to consider</param>
        /// <param name="maxExclusive">Maximum number to consider</param>
        /// <returns>Random valuue between min (inclusive) and max (exclusive)</returns>
        [Condition]
        public static int GetRandomInt(int minInclusive, int maxExclusive)
        {
            return UnityEngine.Random.Range(minInclusive, maxExclusive);
        }

        /// <summary>
        /// Returns a random value from 0.0 - 1.0
        /// </summary>
        /// <returns>Random value from 0 to 1</returns>
        [Condition]
        public static float GetRandomPercent()
        {
            return UnityEngine.Random.Range(0.0f, 1.0f);
        }
    }
}
