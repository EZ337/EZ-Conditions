using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using EZConditions.LRU;

namespace EZConditions
{
    public static class ConditionUtility
    {
        public const string VERSION = "1.0.0-beta.4";
        private const string LOGBEGIN = "\n---EZConditions---";
        private const string LOGEND = "---END EZConditions---\n";
        /// <summary>
        /// Change of the cache size only takes effect when ConditionUtility is reinitialised
        /// </summary>
        public static int CACHESIZE = 30;

        /// <summary>
        /// A collection of cached methods for a higher hit rate. (Not implemented yet)
        /// </summary>
        public static LRUCache<string, MethodInfo> ConditionCache = new LRUCache<string, MethodInfo>(CACHESIZE);

        #region ConditionFunctions
        /// <summary>
        /// Returns a random float from [min, max]
        /// </summary>
        /// <param name="minInclusive">Minimum number to consider</param>
        /// <param name="maxInclusive">Maximum number to consider</param>
        /// <returns>Random value between min (inclusive) and max (inclusive)</returns>
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
        /// <returns>Random value between min (inclusive) and max (exclusive)</returns>
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
#if !UNITY_EDITOR
                Debug.Log(LOGBEGIN);
#endif
                Debug.Log(message);
#if !UNITY_EDITOR

                Debug.Log(LOGEND);
#endif
            }

        }

        /// <summary>
        /// Logs Warning to console or player log. Decorated for easier detection in Player Log
        /// </summary>
        /// <param name="message">Message to log</param>
        internal static void LogWarning(string message)
        {
#if !UNITY_EDITOR

            Debug.Log(LOGBEGIN + ": WARNING!");
#endif
            Debug.LogWarning(message);
#if !UNITY_EDITOR

            Debug.Log(LOGEND);
#endif
        }

        /// <summary>
        /// Logs Error to console or player log. Decorated for easier detection in Player Log
        /// </summary>
        /// <param name="message">Message to log</param>
        internal static void LogError(string message)
        {
#if !UNITY_EDITOR
            Debug.Log(LOGBEGIN + ": ERROR!!!");
#endif
            Debug.LogError(message);
#if !UNITY_EDITOR
            Debug.Log(LOGEND);
#endif
        }
        /// <summary>
        /// Logs Exception to console or player loog. Decorated for easier detection
        /// </summary>
        /// <param name="exception">Exception to log</param>
        internal static void LogException(System.Exception exception)
        {
#if !UNITY_EDITOR
            Debug.Log(LOGBEGIN + "EXCEPTION!!!!!!!");
#endif
            Debug.LogException(exception);
#if !UNITY_EDITOR
            Debug.Log(LOGEND);
#endif
        }
#endregion

    }

    namespace LRU
    {
        public class LRUCache<K, V>
        {
            private readonly int capacity;
            private readonly Dictionary<K, LinkedListNode<CacheItem>> cacheMap;
            private readonly LinkedList<CacheItem> lruList;

            /// <summary>
            /// Simple class to store the key/value pair so I can remove it from the dictionary
            /// </summary>
            private class CacheItem
            {
                public K Key { get; set; }
                public V Value { get; set; }
            }
            /// <summary>
            /// Creates an LRU with size = capacity
            /// </summary>
            /// <param name="capacity">Capacity of the LRU before replacements take place</param>
            /// <exception cref="ArgumentException"></exception>
            public LRUCache(int capacity)
            {
                if (capacity <= 0) throw new ArgumentException("Capacity must be greater than zero.");
                this.capacity = capacity;
                cacheMap = new Dictionary<K, LinkedListNode<CacheItem>>(capacity);
                lruList = new LinkedList<CacheItem>();
            }

            /// <summary>
            /// Gets the value associated with the specified key from the LRU cache.
            /// Returns null if the key is not found.
            /// </summary>
            /// <param name="key">The key to look up.</param>
            /// <returns>The value associated with the key, or null if not found.</returns>
            public V Get(K key)
            {
                if (cacheMap.TryGetValue(key, out LinkedListNode<CacheItem> node))
                {
                    lruList.Remove(node);
                    lruList.AddFirst(node);
                    return node.Value.Value;
                }
                // Return default value (null for reference types, or default value for value types)
                return default;
            }


            public void Put(K key, V value)
            {
                if (cacheMap.TryGetValue(key, out var node))
                {
                    lruList.Remove(node);
                    node.Value.Value = value;
                    lruList.AddFirst(node);
                }
                else
                {
                    if (cacheMap.Count >= capacity)
                    {
                        var lastNode = lruList.Last;
                        if (lastNode != null)
                        {
                            cacheMap.Remove(lastNode.Value.Key);
                            lruList.RemoveLast();
                        }
                    }

                    var newItem = new CacheItem { Key = key, Value = value };
                    var newNode = new LinkedListNode<CacheItem>(newItem);
                    lruList.AddFirst(newNode);
                    cacheMap[key] = newNode;
                }
            }


        }
    }

}
