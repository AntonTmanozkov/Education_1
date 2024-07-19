using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Unity.EditorCoroutines.Editor;
#endif

namespace SOVa
{
    public abstract class Variable : ScriptableObject
    {
        [SerializeField] private bool _logChanges;
        [SerializeField, HideInInspector] bool _isInitialized = false;
        [SerializeField] internal bool IsOldValue;
        protected bool LogChanges => _logChanges;

        public abstract object ObjValue { get; set; }
        public abstract object OldObjValue { get; set; }
        internal abstract void SetValue(object value);
        public abstract Event Changed { get; }
        public virtual bool HaveOldValue => true;
        public virtual bool HaveChangedEvent => true;

#if UNITY_EDITOR
        private EditorCoroutine _createCoroutine;
#endif
        protected virtual void Awake()
        {
#if UNITY_EDITOR
            if (_isInitialized) { return; }

            _createCoroutine = EditorCoroutineUtility.StartCoroutine(CreateCoroutine(), this);
#endif
        }

        protected virtual void OnEnable()
        {
                
        }


        protected virtual void OnDisable()
        {

        }

#if UNITY_EDITOR
        [MenuItem("Assets/SOVa/Rename Children")]
        private static void RenameChildren()
        {
            Object selection = Selection.activeObject;
            if (selection is not Variable)
            {
                Debug.LogWarning("Selected object is not variable");
                return;
            }
            Variable variable = selection as Variable;
            variable.PerformRename_EDITOR();
        }
#endif

        protected virtual void PerformRename_EDITOR() { }

        protected virtual void OnDestroy()
        {
#if UNITY_EDITOR
            if (_createCoroutine != null)
            {
                EditorCoroutineUtility.StopCoroutine(_createCoroutine);
            }
#endif
        }
#if UNITY_EDITOR
        private IEnumerator CreateCoroutine()
        {
            while (AssetDatabase.Contains(GetInstanceID()) == false)
            {
                yield return null;
            }
            yield return null;
            AfterCreate_EDITOR();
            _isInitialized = true;
        }

        protected virtual void AfterCreate_EDITOR() { }
#endif
    }
}
