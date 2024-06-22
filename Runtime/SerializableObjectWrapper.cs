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
        /// This is assigned if the object is a unityObject otherwise, null
        /// </summary>
        public UnityEngine.Object UnityObject;

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
                TypeName = obj.GetType().AssemblyQualifiedName;
                JsonData = JsonUtility.ToJson(obj);
            }
        }

        /// <summary>
        /// The wrapped Object
        /// </summary>
        /// <returns></returns>
        public object GetObject()
        {
            if (UnityObject != null)
            {
                return UnityObject;
            }
            if (!string.IsNullOrEmpty(TypeName) && !string.IsNullOrEmpty(JsonData))
            {
                Type type = Type.GetType(TypeName);
                if (type != null)
                {
                    return JsonUtility.FromJson(JsonData, type);
                }
            }
            return null;
        }

        public override string ToString()
        {
            return GetObject().ToString();
        }
    }

}

