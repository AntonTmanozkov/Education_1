#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityExtentions.Random;
using UnityEditor.UIElements;

namespace UnityExtentions.Editor.RandomTools
{
    [CustomPropertyDrawer(typeof(Random<>.ObjectInfo), true)]
    internal class CEditor : PropertyDrawer
    {
        private VisualElement _instance;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualTreeAsset asset = Resources.Load<VisualTreeAsset>("RandomViewElement");
            _instance = asset.Instantiate();

            _instance.Q<PropertyField>("object-field").BindProperty(property.FindPropertyRelative("Object"));
            _instance.Q<PropertyField>("wight").BindProperty(property.FindPropertyRelative("Weight"));
            _instance.Q<PropertyField>("wight-drop-offcet").BindProperty(property.FindPropertyRelative("WeightOffcetWhenDropped"));
            _instance.Q<PropertyField>("wight-drop-other-offcet").BindProperty(property.FindPropertyRelative("WeightOffcetWhenDroppedOther"));

            return _instance;
        }
    }
}
#endif