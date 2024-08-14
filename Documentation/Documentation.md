- [Scripting References](#scripting-references)
- [How To](#how-to)
  - [Condition Attribute](#condition-attribute)
  - [Creating A Condition](#creating-a-condition)
  - [Visual Tutorial (EZSampleScript1 and 2 included in package)](#visual-tutorial-ezsamplescript1-and-2-included-in-package)
      - [Property Condition](#property-condition)
  - [Limitations](#limitations)
  - [Examples](#examples)
    - [Example Operations. "+" Means OR "\*" means "AND"](#example-operations--means-or--means-and)
    - [Concrete Examples. A = TRUE, B = FALSE, C = FALSE, D = TRUE, E = FALSE](#concrete-examples-a--true-b--false-c--false-d--true-e--false)

# Scripting References
- [ConditionUtility.cs](./ConditionUtility.cs.md)

# How To
Conditions are evaluated with `OR` Superiority. Meaning if I have a condition `A OR B AND C`, the condition is evaluated as `(A OR B) AND C`. In otherwords, the order of operation is such that immediately an OR is encountered, it grabs the next immediate Condition to evaluate until it hits an AND. An OR Condition with nothing after it is treated as an AND. Meaning if it's False, the total operation is false. See Examples below. **<u>The order in which you have your conditions MATTER.</u>** One false means the whole ConditionManager is false. `A OR B AND C` is **VERY** different from `B AND A OR C` or any variation.

***

There are 2 main steps to creating and using a condition. The first part is setting up the condition itself. Regarding this package, this is the only scripting required. It is simply marking a method or property with the **`[Condition]` attribute**. The second part is attaching conditions to a script using `ConditionManager` ~~or to a gameObject using `ConditionManagerComponent`~~. The term "condition function" will refer to a function marked with the condition attribute. **NOTE: <u>An empty ConditionManager evaluates to true (as of version 1.0.0-beta.4, You can specify the default return)</u>**

There are samples in the package for your reference as of v1.0.0-beta2.


## Condition Attribute
Attributes, for reference, are the metadata in square brackets above a class, method, field, etc in a script. Such as `[SerializeField]` or `[Tooltip]`. The Condition attribute is only valid on **Methods and Properties**. Marking a Property or a Method as a condition lets the ConditionManager know that this function can serve also as a condition. It enables the ConditionManager to seamlessly integrate into systems you likely would've already created. It is also important that the return type of the function in question implements the `IComparable` interface so that they can be easily compared.

## Creating A Condition
The `[Condition]` Attribute is your first step to creating a condition. As stated above, it is important that the return type of the condition implement the `IComparable` interface provided by C#. Once that is done, the next step is to add a `ConditionManager` field to a given object. Once added, you can create a condition with the `Create Condition` button in the inspector. This will open a window for you to create your condition (As of now, Creating Conditions with the "+" button is unsupported. You can remove functions with the "-" button however). Simply drag a gameObject or script into the object field to begin. If there are any valid condition functions on the game object, they will appear for you to select from. **Note that asset objects <u>should only</u> accept other asset objects.** That's it. it is **HIGHLY** recommended that you test conditions as I put additional checks when you run that function in the editor. Again, **<u>An empty ConditionManager evaluates to true</u>** If the object is a MonoScript, only static members are valid.

## Visual Tutorial (EZSampleScript1 and 2 included in package)
I have created a simple class called `ArbitraryClass` which has a `ConditionManager` field on it just as you would in your own class and attached it to a gameObject. When I refer to ArbitraryClass, substitute with the actual class you are working with.
![image](https://github.com/EZ337/EZ-Conditions/assets/88570645/ef1ed9b2-2680-4c28-8f5a-17dca26dd617)

The examples below are all from the script `EZSampleScript1` which is located in `Packages\EZ-Conditions\Documentation\EZConditionsSample1.cs` in the inspector. You can look at it yourself In this example, EZSampleScript1 and EZSampleScript2 are attached to a gameObject called "Player". ScriptableObjects and MonoScripts themselves are supported as well. 

#### Property Condition
1. Create a property just like you would do. (I added the SerializeField attribute so that it can be edited in the inspector. It is NOT needed for this to function)
 ```cs
        [field: SerializeField]
        [Condition]
        public string Name {get; set;} // Return type is string. Therefore we will be comparing against a string
```
2. Select the `Create Condition` button and then drag the object with the script in which you have a condition (In this case, the Player gameObject is being dragged into the ObjectField because it is the object I am trying to condition)
<img width="875" alt="image" src="https://github.com/EZ337/EZ-Conditions/assets/88570645/3412db51-7872-486e-88fe-ebffd413d2f3">

3. As you can see, this script has more than one condition function on it and it has been grouped appropriately.
4. Select the desired Condition Function. Checks are in place if the object is not an `IComparable` type to prevent you from progressing. Assuming you have a valid return type, you will be greeted with an appropriate comparison enum and the object to compare to. In this case, a string:\
![image](https://github.com/EZ337/EZ-Conditions/assets/88570645/4a39552f-e519-4a0d-a4bf-a52b1e43aebe)
5. Pressing Create Condition will create your new condition and add it to the condition manager. (It is recommended that you press test Conditions after you add/remove a condition).\
![image](https://github.com/EZ337/EZ-Conditions/assets/88570645/470a07b8-9394-4ffd-9011-2b1092dd5899)

6. That's it. Now all you need to do is run it when you want with the `EvaluateConditions` method. In this example, I run the condition by pressing the spacebar key. For reference, the Player's name is "Steve"
```cs
    // ArbitraryClass.cs
    private void Update()
    {
        // Evaluate the condition set when I press space. The boolean parameter signifies I want debug logs
        if (Input.GetKeyDown(KeyCode.Space))
        {
            conditionManager.EvaluateConditions(true);
        }
    }
```

7. We get a false return just as we should expect from comparing "Steve" to "John Doe"\
<img width="411" alt="image" src="https://github.com/EZ337/EZ-Conditions/assets/88570645/cf4776d5-a404-46c1-b19f-eb2c65b932db">\
8. Alternatively, adding an or condition and rearranging the order, evaluates to true:\
![image](https://github.com/EZ337/EZ-Conditions/assets/88570645/f9c7c993-4062-4e50-96d5-c5d749b9c3af)



## Limitations
- **DO NOT** Add conditions with the "+" button. The "-" button is useful for removing conditions. The "+" button should not be utilised to create a function. (I hope to enable this feature)
- Once a condition is created, it is read only. You have to remove it from the condition manager and create a new one if you want it to be different. 
- Because of the above, it is ill-advised to create conditions outside of the condition manager. Although you’re not restricted from doing so. Make sure to validate with `Condition.IsValid` property to check the validity of whatever condition you make before utilising it.

## Examples

### Example Operations. "+" Means OR "*" means "AND"
- A = A OR = A AND
- A*B = A AND B
- A+B = A OR B
- A * B + C = A AND (B + C)

### Concrete Examples. A = TRUE, B = FALSE, C = FALSE, D = TRUE, E = FALSE
- A = TRUE
- B = FALSE
- A AND B
  - TRUE AND FALSE
    - FALSE
- A OR B 
  - TRUE OR FALSE 
    - TRUE
- A OR B OR C 
  - TRUE OR FALSE OR FALSE
    - TRUE
- A OR B AND C
  - TRUE OR FALSE AND FALSE
    - (TRUE OR FALSE) AND FALSE
      - TRUE AND FALSE
        - FALSE
- B AND A OR C
  - FALSE AND TRUE OR FALSE
    - FALSE AND (TRUE OR FALSE)
      - FALSE AND TRUE
        - FALSE
- B AND C AND A
  - FALSE AND FALSE AND TRUE
    - FALSE
- B OR C OR E OR A
  - FALSE OR FALSE OR FALSE OR TRUE
    - TRUE
