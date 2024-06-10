using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using EZConditions;

namespace EZConditions
{
    [System.Serializable]
    public class Condition
    {
        #region Fields
        [SerializeField] private UnityEngine.Object obj;
        [SerializeField] private ConditionComparator comparator;
        [SerializeField] private bool or = false;
        [SerializeField] private string methodName;
        [SerializeField] private string param2Type;
        [SerializeField] private string param2Value;
        [SerializeField] private UnityEngine.Object param2AsUnityObject;
        [SerializeField, HideInInspector] private bool param2Evaluation = false;

        // They already not serialized since private but hey...
        [NonSerialized] private MethodInfo function;
        [NonSerialized] private System.Object param2;
        #endregion

        #region Properties
        public UnityEngine.Object Obj { get => obj; private set => obj = value; }
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
                    if (MethodName == null)
                        Debug.LogError("MethodName was null for some reason. Let EZ Know");
                    function = Obj.GetType().GetMethod(MethodName);
                }
                if (function == null)
                {
                    Debug.LogError("Critical Error. Was unable to fetch condition function.\nCreating Conditions with the \"+\" button is unsupported. If that's not your issue, Let EZ Know");
                }

                return function;
            }
            private set { function = value; }
        }

        // System.Object Serialization since Unity does cannot serialize
        public string Param2Type { get => param2Type; private set => param2Type = value; }
        public string Param2Value { get => param2Value; private set => param2Value = value; }

        public System.Object Param2
        {
            get
            {
                try
                {
                    // Reconstruct param2 from its serialized form
                    if (param2 == null && !string.IsNullOrEmpty(param2Type))
                    {
                        var type = Type.GetType(param2Type);

                        if (type != null)
                        {
                            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                            {
                                param2 = param2AsUnityObject;
                            }
                            else if (type.IsEnum)
                            {
                                param2 = Enum.Parse(type, param2Value);
                            }
                            else
                            {
                                param2 = Convert.ChangeType(param2Value, type);
                            }
                        }
                    }
                }

                catch (Exception e)
                {
                    Debug.LogWarning($"EZConditions: Crashed getting param2. Param2Type: {param2Type}; Value: {param2Value}. Let EZ Know");
                    Debug.LogError(e);
                }

                return param2;
            }
            private set => param2 = value;
        }

        /// <summary>
        /// If this condition is a valid condition. i.e. There wouldn't be any problems using it
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (Obj != null && Function != null && !string.IsNullOrEmpty(MethodName));

            }
        }
        #endregion


        public void Reconstruct()
        {
            this.Function = null;
            this.param2 = null;
        }

        public Condition(System.Object obj, MethodInfo function, ConditionComparator comparator, System.Object param2, bool OR, bool param2Evaluation)
        {

            if (function.ReturnType == typeof(void))
            {
                Debug.LogWarning($"{function.Name} returns void. The condition will always be false");
            }


            this.Obj = (UnityEngine.Object)obj;
            this.Comparator = comparator;
            this.param2 = param2;
            this.OR = OR;
            this.param2Evaluation = param2Evaluation;

            // Save the name of the function so we can rebuild the method
            // at any point
            MethodName = function.Name;

            if (param2 != null)
            {
                Param2Type = param2.GetType().AssemblyQualifiedName;
                Param2Value = param2.ToString();
                param2AsUnityObject = param2 as UnityEngine.Object;
            }
        }

        public bool EvaluateCondition()
        {
            if (param2Evaluation)
            {
                return EvaluateParam2(Obj, Function, Comparator, Param2);
            }
            else
            {
                return Evaluate(Obj, Function, Comparator, Param2);
            }
        }

        /// <summary>
        /// Defines the implicit cconversion to a bool. Returns the evaluated condition
        /// </summary>
        /// <param name="condition">Condition to evaluate</param>
        public static implicit operator bool(Condition condition)
        {
            return condition.EvaluateCondition();
        }


        /// <summary>
        /// Evaluates obj against param2 based off of function. Returns the comparison. Returns false if unable to compare
        /// </summary>
        /// <param name="obj">Object owning the function call. Must be thhe declaring type for function.
        /// It's also the subject of the condition</param>
        /// <param name="function">Condition predicate. The question being asked. Method with the [Condition] attribute</param>
        /// <param name="comparator">The comparison operator to check foro</param>
        /// <param name="param2">The other object to compare against</param>
        /// <returns>A valid comparison of true or false. False if unable to compare the two objects</returns>
        public bool Evaluate(System.Object obj, MethodInfo function, ConditionComparator comparator, System.Object param2)
        {
            // All functions should not take args
            System.Object[] argsList = new System.Object[0];
            System.Object ret = function.Invoke(obj, argsList); // Call the function

            // Can we compare the two objects against each other? (It's a loose definition here)
            if (ret is IComparable comparableRet && param2 is IComparable comparableParam2)
            {
                try
                {
                    return Compare(comparableRet.CompareTo(comparableParam2), comparator);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Condition - {function}: Cannot compare these two types with each other. Error is handled though. Let EZ know");
                    Debug.LogError($"Condition Function Returning false.\n" + ex);
                    return false;
                }
            }


            Debug.LogWarning($"Condition - {function}: Param1 and Param2 cannot be compared to each other. False");
            return false;
        }


        public bool EvaluateParam2(System.Object obj, MethodInfo function, ConditionComparator comparator, System.Object param2)
        {
            // Function should take param2. Need to expand this later probably
            System.Object[] argsList = new System.Object[1] { param2 };
            System.Object ret = function.Invoke(obj, argsList); // Call the function

            if (ret is bool bRet)
            {
                // Just returns the result from the function. This means all the work is left to
                // the function itself. May need revisitation to uncomment below but for now, the work is done
                // by the function.
                return bRet;

                /*
                // returnedValue = 0 if true. 1 if false.
                int retVal = (bRet) ? 0 : 1;

                if (comparator == ConditionComparator.Equal)
                {
                    // True means 0, false is otherwise... ik. Backwards
                    // In casee I forget why: Remember in architecture, True means (A - B) = 0. Meaning they are the same
                    // Compare() is a true boolean evaluation. So If checking equal, you want to check that there is no difference i.e.
                    // result is 0.

                    return Compare(retVal, comparator);
                }
                else if (comparator == ConditionComparator.NotEqual)
                {
                    // Because we are checking NotEqual, that means we are checking that there IS a difference. In other words,
                    // returnedValue should NOT be 0.

                    return Compare(retVal, comparator);
                }
                else
                {
                    Debug.LogWarning($"Only choose Equal or NotEqual for Bool returning functions. Returning false");
                    return false;
                }

                */
            }

            Debug.LogWarning($"{function} Does not return a boolean. ConditionFunction returning false");
            return false;
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

        private bool Compare(int comparison, ConditionComparator comparator)
        {
            switch (comparator)
            {
                case ConditionComparator.EqualTo:
                    return comparison == 0;
                case ConditionComparator.NotEqualTo:
                    return comparison != 0;
                case ConditionComparator.GreaterThan:
                    return comparison > 0;
                case ConditionComparator.GreaterThanOrEqual:
                    return comparison >= 0;
                case ConditionComparator.LessThan:
                    return comparison < 0;
                case ConditionComparator.LessThanOrEqual:
                    return comparison <= 0;
            }

            return false;
        }

        /// <summary>
        /// String representation of Condition
        /// </summary>
        /// <returns>String repr of condition</returns>
        public override string ToString()
        {
            string OrTxt = (OR) ? "OR" : "AND";
            string Param2Txt = (string.IsNullOrEmpty(param2Value)) ? "Null" : param2Value;

            return $"{Obj}.{MethodName} {ComparatorString(Comparator)} {Param2Txt} {OrTxt}";
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
