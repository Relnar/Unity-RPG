using UnityEditor;
using UnityEngine;

namespace RPG.Utils
{
    [CustomPropertyDrawer(typeof(EnumNamedArrayAttribute))]
    public class DrawerEnumNamedArray : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Properly configure height for expanded contents.
            return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumNamedArrayAttribute enumNames = attribute as EnumNamedArrayAttribute;

            // propertyPath returns something like component_hp_max.Array.data[4]
            // so get the index from there
            int index = System.Convert.ToInt32(property.propertyPath.Substring(property.propertyPath.LastIndexOf("[")).Replace("[", "").Replace("]", ""));

            // Change the label
            label.text = enumNames.names[index];

            // Draw field
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
