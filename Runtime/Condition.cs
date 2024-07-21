using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EZConditions
{
    [System.Serializable]
    public class Condition
    {
        #region Fields
        [SerializeField] private SerializableObjectWrapper obj;
        [SerializeField] private ConditionComparator comparator;
        [SerializeField] private bool or = false;
        [SerializeField] private string methodName;
        [SerializeField, HideInInspector] private SerializableObjectWrapper[] parameters;
        [SerializeField] private SerializableObjectWrapper comparedValue;

        // They already not serialized since private but hey...
        [NonSerialized] private MethodInfo function;
        #endregion

        #region Properties
        public System.Object Obj { get => obj.GetObject(); private set => obj = new SerializableObjectWrapper(value); }
        public ConditionComparator Comparator { get => comparator; private set => comparator = value; }
        public bool OR { get => or; private set => or = value; } // Whether this condition treated as AND or OR

        // MethodInfo Serialization since Unity does not serialize by default.
        // Reflections can be a performance concern so worth coming back to
        // TODO: Update to a custom [better] serialization approach
        public string MethodName { get => methodName; private set => methodName = value; }
        public MethodInfo Function
        {
            // Creates and caches function if its null.
            get
            {
                if (function == null)
                {
                    // Try and get Cached Method
                    function = ConditionUtility.ConditionCache.Get(obj.TypeName + MethodName);

                    // Recreate the function and add it to the cache if we don't have it in cache
                    if (function == null)
                    {
                        // Static Method support
                        if (obj.GetObject() is Type tp)
                        {
                            function = tp.GetMethod(methodName);
                        }
                        else
                        {
                            function = Obj.GetType().GetMethod(MethodName);
                        }

                        // Error. Condition is invalid
                        if (function == null)
                        {
                            ConditionUtility.LogError("Condition: Critical Error. Was unable to fetch condition function.");
#if UNITY_EDITOR
                            Debug.LogError("Creating Conditions with the \"+\" button is unsupported. If that's not your issue, Let EZ Know");
#endif
                            throw new ArgumentNullException($"Condition Method: {MethodName} Could not be constructed");
                        }

                        // Add this to the Cache of methods 
                        ConditionUtility.ConditionCache.Put(obj.TypeName + MethodName, function);
                    }
                }

                return function;
            }
            private set { function = value; ConditionUtility.ConditionCache.Put(obj.TypeName + MethodName, function); }
        }
        public System.Object ComparedValue { get => comparedValue.GetObject(); private set => comparedValue = new SerializableObjectWrapper(value); }

        /// <summary>
        /// If this condition is a valid condition. i.e. There wouldn't be any problems using it
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (!string.IsNullOrEmpty(MethodName) && Function != null)
                {
                    if (Obj != null || Function.IsStatic)
                    {
                        int i = 0;
                        foreach (ParameterInfo paramInfo in Function.GetParameters())
                        {
                            if (paramInfo.ParameterType.AssemblyQualifiedName != Parameters[i++].TypeName)
                                return false;
                        }

                        return true;
                    }
                }


                return false;
            }
        }

        /// <summary>
        /// The parameters associated with this function
        /// </summary>
        public SerializableObjectWrapper[] Parameters { get => parameters; set => parameters = value; }
        #endregion

        /// <summary>
        /// Creates a condition
        /// </summary>
        /// <param name="obj">Do NOT Wrap obj in SerializableObjectWrapper. It is done in the Condition</param>
        /// <param name="function">Function call</param>
        /// <param name="args">Parameters wrapped in SerializableObjectWrapper</param>
        /// <param name="comparator"></param>
        /// <param name="comparedValue"><b>Do NOT Wrap</b> compared value in SerializableObjectWrapper</param>
        /// <param name="OR">True means condition is OR. False means condition is AND</param>
        public Condition(System.Object obj, MethodInfo function, SerializableObjectWrapper[] args, 
            ConditionComparator comparator, System.Object comparedValue, bool OR)
        {
            // Could be null for static methods?
            Obj = obj;
            Function = function;
            MethodName = function.Name;
            Parameters = args;
            Comparator = comparator;
            ComparedValue = comparedValue;
            this.OR = OR;
        }

        public bool EvaluateCondition()
        {
            return Evaluate(Obj, Function, Parameters, Comparator, ComparedValue);
        }

        /// <summary>
        /// Evaluates obj against param2 based off of function. Returns the comparison. Returns false if unable to compare
        /// </summary>
        /// <param name="obj">Object owning the function call. Must be thhe declaring type for function.
        /// It's also the subject of the condition</param>
        /// <param name="function">Condition predicate. The question being asked. Method with the [Condition] attribute</param>
        /// <param name="comparator">The comparison operator to check foro</param>
        /// <param name="comparedValue">The other object to compare against</param>
        /// <returns>A valid comparison of true or false. False if unable to compare the two objects</returns>
        public static bool Evaluate(System.Object obj, MethodInfo function, SerializableObjectWrapper[] argsList, ConditionComparator comparator, System.Object comparedValue)
        {
            // Construct Args
            List<System.Object> arguments = new List<System.Object>();
            foreach (var item in argsList)
            {
                arguments.Add(item.GetObject()); 
            }

            IComparable ret = function.Invoke(obj, arguments.ToArray()) as IComparable; // Call the function

            /* Technically redundant now. I'm going to allow it to crash. If it does crash, we know we really messed up
            if (ret is IComparable comparableRet && param2 is IComparable comparableParam2)
            {
                return Compare(comparableRet.CompareTo(comparableParam2), comparator);
            }*/
            return Compare(ret.CompareTo(comparedValue as IComparable), comparator);

            //Debug.LogWarning($"{this}: Cannot compare {obj} and {(param2 ?? "Null")}.");

        }

        private static bool Compare(int comparison, ConditionComparator comparator)
        {
            return comparator switch
            {
                ConditionComparator.EqualTo => comparison == 0,
                ConditionComparator.NotEqualTo => comparison != 0,
                ConditionComparator.GreaterThan => comparison > 0,
                ConditionComparator.GreaterThanOrEqual => comparison >= 0,
                ConditionComparator.LessThan => comparison < 0,
                ConditionComparator.LessThanOrEqual => comparison <= 0,
                _ => false,
            };
        }

        #region Utility
        public void Reconstruct()
        {
            this.Function = null;
        }

        /// <summary>
        /// Defines the implicit cconversion to a bool. Returns the evaluated condition
        /// </summary>
        /// <param name="condition">Condition to evaluate</param>
        public static implicit operator bool(Condition condition)
        {
            return condition.EvaluateCondition();
        }

        public static string ComparatorString(ConditionComparator comparator)
        {
            return comparator switch
            {
                ConditionComparator.EqualTo => "==",
                ConditionComparator.NotEqualTo => "!=",
                ConditionComparator.GreaterThan => ">",
                ConditionComparator.LessThan => "<",
                ConditionComparator.GreaterThanOrEqual => ">=",
                ConditionComparator.LessThanOrEqual => "<=",
                _ => throw new ArgumentOutOfRangeException(nameof(comparator), comparator, "null"),
            };
        }


        /// <summary>
        /// String representation of Condition
        /// </summary>
        /// <returns>String repr of condition</returns>
        public override string ToString()
        {
            string OrTxt = (OR) ? "OR" : "AND";
            string Param2Txt = (ComparedValue == null) ? "Null" : ComparedValue.ToString();
            string MethodTxt = (string.IsNullOrEmpty(MethodName)) ? "NoFunction" : MethodName; 

            return $"{Obj}.{MethodTxt} {ComparatorString(Comparator)} {Param2Txt} {OrTxt}";
        }

        /* Eh Can be nice to have I suppose
        public string CleanerToString()
        {
            string OrTxt = (OR) ? "OR" : "AND";
            string Param2Txt = (string.IsNullOrEmpty(param2Value)) ? "Null" : param2Value;
            if (param2AsUnityObject != null) 
            {
                Param2Txt = param2AsUnityObject.GetType().ToString();
            }
            return $"{Obj.name}.{MethodName} {ComparatorString(Comparator)} {Param2Txt}";
        }
        */

        #endregion
    }

    public enum ConditionComparator
    {
        EqualTo,
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
        NotEqualTo
    }

}
