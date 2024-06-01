using System;
using UnityEngine;
using EZConditions;

namespace EZConditions
{
    /// <summary>
    /// Placing this attribute on a method or property signifies that method or property can be
    /// used for conditions. Providing a type shows the appropriate field to compare to.
    /// If param2 ends up being a object field, the object itself is passed into the condition function
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class ConditionAttribute : Attribute
    {

        /// <summary>
        /// Type of the first parameter. Typically the class this method beleongs to
        /// </summary>
        Type param1 = null;

        /// <summary>
        /// Type of the second parameter if applicable
        /// </summary>
        Type param2 = null;

        public ConditionAttribute(Type param2 = null)
        {
            this.param2 = param2;
        }

        /*
        /// <summary>
        /// A condition attribute that defines the amount of parameters the 
        /// method/property needs to complete the condition.
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        public ConditionAttribute(Type param1 = null, Type param2 = null)
        {
            this.Param1 = param1;
            this.Param2 = param2;
        }
        */

        public Type Param1 { get => param1; set => param1 = value; }
        public Type Param2 { get => param2; set => param2 = value; }
    }

}
