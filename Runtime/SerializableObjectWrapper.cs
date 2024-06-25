using System;
using UnityEngine;

namespace EZConditions
{
    /// <summary>
    /// Generalised class to serialise System.Object. Cannot guarantee this will work super well.
    /// Keep an eye on this
    /// </summary>
    [System.Serializable]
    public class SerializableObjectWrapper
    {
        public string TypeName;
        public string JsonData;
        /// <summary>
        /// This is assigned if the object is a Unity Object, otherwise null.
        /// </summary>
        [SerializeField] private UnityEngine.Object UnityObject;
        public System.Object cachedObject;

        /// <summary>
        /// Serialized version of obj
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        public SerializableObjectWrapper(object obj)
        {
            // Copy constructor. Copy Constructor itself is probably redundant now
            if (obj is SerializableObjectWrapper original)
            {
                if (original.GetObject() != null)
                {
                    TypeName = original.TypeName;
                    JsonData = original.JsonData;
                    cachedObject = original.cachedObject;
                    UnityObject = original.UnityObject;
                    return;
                }
            }
            
            if (obj is UnityEngine.Object UEO)
            {
                UnityObject = UEO;
            }
            else if (obj != null)
            {
                Type type = obj.GetType();
                TypeName = type.AssemblyQualifiedName;

                if (type.IsPrimitive || obj is string || obj is decimal)
                {
                    // Serialize primitive types and strings directly as JSON data
                    JsonData = obj.ToString();
                }
                else if (obj is Type tp)
                {
                    TypeName = typeof(System.Type).AssemblyQualifiedName;
                    JsonData = tp.AssemblyQualifiedName;
                }
                else
                {
                    JsonData = JsonUtility.ToJson(obj);
                }
            }
            cachedObject = obj;
        }

        public SerializableObjectWrapper(SerializableObjectWrapper original)
        {
            if (original.GetObject() != null)
            {
                TypeName = original.TypeName;
                JsonData = original.JsonData;
                cachedObject = original.cachedObject;
                UnityObject = original.UnityObject;
            }
        }

        /// <summary>
        /// The wrapped Object
        /// </summary>
        /// <returns>The deserialized object.</returns>
        public object GetObject()
        {

            if (UnityObject != null)
            {
                return UnityObject;
            }

            if (cachedObject != null)
            {
                return cachedObject;
            }

            if (!string.IsNullOrEmpty(TypeName))
            {
                Type type = Type.GetType(TypeName);
                if (type != null && !string.IsNullOrEmpty(JsonData))
                {
                    if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
                    {
                        cachedObject = Convert.ChangeType(JsonData, type);
                    }
                    else if (type == typeof(System.Type))
                    {
                        cachedObject = Type.GetType(JsonData);
                    }
                    else
                    {
                        cachedObject = JsonUtility.FromJson(JsonData, type);
                    }
                }
            }

            return cachedObject;
        }

        public override string ToString()
        {
            var obj = GetObject();
            return obj != null ? obj.ToString() : "null";
        }
    }
}
