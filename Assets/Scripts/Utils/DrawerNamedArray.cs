using System;
using UnityEditor;
using UnityEngine;

namespace RPG.Utils
{
    [CustomPropertyDrawer(typeof(NamedArrayAttribute))]
    public class DrawerNamedArray : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Properly configure height for expanded contents.
            return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            NamedArrayAttribute name = attribute as NamedArrayAttribute;

            // Change the label
            string[] parts = label.text.Split(' ');
            if (parts.Length > 1 && Int32.TryParse(parts[1], out int j))
            {
                j++;
                label.text = name.name + " " + j;
            }

            // Draw field
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
