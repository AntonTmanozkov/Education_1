using System;
using UnityEngine;

namespace MemorySaveSystem.Samples.SimpleFileWorker
{
    public class TransformSaver : MonoBehaviour
    {
        [SerializeField] private string _uniqueSaveKey = "SAVE_KEY";

        [ContextMenu("Do save")]
        public void Save()
        {
            SaveData data = new()
            {
                Position = transform.position,
                Rotation = transform.rotation,
            };

            new Save(_uniqueSaveKey, data);
        }

        [ContextMenu("Do load")]
        public void Load()
        {
            SaveData data = new Load<SaveData>(_uniqueSaveKey);

            // если сохранение отсутствует, Load вернёт значение defualt. Для классов это Null
            // поэтому после загрузки стоит проверка, на null.
            data ??= new();

            transform.position = data.Position;
            transform.rotation = data.Rotation;
        }

        [Serializable]
        private class SaveData
        {
            public Vector3 Position;
            public Quaternion Rotation;
        }
    }
}