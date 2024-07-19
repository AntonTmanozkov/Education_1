using SOVa.CommonVariables;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SOVa.CustomVariables
{
    [CreateAssetMenu(menuName = "SOVa/Common variables/Int to float")]
    public class IntToFloat : Float
    {
        [SerializeField] Int _int;
        [SerializeField] int _offcet;

        public override float Value
        {
            get => _int.Value + _offcet;
            set => _int.Value = (int)value;
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(IntToFloat))]
        private class CEditor : Editor 
        {
            public override void OnInspectorGUI()
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
                GUI.enabled = true;

                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_int)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_offcet)));
                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
