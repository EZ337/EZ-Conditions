using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EZConditions
{
    [CustomPropertyDrawer(typeof(ConditionManager))]
    public class ConditionManagerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (GUI.Button(position, "Press Me"))
            {
                Debug.Log(property.displayName);
                var X = property.serializedObject;
                ConditionManagerWindow.ShowWindow(property);
            }

            EditorGUI.EndProperty();
        }
    }
}

