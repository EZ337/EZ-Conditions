- [ConditionUtility.cs](#conditionutilitycs)
  - [Static Member Variables](#static-member-variables)
    - [VERSION](#version)
    - [CACHESIZE](#cachesize)
    - [LRUCache](#lrucache)
  - [Static **Condition** Functions](#static-condition-functions)
    - [GetRandomFloat](#getrandomfloat)
    - [GetRandomInt](#getrandomint)
    - [GetRandomPercent](#getrandompercent)


# ConditionUtility.cs
A static class with functions that may be useful in general developmet and other general support for EZConditions.

## Static Member Variables

### VERSION
The current version of EZConditions (1.0.0-beta.4)

### CACHESIZE
The size of the LRU Dictionary Cache size for methodInfos. <u>Defaults to 30</u>.

### LRUCache
A static cache of MethodInfos for easier lookup for optimisation implemented as
an LRU algorithm. Modify **CACHESIZE** to increase the size. <u>Defaults to 30</u>.

## Static **<u>Condition</u>** Functions
Public static condition functions that you can call in your program or use as a condition function without having to write your own

### GetRandomFloat
```cs
/// Returns a random float from [minInclusive, maxInclusive]
[Condition]
public static float GetRandomFloat(float minInclusive, float maxInclusive)
```

### GetRandomInt
```cs
/// Returns a random int from [minInclusive, maxExclusive)
[Condition]
public static int GetRandomInt(int minInclusive, int maxExclusive)
```


### GetRandomPercent
```cs
/// Returns a random value from 0.0 - 1.0
[Condition]
public static float GetRandomPercent()
```
