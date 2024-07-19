using System;
using SOVa;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System.Diagnostics.Eventing.Reader;

namespace SOVA.Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(VariableBase<,>), true)]
    [CanEditMultipleObjects]
    public class ReferencebleValueBaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            GUI.enabled = true;
            EditorGUILayout.Space(3);
            bool isOldValue = serializedObject.FindProperty("IsOldValue").boolValue;
            if (isOldValue)
            {
                DrawInspectorForOldValue();
                return;
            }

            DrawInspector();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInspectorForOldValue() 
        {
            EditorGUILayout.LabelField("This is old value. Read only");
            GUI.enabled = false;
            DrawInspector();
            GUI.enabled = true;
        }

        private void DrawInspector() 
        {
            if (Application.isPlaying)
            {
                RuntimeValueDraw();
            }
            else
            {
                SerializedProperty property;
                property = serializedObject.FindProperty("_value");
                EditorGUILayout.PropertyField(property);
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_logChanges"));

            SerializedProperty selfSave = serializedObject.FindProperty("_selfSave");
            EditorGUILayout.PropertyField(selfSave);

            if (selfSave.boolValue)
            {
                SerializedProperty saveKey = serializedObject.FindProperty("_saveKey");
                if (string.IsNullOrEmpty(saveKey.stringValue))
                {
                    saveKey.stringValue = target.name.Replace(' ', '_').ToUpper();
                }
                EditorGUILayout.PropertyField(saveKey);
            }

            SerializedProperty clampValue = serializedObject.FindProperty("_clamp");
            EditorGUILayout.PropertyField(clampValue);
            if (clampValue.boolValue && serializedObject.FindProperty("_max").objectReferenceValue != null && serializedObject.FindProperty("_min").objectReferenceValue != null)
            {
                SerializedObject serializedClampBorder;

                Type targetType = target.GetType();
                PropertyInfo propertyInfo = targetType.GetProperty("Max");
                if ((propertyInfo.GetGetMethod().Attributes & MethodAttributes.Virtual) == 0 || (propertyInfo.GetGetMethod().Attributes & MethodAttributes.NewSlot) != 0)
                {
                    serializedClampBorder = new(serializedObject.FindProperty("_max").objectReferenceValue);
                    GUI.enabled = false;
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(serializedObject.FindProperty("_max").objectReferenceValue, typeof(Variable), false);
                    GUI.enabled = true;
                    EditorGUILayout.PropertyField(serializedClampBorder.FindProperty("_value"), new GUIContent(""));
                    GUI.enabled = false;
                    GUILayout.EndHorizontal();
                    serializedClampBorder.ApplyModifiedProperties();
                }

                propertyInfo = targetType.GetProperty("Min");
                if ((propertyInfo.GetGetMethod().Attributes & MethodAttributes.Virtual) == 0 || (propertyInfo.GetGetMethod().Attributes & MethodAttributes.NewSlot) != 0)
                {
                    serializedClampBorder = new(serializedObject.FindProperty("_min").objectReferenceValue);
                    GUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(serializedObject.FindProperty("_min").objectReferenceValue, typeof(Variable), false);
                    GUI.enabled = true;
                    EditorGUILayout.PropertyField(serializedClampBorder.FindProperty("_value"), new GUIContent(""));
                    GUILayout.EndHorizontal();
                    serializedClampBorder.ApplyModifiedProperties();
                }
            }
        }

        private void RuntimeValueDraw()
        {
            Type targetType = target.GetType();
                while (targetType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.Name == "_runtimeValue").Count() == 0 || targetType == typeof(object))
                {
                    targetType = targetType.BaseType;
                }
                FieldInfo field = targetType.GetField("_runtimeValue", BindingFlags.Instance | BindingFlags.NonPublic);
                object fieldValue = field.GetValue(target);
                object oldValue = fieldValue;
                if (fieldValue is int)
                {
                    fieldValue = EditorGUILayout.IntField("Runtime value", (int)fieldValue);
                }
                else if (fieldValue is float)
                {
                    fieldValue = EditorGUILayout.FloatField("Runtime value",(float)fieldValue);
                }
                else if (fieldValue is string)
                {
                    fieldValue = EditorGUILayout.TextField("Runtime value", (string)fieldValue);
                }
                else if (fieldValue is bool)
                {
                    fieldValue = EditorGUILayout.Toggle("Runtime value", (bool)fieldValue);
                }
                else
                {
                    GUILayout.Label($"Runtime value: {fieldValue}");
                }

                if (oldValue != fieldValue)
                {
                    targetType = target.GetType();
                    field.SetValue(target, fieldValue);

                    MethodInfo info = null;
                    do
                    {
                        info = targetType.GetMethod("OnValidate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        targetType = targetType.BaseType;

                    } while (info == null && targetType != typeof(object));

                    if (info != null)
                    {
                        info.Invoke(target, new object[0]);
                    }
                }
        }
    }
#endif
}
