using System;
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

    }

}
