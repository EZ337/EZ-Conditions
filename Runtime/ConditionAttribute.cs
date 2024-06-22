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
        /// Whether the ConditionManager is expected to handle this function's returned.
        /// If true, Specify ComparedType to show the appropriate field to use for comparison.
        /// Compared type is ignored if this value is false
        /// </summary>
        public bool CompareReturnedValue { get; private set; } = false;

        /// <summary>
        /// The type we compare against the returned value. <b>NOTE: whatever ComparedType is must implement IComparable</b>
        /// </summary>
        public Type ComparedType { get; private set; } = null;

        /// <summary>
        /// Up to 16 parameters. If looping through and a null is encountered, treats everything else as null
        /// Defaults to no argument
        /// </summary>
        public Type[] Parameters { get; private set; } = new Type[0];

        public ConditionAttribute(Type[] Parameters = null, bool CompareReturnedValue = false, Type ComparedType = null)
        {
            if (Parameters != null)
            {
                this.Parameters = Parameters;
            }

            this.CompareReturnedValue = CompareReturnedValue;
            this.ComparedType = ComparedType;
        }


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
