using EZConditions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class ConditionManagerWindow : EditorWindow
{
    public VisualTreeAsset visualTree;
    public static SerializedProperty ConditionManager;

    #region Visual Elements
    public DropdownField conditionField;
    public EnumField comparatorField;
    public EnumField enumCompare;
    public ObjectField param1Field;
    public IntegerField intCompare;
    public FloatField floatCompare;
    public TextField stringCompare;
    public Toggle boolCompare;
    public ObjectField param2Field;
    public Toggle ORField;
    public Button compareBtn;
    public Button createConditionBtn;

    private List<MemberInfo> methods = new List<MemberInfo>();
    private MemberInfo selectedMethod;
    private VisualElement selectedArgument;
    #endregion

    public static void ShowWindow(SerializedProperty conditionmanager)
    {
        ConditionManager = conditionmanager;
        ConditionManagerWindow wnd = GetWindow<ConditionManagerWindow>();
        wnd.titleContent = new GUIContent($"{ConditionManager.serializedObject.targetObject} - Condition Manager");
    }

    private void OnDisable()
    {
        if (ConditionManager != null)
        {
            ConditionManager.Dispose();
        }
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        if (visualTree == null)
        {
            visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.ez337.ezconditions/Editor/ConditionManagerWindow.uxml");
        }
        visualTree.CloneTree(root);

        ListView conditionsList = root.Q<ListView>("conditions-list");

        // Hide the window if we had the screen up for 
        if (ConditionManager != null)
        {
            //Debug.Log("We have a condition Manager: " + ConditionManager.boxedValue);
            conditionsList.Clear();
            conditionsList.BindProperty(ConditionManager.FindPropertyRelative("Conditions"));
        }
        else
        {
            Debug.Log("No Valid ConditionManager");
            this.Close();
        }

        param1Field = root.Q<ObjectField>("param1");
        conditionField = root.Q<DropdownField>("cond-func");
        comparatorField = root.Q<EnumField>("comparator");
        enumCompare = root.Q<EnumField>("enum-compare");
        intCompare = root.Q<IntegerField>("int-compare");
        floatCompare = root.Q<FloatField>("float-compare");
        stringCompare = root.Q<TextField>("string-compare");
        boolCompare = root.Q<Toggle>("bool-compare");
        param2Field = root.Q<ObjectField>("param2");
        ORField = root.Q<Toggle>("OR");

        if (Selection.activeGameObject == null)
        {
            param1Field.allowSceneObjects = false;
            param2Field.allowSceneObjects = false;
        } 

        compareBtn = root.Q<Button>("evaluateCondition");
        compareBtn.RegisterCallback<ClickEvent>(EvaluateCondition);

        createConditionBtn = root.Q<Button>("createCondition");
        createConditionBtn.RegisterCallback<ClickEvent>(CreateCondition);

        param1Field.RegisterValueChangedCallback(OnObjectChange);
        conditionField.RegisterValueChangedCallback(OnConditionFunctionChange);

        HideAllOptions();
        ShowElement(param1Field);

    }


    /// <summary>
    /// Gets all valid MethodInfo and PropertyInfos that are public, private, or protected
    /// AND have the [Condition] attribute on them
    /// </summary>
    /// <param name="component">The object we are querying</param>
    private void PopulateConditions(UnityEngine.Object component)
    {
        // Get only public methods and properties with ConditionAttribute
        List<MemberInfo> members = component.GetType()
            .GetMembers(BindingFlags.Instance | BindingFlags.Public)
            .Where(member => (member is MethodInfo || member is PropertyInfo) &&
                             member.GetCustomAttribute<ConditionAttribute>() != null)
            .ToList();

        // Put all valid methods into the methods list
        foreach (var member in members)
        {
            // Disabled to just keep property and methods together.
            /*
            MethodInfo methodInfo = member as MethodInfo;
            //Debug.Log("MethodInfo: " + methodInfo);
            if (member is PropertyInfo property)
            {
                // Properties are converted to methodInfo
                methodInfo = ((PropertyInfo)member).GetMethod;

                //Debug.Log("Property with ConditionAttribute: " + property.Name);
                //Debug.Log("Property as Method: " + methodInfo);
            }

            else if (member is MethodInfo method)
            {
                Debug.Log("Method with ConditionAttribute: " + method.Name);
            }
            */

            conditionField.choices.Add(component.GetType() + "/" + member.Name);
            methods.Add(member);
        }

    }

    /// <summary>
    /// Event called when the ObjectField changes value
    /// </summary>
    /// <param name="evt"></param>
    private void OnObjectChange(ChangeEvent<UnityEngine.Object> evt)
    {
        HideAllOptions();
        if (evt.newValue != null)
        {
            ShowElement(conditionField);
            // Clear Dropdown and Methods list
            ClearDropDown();

            // If it's a gameObject, get all components on it and get the methods/properties with Condtion Attribute
            if (evt.newValue.GetType() == typeof(GameObject))
            {
                //Debug.Log("We got a gameObject");
                // Cast to gameObject and get the components
                Component[] components = ((GameObject)param1Field.value).GetComponents<Component>();

                foreach (Component component in components)
                {
                    //Debug.Log(item);
                    PopulateConditions(component);
                }
            }
            else
            {
                PopulateConditions(evt.newValue);
            }

        }

    }

    /// <summary>
    /// Event called when we pick something in the dropdown
    /// </summary>
    /// <param name="evt"></param>
    private void OnConditionFunctionChange(ChangeEvent<string> evt)
    {
        //Debug.Log("Dropdown Value changed");
        //Debug.Log(evt.newValue

        selectedMethod = methods[conditionField.choices.IndexOf(evt.newValue)];
        //Debug.Log("Index is " + index);
        //Debug.Log("MethodInfo at index is " + methods[index]);
        conditionField.SetValueWithoutNotify(evt.newValue.Replace('/', '.'));

        SetUpConditionWindow(selectedMethod);
    }

    private void SetUpConditionWindow(MemberInfo method)
    {
        ConditionAttribute attr = method.GetCustomAttribute<ConditionAttribute>();
        // NOTE: Room to deal with Param1 as well. For now, param1 is neglected.
        // In future, param1 could potentially support condition checking for methods that do not
        // belong to the calling class... Idk if we should support that. But just in case.

        Type attrType = attr.Param2;

        HideAllOptions();
        ShowElement(conditionField);
        // Show the appropriate param2 field
        if (attrType != null)
        {
            ShowElement(comparatorField);

            if (attrType == typeof(int))
            {
                ShowElement(intCompare);
                selectedArgument = intCompare;
            }
            else if (attrType == typeof(float))
            {
                ShowElement(floatCompare);
                selectedArgument = floatCompare;
            }
            else if (attrType == typeof(string))
            {
                ShowElement(stringCompare);
                selectedArgument = stringCompare;
            }
            else if (attrType == typeof(bool))
            {
                ShowElement(boolCompare);
                selectedArgument = boolCompare;
            }
            else if (attrType.IsEnum)
            {
                ShowElement(enumCompare);
                selectedArgument = enumCompare;
                // Get the type of the enum
                System.Enum enumType = (System.Enum)attrType.GetEnumValues().GetValue(0);
                enumCompare.Init(enumType);
            }
            else
            {
                ShowElement(param2Field);
                HideElement(comparatorField);
                try
                {
                    param2Field.objectType = attrType;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Unable to properly accept type {attrType}. Verify or Let EZ Know");
                    Debug.LogError(ex);
                }
                selectedArgument = param2Field;
            }
        }

        ShowElement(ORField);
        ShowElement(compareBtn);
        ShowElement(createConditionBtn);
    }


    #region Utility
    /// <summary>
    /// Clears the dropdown value and the methods list and selected method
    /// </summary>
    private void ClearDropDown()
    {
        conditionField.choices.Clear();
        methods.Clear();
        selectedMethod = null;
        selectedArgument = null;
        conditionField.SetValueWithoutNotify("No Function");
    }

    private Tuple<System.Object, MethodInfo> PreProcess()
    {
        // Convert the selected method into a MethodInfo
        MethodInfo methodInfo;
        if (selectedMethod is PropertyInfo propertyInfo)
        {
            methodInfo = propertyInfo.GetMethod;
        }
        else
        {
            methodInfo = (MethodInfo)selectedMethod;
        }


        // This section converts the object into the appropriate type that can call this method
        System.Object callingObject = param1Field.value;
        if (callingObject is GameObject go)
        {
            Type targetType = methodInfo.DeclaringType;
            foreach (var item in go.GetComponents<Component>())
            {
                if (item.GetType() == targetType)
                {
                    callingObject = item;
                    break;
                }
            }
        }

        return new(callingObject, methodInfo);
    }

    private void CreateCondition(ClickEvent evt)
    {
        Tuple<System.Object, MethodInfo> values = PreProcess();
        Condition condition;

        if (selectedArgument is ObjectField)
        {
            // Evaluate param2 as an argument for the function
            condition = new(values.Item1, values.Item2, (ConditionComparator)comparatorField.value, GetElementValue(selectedArgument), ORField.value, true);
        }
        else
        {
            // Evaluate (Instance.MethodInfo() lt/gt/eq Param2)
            condition = new(values.Item1, values.Item2, (ConditionComparator)comparatorField.value, GetElementValue(selectedArgument), ORField.value, false);
        }

        //Debug.Log("Created Condition: " + condition);
        // Add the new Condition to the list
        if (ConditionManager != null)
        {
            SerializedProperty Conditions = ConditionManager.FindPropertyRelative("Conditions");
            Conditions.InsertArrayElementAtIndex(Conditions.arraySize);
            Conditions.GetArrayElementAtIndex(Conditions.arraySize - 1).boxedValue = condition;
            ConditionManager.serializedObject.ApplyModifiedProperties();
        }
    }

    private void EvaluateCondition(ClickEvent evt)
    {
        /*
        if (selectedMethod != null)
        {
            var val = PreProcess();

            if (selectedArgument is ObjectField objectField)
            {
                // Evaluate param2 as an argument for the function
                Debug.Log(Condition.EvaluateParam2(val.Item1, val.Item2, (ConditionComparator)comparatorField.value, objectField.value));
            }
            else
            {
                // Evaluate (Instance.MethodInfo() lt/gt/eq Param2)
                Debug.Log(Condition.Evaluate(val.Item1, val.Item2, (ConditionComparator)comparatorField.value, GetElementValue(selectedArgument)));
            }

        }
        */

        if (ConditionManager != null)
        {
            ((ConditionManager) (ConditionManager.boxedValue)).EvaluateConditions(true);
        }

    }

    private System.Object GetElementValue(VisualElement elm)
    {
        if (elm is IntegerField intField)
            return intField.value;
        else if (elm is FloatField floatField)
            return floatField.value;
        else if (elm is TextField textField)
            return textField.value;
        else if (elm is ObjectField objectField)
            return objectField.value;
        else if (elm is Toggle toggle)
            return toggle.value;
        else if (elm is EnumField enumField)
            return enumField.value;

        return null;
    }

    private void ShowElement(VisualElement elm)
    {
        elm.style.display = DisplayStyle.Flex;
    }

    private void HideElement(VisualElement elm)
    {
        elm.style.display = DisplayStyle.None;
    }

    private void ShowAllOptions()
    {
        conditionField.style.display = DisplayStyle.Flex;
        intCompare.style.display = DisplayStyle.Flex;
        floatCompare.style.display = DisplayStyle.Flex;
        stringCompare.style.display = DisplayStyle.Flex;
        param2Field.style.display = DisplayStyle.Flex;
        boolCompare.style.display = DisplayStyle.Flex;
        comparatorField.style.display = DisplayStyle.Flex;
        compareBtn.style.display = DisplayStyle.Flex;
        createConditionBtn.style.display = DisplayStyle.Flex;
        ORField.style.display = DisplayStyle.Flex;
        enumCompare.style.display = DisplayStyle.Flex;
    }

    private void HideAllOptions()
    {
        //obj.style.display = DisplayStyle.None;
        conditionField.style.display = DisplayStyle.None;
        intCompare.style.display = DisplayStyle.None;
        floatCompare.style.display = DisplayStyle.None;
        stringCompare.style.display = DisplayStyle.None;
        param2Field.style.display = DisplayStyle.None;
        param2Field.value = null;
        boolCompare.style.display = DisplayStyle.None;
        comparatorField.style.display = DisplayStyle.None;
        //compareBtn.style.display = DisplayStyle.None;
        createConditionBtn.style.display = DisplayStyle.None;
        ORField.style.display = DisplayStyle.None;
        enumCompare.style.display = DisplayStyle.None;
    }

    #endregion
}