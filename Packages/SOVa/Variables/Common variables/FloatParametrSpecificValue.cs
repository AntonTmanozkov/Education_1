using SOVa;
using UnityEditor;
using UnityEngine;

namespace SOVa.CommonVariables
{
    public class FloatParametrSpecificValue : Float
    {
        [SerializeField] FloatParametr _parametr;
        [SerializeField] Reference<int> _level;

        public override float Value
        {
            get
            {
                return _parametr[_level.Value];
            }
            set => UnityEngine.Debug.LogError($"{this} value read only!", this);
        }

        protected override void Load()
        {
            base.Load();
            _parametr.Changed.AddListener(OnParametrChange);
            _level.Changed += OnParametrChange;
        }

        private void OnParametrChange()
        {
            InvokeChanged();
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(FloatParametrSpecificValue))]
        private class CEditor : Editor
        {
            private FloatParametrSpecificValue _target => target as FloatParametrSpecificValue;

            public override void OnInspectorGUI()
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_parametr)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_level)));
                if (_target._level.HaveReference && _target._parametr != null)
                    GUILayout.Label($"Current: {_target.Value}");
                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
