using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SOVa.CommonVariables
{
    [CreateAssetMenu(menuName = "Values/Float parametr")]
    public class FloatParametr : Float, IParametr<float>
    {
        [SerializeField] private ParametrBase<float> _parametr;

        public override float Value 
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

        public float this[int index]
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
            base.Load();
            _parametr.owner = this;
            _parametr.Level.Changed.AddListener(OnLevelChange);
        }

        private void OnLevelChange()
        {
            InvokeChanged();
        }
        
        #if UNITY_EDITOR
        [CustomEditor(typeof(FloatParametr))]
        private class FloatParametrEditor : Editor
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