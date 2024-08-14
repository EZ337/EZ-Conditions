# ConditionManager.cs
Class to house a list of conditions and perform evaluation on the list as a set. <u>If the condition list is empty, the returned values is the default return (True by default) specified by you </u>. Evaluation of the ConditionManager returns true if and only if ALL the conditions in the list return true. [See the How To Page for more information](./Documentation.md#how-to).

## Constructors
```cs
public ConditionManager(bool defaultReturn = true)
```
Constructor for the ConditionManager. defaultReturn specifies the returned value when the conditionManager's List is empty.

## Fields
### DefaultReturn
```cs
public bool DefaultReturn = true;
```
The value returned when there are no conditions in the Conditions List.

### Conditions
```cs
public List<Condition> Conditions;
```
List of conditions for the ConditionManager. An Empty list causes `EvaluateConditions` to return DefaultReturn.

## Functions
### EvaluateConditions
```cs
public bool EvaluateConditions(bool debug = false)
```
Evaluates the set of conditions belonging to this manager. Set `debug` to true to print messages to the console about individual condition failure.

