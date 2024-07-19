using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SOVa
{
    [CreateAssetMenu(menuName = "SOVa/Custom variables/Int Array")]
    public class IntArray : Int
    {
        [SerializeField] private Int[] _array;

        public override int Value
        {
            get
            {
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
        [CustomEditor(typeof(IntArray))]
        private class IntArrayEditor : Editor
        {
            private IntArray _target;

            private void Awake()
            {
                _target = (IntArray)target;
            }

            public override void OnInspectorGUI()
            {
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