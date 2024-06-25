using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EZConditions
{
    public static class ConditionUtility
    {
        public const string VERSION = "1.0.0-beta.3";
        private const string LOGBEGIN = "\n---EZConditions---";
        private const string LOGEND = "---END EZConditions---\n";

        /// <summary>
        /// A collection of cached methods for a higher hit rate. (Not implemented yet)
        /// </summary>
        public static Dictionary<string, MethodInfo> ConditionCache = new Dictionary<string, MethodInfo>();

        #region ConditionFunctions
        /// <summary>
        /// Returns a random float from [min, max]
        /// </summary>
        /// <param name="minInclusive">Minimum number to consider</param>
        /// <param name="maxInclusive">Maximum number to consider</param>
        /// <returns>Random valuue between min (inclusive) and max (inclusive)</returns>
        [Condition]
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

        #endregion

        #region Logging
        /// <summary>
        /// Logs information to console or player log. Decorated for easier detection in Player Log
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="category">1 for warning, 2 for error. Anything else is a regular log</param>
        internal static void Log(string message, short category = 0)
        {

            if (category == 1)
                LogWarning(message);
            else if (category == 2)
                LogError(message);
            else
            {
                Debug.Log(LOGBEGIN);
                Debug.Log(message);
                Debug.Log(LOGEND);
            }

        }

        /// <summary>
        /// Logs Warning to console or player log. Decorated for easier detection in Player Log
        /// </summary>
        /// <param name="message">Message to log</param>
        internal static void LogWarning(string message)
        {
            Debug.Log(LOGBEGIN + ": WARNING!");
            Debug.LogWarning(message);
            Debug.Log(LOGEND);
        }

        /// <summary>
        /// Logs Error to console or player log. Decorated for easier detection in Player Log
        /// </summary>
        /// <param name="message">Message to log</param>
        internal static void LogError(string message)
        {
            Debug.Log(LOGBEGIN + ": ERROR!!!");
            Debug.LogError(message);
            Debug.Log(LOGEND);
        }
        /// <summary>
        /// Logs Exception to console or player loog. Decorated for easier detection
        /// </summary>
        /// <param name="exception">Exception to log</param>
        internal static void LogException(System.Exception exception)
        {
            Debug.Log(LOGBEGIN + "EXCEPTION!!!!!!!");
            Debug.LogException(exception);
            Debug.Log(LOGEND);
        }
        #endregion

    }
}
