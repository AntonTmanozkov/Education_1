using SOVa.CommonVariables;
using UnityEditor;
using UnityEngine;

namespace SOVa
{
    [CreateAssetMenu(menuName = "SOVa/Custom variables/Vector3 Parametr")]
    public class Vector3Parametr : Vector3Value, IParametr
    {
        [SerializeField] private ParametrBase<Vector3> _parametr;

        public override bool HaveOldValue => false;
        public override bool HaveChangedEvent => false;
        public override Event Changed => _parametr.Level.Changed;

        public override Vector3 Value 
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

        public Vector3 this[int index]
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
                this[index] = (Vector3)value; 
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
            //Stack overflow
            //InvokeChanged();
        }
        
#if UNITY_EDITOR
        [CustomEditor(typeof(Vector3Parametr))]
        private class Vector3ParametrEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                var parametrProp = serializedObject.FindProperty("_parametr");
                var levelProp = parametrProp.FindPropertyRelative("Level");
                var parametrPerLevel = parametrProp.FindPropertyRelative("_parametrPerLevel");
                EditorGUILayout.PropertyField(levelProp);
                EditorGUILayout.PropertyField(parametrPerLevel);
                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}