using UnityEngine;
using UnityEngine.SceneManagement;

namespace MemorySaveSystem.Samples.FileWorker
{
    internal class FileWorker : MonoBehaviour
    {
        private const string OBJECT_NAME = "[Game saver]";


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Memory.ReadFromFile();

            GameObject newGameObject = new();
            newGameObject.name = OBJECT_NAME;
            newGameObject.AddComponent<FileWorker>();
            DontDestroyOnLoad(newGameObject);

            SceneManager.activeSceneChanged += OnSceneChange;
        }

        private static void OnSceneChange(Scene oldScene, Scene newScene) => OnSceneChange();
        private static void OnSceneChange()
        {
            Memory.WriteToFile();
        }

        private void OnApplicationPause(bool pause)
        {
            Memory.WriteToFile();
        }

        private void OnApplicationQuit()
        {
            Memory.WriteToFile();
        }
    }
}
