using System;
using UnityEngine;

namespace UnityExtentions
{
    public interface INeedInit 
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoad()
        {
            try
            {
                ScriptableObjectList list = Resources.Load<ScriptableObjectList>(ScriptableObjectList.BEFORE_SCENE_LOAD);
                list.Load();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad()
        {
            try
            {
                ScriptableObjectList list = Resources.Load<ScriptableObjectList>(ScriptableObjectList.AFTER_SCENE_LOAD);
                list.Load();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected RuntimeInitializeLoadType[] When { get; }
        protected void Init(RuntimeInitializeLoadType time);
        internal void InitInternal(RuntimeInitializeLoadType time) => Init(time);

#if UNITY_EDITOR
        internal RuntimeInitializeLoadType[] WhenInternal => When;
#endif
    }
}