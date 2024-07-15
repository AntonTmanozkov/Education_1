using System;
using UnityEngine;

namespace MemorySaveSystem
{
    public class Save
    { 
        public Save(string key, object data)
        {
            Memory.saves.Remove(key);

            Type dataType = data.GetType();
            if (dataType.IsPrimitive || dataType == typeof(string) || dataType == typeof(decimal))
            {
                Memory.saves.Add(key, data.ToString());

                if ((int)MemorySettings.Instance.DebugModeValue > 1)
                    Debug.Log($"{Memory.MESSAGE_PREFFIX} Save with key {key} and data {data} was created");
            }
            else
            {
                string JsonData = JsonUtility.ToJson(data);
                Memory.saves.Add(key, JsonData);

                if ((int)MemorySettings.Instance.DebugModeValue > 1)
                    Debug.Log($"{Memory.MESSAGE_PREFFIX} Save with key {key} and data {JsonData} was created");
            }
        }
    }
}