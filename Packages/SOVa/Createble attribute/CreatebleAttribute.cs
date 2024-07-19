using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

namespace SOVa
{
    public class CreatebleAttribute : PropertyAttribute
    {
        public Type FieldType = null;
        public string CreatedAssetName = null;
        public CreatebleAttribute() { }
        public CreatebleAttribute(Type fieldType) 
        {
            FieldType = fieldType;
        }

        public CreatebleAttribute(string createdAssetName) 
        {
            CreatedAssetName = createdAssetName;
        }

        public CreatebleAttribute(Type fieldType, string createdAssetName) 
        {
            FieldType = fieldType;
            CreatedAssetName = createdAssetName;
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(CreatebleAttribute))]
        public class CEditor : PropertyDrawer
        {
            private CreatebleAttribute _attribute;
            private Type _propertyType;
            private SerializedProperty _property;
            private Object _target;
            private const float BUTTON_SIZE = 20;
            private const float DROPDOWN_SIZE = 80;
            private string _propertyName;
            private int _dropdownselectedIndex;

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                _attribute = attribute as CreatebleAttribute;

                Type parentType = property.serializedObject.targetObject.GetType();
                FieldInfo fieldInfo;
                do
                {
                    fieldInfo = parentType.GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Instance);
                    parentType = parentType.BaseType;
                } while (parentType != typeof(object) && fieldInfo == null);

                if (fieldInfo != null)
                {
                    _propertyType = fieldInfo.FieldType;
                }

                if (fieldInfo == null && _attribute.FieldType != null)
                {
                    _propertyType = _attribute.FieldType;
                }
                else if (fieldInfo == null)
                {
                    Debug.LogError($"Can't find a {property.propertyPath}");
                    base.OnGUI(position, property, label);
                    return;
                }

                _target = property.serializedObject.targetObject;
                _property = property;
                _propertyName = label.text;


                if (property.serializedObject.targetObject is not ScriptableObject)
                {
                    DrawNotSOInspector(position, property, label);
                    return;
                }

                IEnumerable<Type> types = Assembly.GetAssembly(_propertyType).GetTypes().Where(x => x.IsSubclassOf(_propertyType));
                if (_propertyType.IsAbstract == false)
                {
                    types = types.Append(_propertyType);
                }
                if (property.propertyType != SerializedPropertyType.ObjectReference)
                {
                    Debug.Log($"{nameof(CreatebleAttribute)} work with scriptable objects only!");
                }
                else if (types.Count() != 0 && property.objectReferenceValue == null)
                {
                    DrawCreateAbstractSO(position, property, label, types);
                }
                else
                {
                    DrawCreateButton(position, property, label);
                }

            }

            private void DrawCreateAbstractSO(Rect position, SerializedProperty property, GUIContent label, IEnumerable<Type> types)
            {
                Rect fieldRect = new(
                position.x,
                position.y,
                position.width - DROPDOWN_SIZE,
                position.height
                   );
                EditorGUI.PropertyField(fieldRect, property, label, true);

                Rect buttonRect = new(
                position.x + fieldRect.width,
                    position.y,
                    DROPDOWN_SIZE,
                    position.height
                    );

                List<string> displayedOptions = new() { "none" };
                displayedOptions.AddRange(types.Select(x => x.Name));

                _dropdownselectedIndex = EditorGUI.Popup(buttonRect, _dropdownselectedIndex, displayedOptions.ToArray());
                if (_dropdownselectedIndex != 0)
                {
                    Create(types.ElementAt(_dropdownselectedIndex - 1));
                }
            }

            private void DrawCreateButton(Rect position, SerializedProperty property, GUIContent label)
            {
                Rect fieldRect = new(
                position.x,
                position.y,
                position.width - BUTTON_SIZE,
                position.height
                   );
                bool contains = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(property.objectReferenceValue)).Contains(property.objectReferenceValue);
                if (contains)
                {
                    GUI.enabled = false;
                }
                EditorGUI.PropertyField(fieldRect, property, label, true);
                GUI.enabled = true;

                Rect buttonRect = new(
                position.x + fieldRect.width,
                    position.y,
                    BUTTON_SIZE,
                    position.height
                    );

                if (property.objectReferenceValue == null)
                {
                    if (GUI.Button(buttonRect, "➕"))
                    {
                        Create(_propertyType);
                    }
                }
                else if (contains && GUI.Button(buttonRect, "➖"))
                {
                    Delete();
                }
            }

            private void DrawNotSOInspector(Rect position, SerializedProperty property, GUIContent label) 
            {
                Rect fieldRect = new(
                position.x,
                position.y,
                position.width - DROPDOWN_SIZE,
                position.height
                   );
                EditorGUI.PropertyField(fieldRect, property, label, true);

                Rect buttonRect = new(
                position.x + fieldRect.width,
                    position.y,
                    DROPDOWN_SIZE,
                    position.height
                    );

                EditorGUI.LabelField(buttonRect, "Not SO");
            }

            private void Create(Type type)
            {
                ScriptableObject instance = ScriptableObject.CreateInstance(type);
                AssetDatabase.AddObjectToAsset(instance, _target);
                string name = $"{_target.name}.{_propertyName}";
                if (_attribute.CreatedAssetName != null) 
                {
                    name = _attribute.CreatedAssetName;
                }
                instance.name = name;
                _property.objectReferenceValue = instance;
                _property.serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }

            private void Delete()
            {
                AssetDatabase.RemoveObjectFromAsset(_property.objectReferenceValue);
                Object.DestroyImmediate(_property.objectReferenceValue);
                _property.objectReferenceValue = null;
                _property.serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }
        }
#endif
    }
}