using SOVa.CommonVariables;
using UnityEditor;
using UnityEngine;

namespace SOVa
{
    [CreateAssetMenu(menuName = "SOVa/Custom variables/Linear Float")]
    public class LinearFloatParametr : Float
    {
        [SerializeField] private Int _level;
        [SerializeField] private float _multiply;
    
        public override bool HaveOldValue => false;
        public override bool HaveChangedEvent => false;
        public override Event Changed => _level.Changed;
    
        public override float Value
        {
            get
            {
#if UNITY_EDITOR
                if (_level == null)
                {
                    Debug.LogError($"Level is null {name}", this);
                    return 0;
                }
#endif
                var result = _multiply * _level;
                return result;
            } 
            set => Debug.LogError($"Cannot set value to {name}!", this);
        }
            
#if UNITY_EDITOR
        [CustomEditor(typeof(LinearFloatParametr))]
        private class LinearFloatParametrEditor : Editor
        {
            private SOVa.LinearFloatParametr _target;
    
            private void Awake()
            {
                _target = (SOVa.LinearFloatParametr)target;
            }
    
            public override void OnInspectorGUI()
            {
                var levelProp = serializedObject.FindProperty("_level");
                var multiplyProp = serializedObject.FindProperty("_multiply");
    
                EditorGUILayout.PropertyField(levelProp);
                EditorGUILayout.PropertyField(multiplyProp);
    
                var level = (Int)levelProp.objectReferenceValue;
                    
                DisplayResult(level, multiplyProp);
    
                serializedObject.ApplyModifiedProperties();
            }
    
            private void DisplayResult(Int level, SerializedProperty multiplyProp)
            {
                if (level == null)
                {
                    return;
                }
                EditorGUILayout.LabelField($"Result = {_target.Value}");
            }
        }
#endif
    }
}