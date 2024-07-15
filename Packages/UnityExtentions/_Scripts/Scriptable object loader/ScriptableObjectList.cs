using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using System;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using Unity.EditorCoroutines.Editor;
using UnityEditor.Callbacks;
#endif

namespace UnityExtentions
{
    public class ScriptableObjectList : ScriptableObject
    {
        public const string BEFORE_SCENE_LOAD = "Before scene load objects to load";
        public const string AFTER_SCENE_LOAD = "After scene load objects to load";

        [SerializeField] RuntimeInitializeLoadType _loadType;
        [SerializeField] UnityEvent _loadComplete;
        [SerializeField] ScriptableObject[] _objects;
        //[SerializeField] AssetLabelReference _label;
        //[SerializeField] string[] _ignoreWords = new[] {"Shared Data"};
//#if UNITY_EDITOR
//        // ToDo fix
//        [SerializeField] AddressableAssetGroup _assetGroup;
//#endif

        public void Load() 
        {
            foreach (INeedInit loadeble in _objects) 
            {
                loadeble.InitInternal(_loadType);
            }
            _loadComplete.Invoke();
        }

        //private void OnHandleComplete(ScriptableObject scriptableObject) 
        //{
        //    try 
        //    {
        //        INeedInit obj = (scriptableObject as INeedInit);
        //        DontDestroyOnLoad(scriptableObject);
        //        obj.InitInternal(_loadType);
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.LogError($"Cant load asset. {e}");
        //    }
        //}

#if UNITY_EDITOR
        public const string ASSETS_PATH_BEFORE_SCENE_LOAD = "Assets/Resources/Before scene load objects to load.asset";
        public const string ASSETS_PATH_AFTER_SCENE_LOAD = "Assets/Resources/After scene load objects to load.asset";

        private EditorCoroutine _initCheckCoroutine;
        private int progressID;

        [DidReloadScripts]
        private static void InitializeOnLoad() 
        {
            RuntimeInitializeLoadType[] types = new[] 
            {
                RuntimeInitializeLoadType.BeforeSceneLoad,
                RuntimeInitializeLoadType.AfterSceneLoad,
            };
            int index = 0;
            foreach (string path in new[] 
            { 
                ASSETS_PATH_BEFORE_SCENE_LOAD,
                ASSETS_PATH_AFTER_SCENE_LOAD}) 
            {
                LoadOn(path, types[index]);
                index++;
            }
        }

        private static void LoadOn(string Path, RuntimeInitializeLoadType type) 
        {
            if (AssetDatabase.AssetPathExists(Path))
            {
                ScriptableObjectList loadedList = AssetDatabase.LoadAssetAtPath(Path, typeof(ScriptableObjectList)) as ScriptableObjectList;
                if (loadedList == null){ return; }
                loadedList.OnValidate();
                return;
            }

            if (AssetDatabase.IsValidFolder("Assets/Resources") == false)
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
                AssetDatabase.SaveAssets();
            }

            ScriptableObjectList instance = ScriptableObject.CreateInstance(typeof(ScriptableObjectList)) as ScriptableObjectList;
            instance._loadType = type;
            SerializedObject serializedObject = new(instance);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.CreateAsset(instance, Path);
        }

        private void OnValidate()
        {
            UpdateList();        
        }

        private void UpdateList()
        {
            if (Application.isPlaying) 
            {
                return;
            }

            string[] assetsIds = AssetDatabase.FindAssets("t:ScriptableObject");
            LoadObjects(assetsIds);

            //if (_loadType != RuntimeInitializeLoadType.AfterSceneLoad) 
            //{

            //}
            //else 
            //{
            //    _objects = new ScriptableObject[0];
            //}

            //if (_initCheckCoroutine != null) 
            //{
            //    Progress.Finish(progressID, Progress.Status.Canceled);
            //    EditorCoroutineUtility.StopCoroutine(_initCheckCoroutine);
            //}
            //_initCheckCoroutine = EditorCoroutineUtility.StartCoroutine(LoadArresables(assetsIds), this);
        }

        //private IEnumerator LoadArresables(string[] assetsIds)
        //{
        //    progressID = Progress.Start($"Scriptable object checking", $"Load type: {_loadType}");
        //    AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        //    List<AssetReference> references = new();
        //    int index = 0;
        //    foreach (string assetID in assetsIds)
        //    {
        //        try
        //        {
        //            UnityEngine.Object loaded = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(assetID));
        //            if (_ignoreWords.Where(x => loaded.name.Contains(x)).Count() > 0)
        //            {
        //                continue;
        //            }
        //            AddressableAssetEntry entry = settings.FindAssetEntry(assetID, _assetGroup);
        //            if (loaded is INeedInit && (loaded as INeedInit).WhenInternal.Contains(_loadType))
        //            {
        //                entry = settings.CreateOrMoveEntry(assetID, _assetGroup);
        //                entry.SetLabel(_label.labelString, true);
        //                references.Add(new AssetReference(assetID));
        //            }
        //            else if (entry != null && entry.labels.Contains(_label.labelString))
        //            {
        //                entry.SetLabel(_label.labelString, false);
        //            }
        //            if (entry != null && entry.labels.Count() == 0)
        //            {
        //                settings.RemoveAssetEntry(assetID);
        //            }
        //            index++;
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.LogException(e);
        //        }
        //        Progress.Report(progressID, (float)index / assetsIds.Length);
        //        yield return null;
        //    }

        //    _objects = references.ToArray();
        //    SerializedObject serializedObject = new(this);
        //    serializedObject.ApplyModifiedProperties();
        //    Progress.Finish(progressID);
        //}

        private void LoadObjects(string[] assetsIds) 
        {
            ScriptableObject[] assets = assetsIds.
                Select(x => AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(x))).
                Where(x => x is INeedInit && (x as INeedInit).WhenInternal.Contains(_loadType)).
                ToArray();

            _objects = assets;
            EditorUtility.SetDirty(this);
        }

        [CustomEditor(typeof(ScriptableObjectList))]
        private class CEditor : Editor 
        {
            public ScriptableObjectList _target => target as ScriptableObjectList;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (GUILayout.Button("Update list")) 
                {
                    _target.UpdateList();
                }
            }
        }
#endif
    }
}