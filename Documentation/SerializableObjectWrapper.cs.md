# SerializableObjectWrapper
A class to simplify Serialization of System.Object Types. While you can use this in your own setup, I do not recommend as I may have not made it generic enough to support your use case. In other words, I have not fully vetted it for use outside of EZConditions. You have been warned. It has some basic optimsations in place as well. Read the code for yourself if you want to consider using it for your own game. It *does* handle primitive types and automatically deals with UnityEngine.Object objects as well.

## Constructors
```cs
public SerializableObjectWrapper(object obj)
```

The Copy Constructor is not fully vetted. Use with caution 
```cs
public SerializableObjectWrapper(SerializableObjectWrapper original)
```

## Fields

### TypeName
The assembly qualified typename. Used to reconstruct the object as needed.
```cs
public string TypeName;
```

### JsonData
The JsonData for UnitySerialization if needed.
```cs
public string JsonData;
```

### CachedObject
The cachedObject if it has not been deconstructed already for quicker retrieval.
```cs
public System.Object cachedObject;
```

## Methods
### GetObject
Gets the wrapped Object. Returns the deserialized Object. Null if invalid or fail for whatever reason.
```cs
public System.Object GetObject()
```
