using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZConditions
{
    [CustomPropertyDrawer(typeof(ConditionManager))]
    public class ConditionManagerPropertyDrawer : PropertyDrawer
    {
        public SerializedProperty Conditions;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Conditions = property.FindPropertyRelative("Conditions");
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label, true);
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 1;

            if (property.isExpanded)
            {
                float newPos = EditorGUI.GetPropertyHeight(property, true);
                Rect testBtn = new Rect(position.x + position.width/3, position.y + newPos, position.width / 3, EditorGUIUtility.singleLineHeight);
                Rect createBtn = testBtn; createBtn.y += EditorGUIUtility.singleLineHeight;

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
                // Add the heoght of the conditions property
                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Conditions"), true);
                height += EditorGUIUtility.singleLineHeight * 2; // Space for the button
            }

            return height;
        }
    }
}

