# Система сохранений Memory
Система Memory - простой способ создавать сохранения для игр. Данная система максимально приближена по принципу работы к [PlayerPrefs](https://docs.unity3d.com/ScriptReference/PlayerPrefs.html) от Unity.

## Как использовать?
1. [Класс Save](###Save)
2. [Класс Load](###Load)
3. [Класс Memory](###Memory)

----
### Save
Save - класс, предназначенный для отправки сохраняемых данных в общую систему сохранений. <br/>
Сигнатура класса
```C#
namespace MemorySaveSystem
{
    public class Save
    { 
        public Save(string key, object data);
    }
}
```

Класс состоит лишь из конструктора, который принимает 2 аргумента

| Аргумент | Описение |
|-----------------|---------------------------------------------|
| key | ключ, который будет использоваться в дальнейшем для получения передаваемых данных |
| data | данные, которые необходимо сохранить |

Для использования данного класса рекомендуется создавать отдельный класс, который будет хранить в себе все данные, которые необходимо сохранить.
Пример использования данного класса. Этот скрипт сохраняет позицию и угол разворота [GameObject](https://docs.unity3d.com/ScriptReference/GameObject.html), на котором он применён.

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

    // Класс, который содержит в себе все данные, необходимые для сохранения. 
    // Он обязательно должен иметь аттрибут "Serializeble (https://learn.microsoft.com/ru-ru/dotnet/api/system.serializableattribute?view=net-7.0)",
    // что бы JSONUtility смог его конвертировать.

    [Serializable]
    private class SaveData 
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }
}
```

Этот класс выполняет сохранение позиции в момент, когда будет нажат пунтк "Do save" контекстного меню, после чего данный класс будет записан системой Memory. 
Данный пример содержится во вкладке Sapmples, где можно посмотреть на его работу.
<br/> :heavy_exclamation_mark: **ВАЖНО** :heavy_exclamation_mark:
<br/> Сейчас он не будет записал в файл, и при выходе из игры эти данные будут утеряны. Для записи в файл стоит использовать [Memory](###Memory).WriteToFile().

----
### Load

Класс ```Load<T>``` используется для того, что бы изъять данные из сохранения.
<br> Сигнатура класса:
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

| Элемент класса | Описение |
|-----------------|---------------------------------------------|
| T | Тип, в который данные переведутся автоматически после загрузки |
| _saveName | Ключ, который использутся для получения данных |
| _data | Загружаемые данные. Если данных нет, будет значение Defualt |
| Data | Свойство, которое автоматически переводит полученные данные в тип Т |

Пример использования класса ```Load<T>```, где при его помощи достаются сохранённые позиция и разворот объекта.

```C#
public class TransformSaver : MonoBehaviour
{
    [SerializeField] private string _uniqueSaveKey = "SAVE_KEY";

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
```

:heavy_exclamation_mark: **ВАЖНО** :heavy_exclamation_mark:
<br/> Перед использованием класса Load Для этого стоит использовать [Memory](###Memory).ReadFromFile().
<br/> В противном случае при попытке загрузки будет возвращать defoalt значение типа T

----
### Memory
Memory - стачиский класс предоставляемый этой системой. Основной задачей этого класса является непосредственное проведение записи сохранения в файл, и чтение. <br/>
Сигнатура класса
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

| Элемент класса | Описение |
|-----------------|---------------------------------------------|
| saves | Поле, в котором хранятся все сохранения до записи в файл |
| MESSAGE_PREFIX | Префикс, который система Memory использует при отправке сообщений в консоль |
| Path | Свойство, которое вычисляет путь до сохранения |
| BeforeSaved | Событие, которое вызывается перед непосредственной записью данных в файл |
| Contains | Метод, проверяющий наличие сохранения по данному ключу key. |
| ReadFromFile | Метод, считывающая сохранение из файла, находящегося по указанному в свойстве Path пути. Если файл отсутствует, метод создаст пустой. |
| WriteToFile | Вызывает событие BeforeSaved, после чего начинает запись сохранения |
| ClearAll | Очищает все загруженные сохранения. Файл сохранения остаётся нетрунутым |
| DeleteFile | Удаляет файл сохранения, если он существует |

В мобильных играх, как правило, нет сложной системы загрузок и сохранений, поэтому, чаще всего для автоматизации загрузки и сохранения будет использоваться класс вот такого вида:

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

Данный класс можно найти в Samples. Он автоматически производит считывание файлов при запуске игры, и автоматически сохраняет при выходе из неё.