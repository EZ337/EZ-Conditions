using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace EZConditions
{
    public class ConditionManagerWindow : EditorWindow
    {
        public VisualTreeAsset visualTree;
        public static SerializedProperty ConditionManager;

        #region Visual Elements
        public DropdownField conditionField;
        public EnumField comparatorField;
        public EnumField enumCompare;
        public EnumFlagsField enumFlagsCompare;
        public ObjectField param1Field;
        public IntegerField intCompare;
        public FloatField floatCompare;
        public TextField stringCompare;
        public Toggle boolCompare;
        public ObjectField param2Field;
        public Toggle ORField;
        public Button compareBtn;
        public Button createConditionBtn;
        public VisualElement paramContainer;

        private List<MemberInfo> methods = new List<MemberInfo>();
        private MemberInfo selectedMethod;
        private VisualElement selectedArgument;
        #endregion

        public static void ShowWindow(SerializedProperty conditionmanager)
        {
            ConditionManager = conditionmanager;
            // Check validity of conditionManager cause why not
            ((ConditionManager)ConditionManager.boxedValue).OnValidate();
            ConditionManagerWindow wnd = GetWindow<ConditionManagerWindow>();
            wnd.titleContent = new GUIContent($"{ConditionManager.serializedObject.targetObject} - Condition Manager");
        }

        private void OnDisable()
        {
            ConditionManager?.Dispose();
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
                //Debug.Log("No Valid ConditionManager");
                this.Close();
            }

            param1Field = root.Q<ObjectField>("param1");
            conditionField = root.Q<DropdownField>("cond-func");
            comparatorField = root.Q<EnumField>("comparator");
            enumCompare = root.Q<EnumField>("enum-compare");
            enumFlagsCompare = root.Q<EnumFlagsField>("enumFlags-compare");
            intCompare = root.Q<IntegerField>("int-compare");
            floatCompare = root.Q<FloatField>("float-compare");
            stringCompare = root.Q<TextField>("string-compare");
            boolCompare = root.Q<Toggle>("bool-compare");
            param2Field = root.Q<ObjectField>("param2");
            ORField = root.Q<Toggle>("OR");
            paramContainer = rootVisualElement.Q<VisualElement>("param-fields");

            // Disable Scene objects if the item is an asset
            try
            {
                if (AssetDatabase.Contains(ConditionManagerWindow.ConditionManager.serializedObject.targetObject))
                {
                    param1Field.allowSceneObjects = false;
                    param2Field.allowSceneObjects = false;
                }
            }
            catch
            {
                this.Close();
            }


            if (AssetDatabase.Contains(ConditionManagerWindow.ConditionManager.serializedObject.targetObject))
            {
                param1Field.allowSceneObjects = false;
                param2Field.allowSceneObjects = false;
            }

            compareBtn = root.Q<Button>("evaluateCondition");
            compareBtn.RegisterCallback<ClickEvent>(TestConditionManager);

            createConditionBtn = root.Q<Button>("createCondition");
            createConditionBtn.RegisterCallback<ClickEvent>(CreateCondition);

            param1Field.RegisterValueChangedCallback(OnObjectChange);
            conditionField.RegisterValueChangedCallback(OnConditionFunctionChange);

            // Plops the ConditionUtility in the field
            param1Field.value = AssetDatabase.LoadAssetAtPath<MonoScript>("Packages/com.ez337.ezconditions/Runtime/ConditionUtility.cs");

            HideAllOptions();
            ShowElement(param1Field);

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
        /// Gets all valid MethodInfo and PropertyInfos that are public
        /// AND have the [Condition] attribute on them
        /// </summary>
        /// <param name="component">The object we are querying</param>
        private void PopulateConditions(UnityEngine.Object component)
        {
            List<MemberInfo> members = new();
            Type objectType;

            // If it's a script file, we only get public, static ConditionFunctions
            if (component is MonoScript mono)
            {
                objectType = mono.GetClass();

                members = objectType.GetMembers(BindingFlags.Public | BindingFlags.Static)
                    .Where(member => member.GetCustomAttribute<ConditionAttribute>() != null).ToList();
            }
            else
            {
                objectType = component.GetType();

                // Get only public methods and properties with ConditionAttribute
                members = (objectType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                    .Where(member => member.GetCustomAttribute<ConditionAttribute>() != null)).ToList();
            }





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

                conditionField.choices.Add(objectType + "/" + member.Name);
                methods.Add(member);
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

            PresentParameters(selectedMethod);
            ShowElement(paramContainer);
        }

        private void PresentParameters(MemberInfo method)
        {
            paramContainer.Clear();
            MethodInfo methodInfo = (method is PropertyInfo property) ? property.GetGetMethod(true) : (MethodInfo)method;

            // Show a parameter field for each parameter on this method
            foreach (ParameterInfo parameter in methodInfo.GetParameters())
            {
                Type type = parameter.ParameterType;
                VisualElement parameterField;
                if (type == typeof(int))
                {
                    parameterField = new IntegerField($"{parameter.Name} (int)");
                }
                else if (type == typeof(float))
                {
                    parameterField = new FloatField($"{parameter.Name} (float)");
                }
                else if (type == typeof(string))
                {
                    parameterField = new TextField($"{parameter.Name} (string)");
                }
                else if (type == typeof(bool))
                {
                    parameterField = new Toggle($"{parameter.Name} (bool)");
                }
                else if (type.IsEnum)
                {
                    System.Enum enumType = (System.Enum)type.GetEnumValues().GetValue(0);

                    // Enum Flags
                    if (type.GetCustomAttribute<FlagsAttribute>(true) != null)
                    {
                        parameterField = new EnumFlagsField($"{parameter.Name}", enumType);
                    }
                    else // Regular Enum
                    {
                        parameterField = new EnumField($"{parameter.Name}", enumType);
                    }
                }
                else
                {
                    parameterField = new ObjectField($"{parameter.Name}");
                    try
                    {
                        ((ObjectField)parameterField).objectType = type;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("Unable to properly accept type {type}");
                        Debug.LogError(ex);
                    }
                }

                paramContainer.Add(parameterField);
            }

            // 
            SetUpConditionWindow(methodInfo);
        }

        /// <summary>
        /// Final creation process. Presents comparison field and create condition button
        /// </summary>
        /// <param name="method"></param>
        private void SetUpConditionWindow(MethodInfo method)
        {

            Type returnType = method.ReturnType;

            HideAllOptions();
            ShowElement(conditionField);
            // Show the appropriate field to compare to based on the method's return type
            if (returnType != typeof(void) && typeof(IComparable).IsAssignableFrom(returnType))
            {
                ShowElement(comparatorField);

                if (returnType == typeof(int))
                {
                    ShowElement(intCompare);
                    selectedArgument = intCompare;
                }
                else if (returnType == typeof(float))
                {
                    ShowElement(floatCompare);
                    selectedArgument = floatCompare;
                }
                else if (returnType == typeof(string))
                {
                    ShowElement(stringCompare);
                    selectedArgument = stringCompare;
                }
                else if (returnType == typeof(bool))
                {
                    ShowElement(boolCompare);
                    selectedArgument = boolCompare;
                }
                else if (returnType.IsEnum)
                {
                    System.Enum enumType = (System.Enum)returnType.GetEnumValues().GetValue(0);

                    // Enum Flags
                    if (returnType.GetCustomAttribute<FlagsAttribute>(true) != null)
                    {
                        ShowElement(enumFlagsCompare);
                        selectedArgument = enumFlagsCompare;
                        enumFlagsCompare.Init(enumType);
                    }
                    else // Regular Enum
                    {
                        ShowElement(enumCompare);
                        selectedArgument = enumCompare;
                        enumCompare.Init(enumType);
                    }
                }
                else
                {
                    ShowElement(param2Field);
                    HideElement(comparatorField);
                    try
                    {
                        param2Field.objectType = returnType;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Unable to properly accept type {returnType}. Verify or Let EZ Know");
                        Debug.LogError(ex);
                    }
                    selectedArgument = param2Field;
                }

                ShowElement(ORField);
                ShowElement(compareBtn);
                ShowElement(createConditionBtn);

            }
            else
            {
                Debug.LogWarning($"{returnType} does not implement IComparable. Condition Function's return type must Implement IComparable.");
            }
        }

        private void CreateCondition(ClickEvent evt)
        {
            Tuple<System.Object, MethodInfo> values = PreProcess();
            Condition condition;

            List<SerializableObjectWrapper> serializedParameters = new List<SerializableObjectWrapper>();
            foreach (VisualElement parameter in paramContainer.Children())
            {
                serializedParameters.Add(new SerializableObjectWrapper(GetElementValue(parameter)));
            }

            condition = new(values.Item1, values.Item2, serializedParameters.ToArray(), (ConditionComparator)comparatorField.value,
                GetElementValue(selectedArgument), ORField.value);

            /*
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
            */

            //Debug.Log("Created Condition: " + condition);
            // Add the new Condition to the list
            if (condition.IsValid)
            {
                if (ConditionManager != null)
                {
                    // Extra check on the validity of this Manager. Cause why not
                    ((ConditionManager)ConditionManager.boxedValue).OnValidate();
                    SerializedProperty Conditions = ConditionManager.FindPropertyRelative("Conditions");
                    Conditions.InsertArrayElementAtIndex(Conditions.arraySize);
                    Conditions.GetArrayElementAtIndex(Conditions.arraySize - 1).boxedValue = condition;
                    ConditionManager.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                Debug.Log("Created Condition was invalid");
            }

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
            else if (callingObject is MonoScript ms)
            {
                callingObject = ms.GetClass();
            }

            return new(callingObject, methodInfo);
        }

        private void TestConditionManager(ClickEvent evt)
        {

            if (ConditionManager != null)
            {
                ConditionManager conditionManager = (ConditionManager)(ConditionManager.boxedValue);
                conditionManager.OnValidate();
                conditionManager.EvaluateConditions(true);
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
            else if (elm is EnumFlagsField enumFlagsField)
                return enumFlagsField.value;

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
            enumFlagsCompare.style.display = DisplayStyle.Flex;
            paramContainer.style.display = DisplayStyle.Flex;
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
            enumFlagsCompare.style.display = DisplayStyle.None;
            paramContainer.style.display = DisplayStyle.None;
        }
        #endregion
    }
}
