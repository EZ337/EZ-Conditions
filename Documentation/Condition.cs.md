# Condition.cs
A class representing a Condition. Stores the information about a Condition including the Object to run on, the Method to run and the parameters for the object. The returned value of the given method should return an IComparable. A condition implicitly casts to a bool. Meaning `if (condition) {}` automatically evaluates the condition and returns the value.


## Constructors
Constructs a Condition. **DO NOT WRAP obj as a SerializableObjectWrapper.** It is done for you automatically.
```cs
public Condition(System.Object obj, MethodInfo function, SerializableObjectWrapper[] args, ConditionComparator comparator, System.Object comparedValue, bool OR)
```


## Properties

### Obj
The Object that owns the method we will run. Wrapped as a SerializableObjejctWrapper
```cs
public System.Object Obj { get; private set; }
```

### ConditionComparator
The Comparison to be made. Either lt,gt,eq,le,ge, or neq
```cs
public ConditionComparator Comparator { get; private set; }
```

### OR
Whether we treat this condition as an OR or an AND condition. An OR Condition evaluates in superiority and evaluates as an AND if it is the last condition. [See How To](./Documentation.md#how-to).
```cs
public bool OR { get; private set; }
```
### MethodName
The name of the method. Used to serialize MethodInfo as MethodInfos are not serialized by Unity by default
```cs
public string MethodName { get; private set; }
```

### Function
Gets the function associated with `Obj`. If it is null, it tries to get the Function from the Cache. If that still failed, it tries to reconstruct the MethodInfo. Optimisation has been taken into account here and thus should perform relatively well. If you find yourself making a game that requires objects to unload frequently, I recommend increasing the LRUCache in ConditionUtility.cs
```cs
public MethodInfo Function { get; private set; }
```

### ComparedValue
The object we are actually comparing ourselves to such that we get a true or false.
```cs
public System.Object ComparedValue { get; private set; }
```

### IsValid
Validates the Condition. Use this to check if a Condition is actually usable. This is especially useful for Conditions you create OUTSIDE of the ConditionManager. Or Conditions you create at Runtime. Very useful property
```cs
public bool IsValid {get;}
```

### Parameters
The Array of parameters that the function takes. Wrapped in as a SerializableObjectWrapper. 
```cs
public SerializableObjectWrapper[] Parameters { get; set; }
```

## Functions

### Evaluate
Wrapper function to call the static `Condition.Evaluate(object, MethodInfo, ...)` function. Returns the result of the function after comparing it to ComparedValue
```cs
public bool Evaluate();
```

Static function that evaluates a Condition with the provided values
```cs
public static bool Evaluate(System.Object obj, MethodInfo function, SerializableObjectWrapper[] argsList, ConditionComparator comparator, System.Object comparedValue);
```
> obj - The Object to run the function on<br>
> function - The method/property being ran<br>
> argsList - The arguments for the function if any<br>
> comparator - The comparison type: gt,lt,le,ge,eq,neq<br>
> comparedValue - The value being compared<br>
> Returned Bool - True if the comparedValue with respects to the comparator is true. False otherwise
