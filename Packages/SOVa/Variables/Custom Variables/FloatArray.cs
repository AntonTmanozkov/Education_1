using System.Linq;
using SOVa.CommonVariables;
using UnityEditor;
using UnityEngine;

namespace SOVa
{
    [CreateAssetMenu(menuName = "SOVa/Custom variables/Float Array")]
    public class FloatArray : Float
    {
        [SerializeField] private Float[] _array;
    
        public override float Value
        {
            get
            {
                #if UNITY_EDITOR

                foreach (var el in _array)
                {
                    if (el == null)
                    {
                        Debug.LogError($"Float array elements is null! {name}", this);
                        return 0;
                    }
                }
                #endif
                return _array.Sum(i => i.Value);
            }
            set => Debug.LogError($"Cannot set value to the array {name}", this);
        }
    
        protected override void Load()
        {
            foreach (var item in _array)
            {
#if UNITY_EDITOR
                if (item == null)
                {
                    Debug.LogError($"You have null item in {name}", this);
                    continue;
                }
#endif
                item.Changed.AddListener(InvokeChanged);
            }
        }
#if UNITY_EDITOR
        [CustomEditor(typeof(FloatArray))]
        private class FloatArrayEditor : Editor
        {
            private FloatArray _target;
    
            private void Awake()
            {
                _target = (FloatArray)target;
            }
    
            public override void OnInspectorGUI()
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
                GUI.enabled = true;

                var arrayProp = serializedObject.FindProperty("_array");
                if (Application.IsPlaying(this))
                {
                    EditorGUILayout.LabelField("Cannot change Array during playmode!");
                }
                else
                {
                    EditorGUILayout.PropertyField(arrayProp);
                }
                EditorGUILayout.LabelField($"Array sum = {_target.Value}");
    
                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}