using UnityEditor;
using UnityEngine;

namespace EZConditions
{
    [CustomPropertyDrawer(typeof(ConditionManager))]
    public class ConditionManagerPropertyDrawer : PropertyDrawer
    {
        public SerializedProperty Conditions;
        public SerializedProperty DefaultReturn;

        private string[] defaultReturn = { "False", "True" };
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Conditions = property.FindPropertyRelative("Conditions");
            DefaultReturn = property.FindPropertyRelative("DefaultReturn");
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label, true);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 1;


            if (property.isExpanded)
            {
                float newPos = EditorGUI.GetPropertyHeight(property, true);
                Rect defaultRtnBtn = position; defaultRtnBtn.y += newPos;
                Rect testBtn = new Rect(position.x + position.width/3, position.y + newPos + EditorGUIUtility.singleLineHeight, position.width / 3, EditorGUIUtility.singleLineHeight);
                Rect createBtn = testBtn; createBtn.y += EditorGUIUtility.singleLineHeight;

                DefaultReturn.intValue = EditorGUILayout.Popup(new GUIContent("Default Return",
                    "The default value returned when there are no Conditions in the List"),
                    DefaultReturn.intValue, defaultReturn);

                if (GUI.Button(testBtn, "Test Conditions"))
                {
                    ((ConditionManager)property.boxedValue).EvaluateConditions(true);
                }

                if (GUI.Button(createBtn, "Create Condition"))
                {
                    ConditionManagerWindow.ShowWindow(property);
                }


            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label);

            if (property.isExpanded)
            {
                // Add the heoght of the conditions 
                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Conditions"), true);
                height += EditorGUIUtility.singleLineHeight * 5; // Space for the 2 buttons and the defaultReturn
            }

            return height;
        }
    }
}

