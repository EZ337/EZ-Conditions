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

        /// <summary>
        /// Serialized version of obj
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        public SerializableObjectWrapper(object obj)
        {
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
                else
                {
                    JsonData = JsonUtility.ToJson(obj);
                }
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

            if (!string.IsNullOrEmpty(TypeName))
            {
                Type type = Type.GetType(TypeName);
                if (type != null)
                {
                    if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
                    {
                        return Convert.ChangeType(JsonData, type);
                    }
                    if (!string.IsNullOrEmpty(JsonData))
                    {
                        return JsonUtility.FromJson(JsonData, type);
                    }
                }
            }

            return null;
        }

        public override string ToString()
        {
            var obj = GetObject();
            return obj != null ? obj.ToString() : "null";
        }
    }
}
