using SOVa.CommonVariables;
using UnityEditor;
using UnityEngine;

namespace SOVa
{
    [CreateAssetMenu(menuName = "SOVa/Custom variables/Linear Vector3")]
    public class LinearVector3Parametr : Vector3Value
    {
        [SerializeField] private Int _level;
        [SerializeField] private Vector3 _multiply;
    
        public override Vector3 Value
        {
            get
            {
                var sign = _multiply.x > 0 ? 1 : -1;
                var result = new Vector3(Mathf.Pow(Mathf.Abs(_multiply.x), _level)
                                ,Mathf.Pow(Mathf.Abs(_multiply.y), _level)
                                ,Mathf.Pow(Mathf.Abs(_multiply.z), _level));
                return result * sign;
            }
            set => Debug.LogError($"Cannot set value to {name}!", this);
        }
            
#if UNITY_EDITOR
        [CustomEditor(typeof(LinearVector3Parametr))]
        private class LinearVector3ParametrEditor : Editor
        {
            private SOVa.LinearVector3Parametr _target;
    
            private void Awake()
            {
                _target = (SOVa.LinearVector3Parametr)target;
            }
    
            public override void OnInspectorGUI()
            {
                var levelProp = serializedObject.FindProperty("_level");
                var multiplyProp = serializedObject.FindProperty("_multiply");
    
                EditorGUILayout.PropertyField(levelProp);
                EditorGUILayout.PropertyField(multiplyProp);
    
                var level = (Int)levelProp.objectReferenceValue;
                    
                // DisplayResult(level);
    
                serializedObject.ApplyModifiedProperties();
            }
    
            private void DisplayResult(Int level)
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