using UnityEngine;
using System.Collections;
using System.Collections.Generic;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SOVa.CommonVariables
{
    [CreateAssetMenu(menuName = "Values/Int parametr")]
    public class IntParametr : Int, IParametr<int>, IEnumerable<int>
    {
        [SerializeField] private ParametrBase<int> _parametr;
        public override bool HaveOldValue => false;
        public override bool HaveChangedEvent => false;
        public override int Value 
        {
            get
            {
                return _parametr.Value;
            }
            set
            {
                _parametr.Value = value;
            }
        }
    
        public int this[int index]
        {
            get
            {
                return _parametr[index];
            }
            set
            {
                _parametr[index] = value;
            }
        }
        object IParametr.this[int index]
        {
            get 
            {
                return this[index]; 
            }
            set 
            {
                this[index] = (int)value; 
            }
        }
    
        public int CurrentIndex => _parametr.CurrentIndex;
    
        public override string ToString()
        {
            return _parametr.Value.ToString();
        }
    
        protected override void Load()
        {
            _parametr.owner = this;
            if (_parametr.Level == null)
            {
                return;
            }
            _parametr.Level.Changed.AddListener(OnLevelChange);
        }
    
        private void OnLevelChange()
        {
            InvokeChanged();
        }

        public IEnumerator<int> GetEnumerator()
        {
            return _parametr.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parametr.GetEnumerator();
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(IntParametr))]
        private class CEditor : Editor 
        {

            public override void OnInspectorGUI()
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
                GUI.enabled = true;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_parametr._parametrPerLevel"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_parametr.Level"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_parametr._indexOffset"));
                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}