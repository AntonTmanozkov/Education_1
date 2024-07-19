using System;
using UnityEditor;
using UnityEngine;

namespace SOVa
{
    [Serializable]
    public class Reference<T2>
    {
        [SerializeField] Variable _reference;
        [SerializeField] T2 _constantValue;
        [SerializeField] ReferenceMode _mode;

        public static implicit operator T2(Reference<T2> reference) => reference.Value;

        public event Action Changed;
        public Variable Variable => _reference;
        public bool HaveReference => _reference != null;
        public bool IsConstant => _mode == ReferenceMode.Constant;

        public Reference(T2 defualtValue)
        {
            _mode = ReferenceMode.Constant;
            _constantValue = defualtValue;
        }

        public Reference() 
        {
            _mode = ReferenceMode.Reference;
        }
        
        public T2 Value 
        {
            get 
            {
                switch (_mode) 
                {
                    case (ReferenceMode.Reference):
                        return (T2)_reference.ObjValue;
                    case (ReferenceMode.Constant):
                        return _constantValue;
                    default:
                        throw new Exception("Impossible mode");
                }
            }
            set
            {
                switch (_mode)
                {
                    case (ReferenceMode.Reference):
                        _reference.ObjValue = value;
                        Changed?.Invoke();
                        break;
                    case (ReferenceMode.Constant):
                        _constantValue = value;
                        Changed?.Invoke();
                        break;
                    default:
                        throw new Exception("Impossible mode");
                }
            }
        }
        
        #if UNITY_EDITOR
        public void SetReference_EDITOR(Variable variable)
        {
            _reference = variable;
            _mode = ReferenceMode.Reference;
        }
        #endif
    }

    internal enum ReferenceMode
    {
        Reference,
        Constant
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Reference<>))]
    public class CEditor : PropertyDrawer
    {
        private const float DROPDOWN_SIZE = 80;

        private SerializedProperty _property;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty enumProperty = property.FindPropertyRelative("_mode");
            _property = property;
            Rect fieldRect = new(
                position.x,
                position.y,
                position.width - DROPDOWN_SIZE,
                position.height
                   );

            string propertyName = "_reference";
            if (enumProperty.intValue == 1)
            {
                propertyName = "_constantValue";
            }

            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(propertyName), label);
            Rect buttonRect = new(
            position.x + fieldRect.width,
                position.y,
                DROPDOWN_SIZE,
                position.height
                );
            EditorGUI.PropertyField(buttonRect, enumProperty, new GUIContent(""));
        }
    }
#endif
}
