using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityExtentions.Random;

namespace UnityExtentions.Editor.RandomTools
{
    [CustomPropertyDrawer(typeof(PercentageItem<>))]
    public class PercentageItemDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var percentageProp = property.FindPropertyRelative("_percentage");

            var valueProp = property.FindPropertyRelative("_value");


            EditorGUI.BeginProperty(position, label, property);

            var propRect = new Rect(position.x, position.y, 30, position.height);
            var percentageRect = new Rect(position.x, position.y + 30, 30, position.height);

            EditorGUI.PropertyField(propRect, valueProp);
            EditorGUI.PropertyField(percentageRect, percentageProp);
            
            EditorGUI.EndProperty();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var percentageField = new PropertyField(property.FindPropertyRelative("_percentage"));

            var valueProp = property.FindPropertyRelative("_value");

            var valueField = new PropertyField(valueProp);

            container.Add(valueField);
            container.Add(percentageField);
            return container;
        }
    }
}