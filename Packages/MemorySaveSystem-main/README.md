# ������� ���������� Memory
������� Memory - ������� ������ ��������� ���������� ��� ���. ������ ������� ����������� ���������� �� �������� ������ � [PlayerPrefs](https://docs.unity3d.com/ScriptReference/PlayerPrefs.html) �� Unity.

## ��� ������������?
1. [����� Save](###Save)
2. [����� Load](###Load)
3. [����� Memory](###Memory)

----
### Save
Save - �����, ��������������� ��� �������� ����������� ������ � ����� ������� ����������. <br/>
��������� ������
```C#
namespace MemorySaveSystem
{
    public class Save
    { 
        public Save(string key, object data);
    }
}
```

����� ������� ���� �� ������������, ������� ��������� 2 ���������

| �������� | �������� |
|-----------------|---------------------------------------------|
| key | ����, ������� ����� �������������� � ���������� ��� ��������� ������������ ������ |
| data | ������, ������� ���������� ��������� |

��� ������������� ������� ������ ������������� ��������� ��������� �����, ������� ����� ������� � ���� ��� ������, ������� ���������� ���������.
������ ������������� ������� ������. ���� ������ ��������� ������� � ���� ��������� [GameObject](https://docs.unity3d.com/ScriptReference/GameObject.html), �� ������� �� �������.

```C#
using MemorySaveSystem;
using System;
using UnityEngine;

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

    // �����, ������� �������� � ���� ��� ������, ����������� ��� ����������. 
    // �� ����������� ������ ����� �������� "Serializeble (https://learn.microsoft.com/ru-ru/dotnet/api/system.serializableattribute?view=net-7.0)",
    // ��� �� JSONUtility ���� ��� ��������������.

    [Serializable]
    private class SaveData 
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }
}
```

���� ����� ��������� ���������� ������� � ������, ����� ����� ����� ����� "Do save" ������������ ����, ����� ���� ������ ����� ����� ������� �������� Memory. 
������ ������ ���������� �� ������� Sapmples, ��� ����� ���������� �� ��� ������.
<br/> :heavy_exclamation_mark: **�����** :heavy_exclamation_mark:
<br/> ������ �� �� ����� ������� � ����, � ��� ������ �� ���� ��� ������ ����� �������. ��� ������ � ���� ����� ������������ [Memory](###Memory).WriteToFile().

----
### Load

����� ```Load<T>``` ������������ ��� ����, ��� �� ������ ������ �� ����������.
<br> ��������� ������:
```C#
namespace MemorySaveSystem
{
	public class Load<T>
	{
		private string _saveName;
		private T _data;

		public Load(string saveName);

        public T Data => _data;

        public T GetData();
        public static implicit operator T(Load<T> load);
	}
}
```

| ������� ������ | �������� |
|-----------------|---------------------------------------------|
| T | ���, � ������� ������ ����������� ������������� ����� �������� |
| _saveName | ����, ������� ����������� ��� ��������� ������ |
| _data | ����������� ������. ���� ������ ���, ����� �������� Defualt |
| Data | ��������, ������� ������������� ��������� ���������� ������ � ��� � |

������ ������������� ������ ```Load<T>```, ��� ��� ��� ������ ��������� ���������� ������� � �������� �������.

```C#
public class TransformSaver : MonoBehaviour
{
    [SerializeField] private string _uniqueSaveKey = "SAVE_KEY";

    [ContextMenu("Do load")]
    public void Load() 
    {
        SaveData data = new Load<SaveData>(_uniqueSaveKey);

        // ���� ���������� �����������, Load ����� �������� defualt. ��� ������� ��� Null
        // ������� ����� �������� ����� ��������, �� null.
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
```

:heavy_exclamation_mark: **�����** :heavy_exclamation_mark:
<br/> ����� �������������� ������ Load ��� ����� ����� ������������ [Memory](###Memory).ReadFromFile().
<br/> � ��������� ������ ��� ������� �������� ����� ���������� defoalt �������� ���� T

----
### Memory
Memory - ��������� ����� ��������������� ���� ��������. �������� ������� ����� ������ �������� ���������������� ���������� ������ ���������� � ����, � ������. <br/>
��������� ������
```C#
    public class Memory
    {
        internal static UnityDictionarity<string, string> saves = new();
        internal const string MESSAGE_PREFFIX;
        public static string Path;

        public static event Action BeforeSaved;
        public static bool Contains(string key);
        public static void ReadFromFile();
        public static void WriteToFile();
        public static void ClearAll();
        public static void DeleteFile();
    }
```

| ������� ������ | �������� |
|-----------------|---------------------------------------------|
| saves | ����, � ������� �������� ��� ���������� �� ������ � ���� |
| MESSAGE_PREFIX | �������, ������� ������� Memory ���������� ��� �������� ��������� � ������� |
| Path | ��������, ������� ��������� ���� �� ���������� |
| BeforeSaved | �������, ������� ���������� ����� ���������������� ������� ������ � ���� |
| Contains | �����, ����������� ������� ���������� �� ������� ����� key. |
| ReadFromFile | �����, ����������� ���������� �� �����, ������������ �� ���������� � �������� Path ����. ���� ���� �����������, ����� ������� ������. |
| WriteToFile | �������� ������� BeforeSaved, ����� ���� �������� ������ ���������� |
| ClearAll | ������� ��� ����������� ����������. ���� ���������� ������� ���������� |
| DeleteFile | ������� ���� ����������, ���� �� ���������� |

� ��������� �����, ��� �������, ��� ������� ������� �������� � ����������, �������, ���� ����� ��� ������������� �������� � ���������� ����� �������������� ����� ��� ������ ����:

```C#
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

```

������ ����� ����� ����� � Samples. �� ������������� ���������� ���������� ������ ��� ������� ����, � ������������� ��������� ��� ������ �� ��.