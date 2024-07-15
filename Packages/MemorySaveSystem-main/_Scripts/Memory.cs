using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

#if UNITY_CLOUD_SAVE

using Unity.Services.CloudSave;
using Unity.Services.Core;

#endif

using UnityEngine;

namespace MemorySaveSystem
{
    public class Memory
    {
        internal static MemoryFile<string, string> saves = new(DateTime.UtcNow.ToString(DATE_FORMAT));
        internal const string MESSAGE_PREFFIX = "<color=yellow>[MEMORY]</color>:";
        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss,fff";
        public static ILegacySaveConverter[] LegacySaveConverters = new ILegacySaveConverter[0];
        public static string Path => GetFilePath(DefaultFileName);
        private static string DefaultFileName => "save";
        private static string FileExtension => ".json";
        private static MemorySettings Settings => MemorySettings.Instance;
        private static string GetFilePath(string fileName) =>
            System.IO.Path.Combine(Application.persistentDataPath, $"{fileName}{FileExtension}");

        internal static List<Action> _beforeSaveActions = new();
        internal static List<Action> _loadedActions = new();
        public static string JSONSave => JsonUtility.ToJson(saves, true);


        /// <summary>
        /// Don't subscribe before scene load
        /// (The enabled objects on a scene that call Awake or OnEnable)
        /// </summary>
        public static event Action BeforeSaved
        {
            add => _beforeSaveActions.Add(value);
            remove => _beforeSaveActions.Remove(value);
        }

        public static event Action Loaded
        {
            add => _loadedActions.Add(value);
            remove => _loadedActions.Add(value);
        }

        public static bool Contains(string key)
        {
            return saves.Keys.Contains(key);
        }

        public static void ClearMemory()
        {
            saves.Clear();
        }

        public static void RemoveSave(string key)
        {
            saves.Remove(key);
        }

        public static async Task<ReadFileResult> ReadFromFile()
        {
            return await ReadFromFile(DefaultFileName);
        }

#pragma warning disable CS1998
        public static async Task<ReadFileResult> ReadFromFile(string fileName)
#pragma warning restore CS1998
        {
            string filePath = GetFilePath(fileName);

            ReadFileResult localResult = ReadLocalFile(fileName);

            if (MemorySettings.Instance.DebugModeValue >= MemorySettings.DebugMode.Defualt)
                Debug.Log($"{MESSAGE_PREFFIX} Read of {filePath} completed with result: {localResult.ReadResult}");

            List<ReadFileResult> results = new()
            {
                localResult
            };

#if UNITY_CLOUD_SAVE
            if (Settings.CloudSaveMode == MemorySettings.CloudMode.Unity)
            {
                ReadFileResult cloudResult = await ReadUnityCloudFile(fileName);
                results.Add(cloudResult);
            }
#endif

            DateTime lastTime = new();
            ReadFileResult result = localResult;
            foreach (ReadFileResult file in results)
            {
                if (MemorySettings.Instance.DebugModeValue >= MemorySettings.DebugMode.Defualt)
                {
                    Debug.Log($"{MESSAGE_PREFFIX} Save from {(file.Save == null ? "<invalid date>" : file.Save.SaveDate)} by {file.SaveSource} was loaded with result: {file.ReadResult}");
                }

                if (file.ReadResult == State.Success)
                {
                    DateTime loadedTime = DateTime.ParseExact(file.Save.SaveDate, DATE_FORMAT, CultureInfo.InvariantCulture);
                    if (loadedTime > lastTime)
                    {
                        lastTime = loadedTime;
                        saves = file.Save;
                        result = file;
                    }
                }
            }

            if (MemorySettings.Instance.DebugModeValue >= MemorySettings.DebugMode.Defualt)
                Debug.Log($"{MESSAGE_PREFFIX} Using save from {result.Save.SaveDate} by {result.SaveSource}");

            ValidateConverters(saves);

            foreach (Action action in _loadedActions.ToArray())
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            return result;
        }

        public static ReadFileResult ReadLocalFile(bool writeToMemory = false) => ReadLocalFile(DefaultFileName, writeToMemory);

        public static ReadFileResult ReadLocalFile(string fileName, bool writeToMemory = false)
        {
            string filePath = GetFilePath(fileName);

            MemoryFile<string, string> localSave;
            ReadFileResult localResult;

            if (File.Exists(filePath) == false)
            {
                localSave = new("2000-01-01 14:00:00,000");
                localResult = new()
                {
                    SaveSource = FileSource.Device,
                    ReadResult = State.Failed,
                    Save = localSave
                };
            }
            else
            {
                string JSONSave = File.ReadAllText(filePath);
                localSave = JsonUtility.FromJson<MemoryFile<string, string>>(JSONSave);
                if (string.IsNullOrEmpty(localSave.SaveDate))
                {
                    localSave = new(DateTime.Now.ToString(DATE_FORMAT), localSave);
                }
                localResult = new()
                {
                    SaveSource = FileSource.Device,
                    ReadResult = State.Success,
                    Save = localSave,
                };
            }


            if(writeToMemory)
            {
                saves = localSave;
            }

            return localResult;
        }

        private static void ValidateConverters(MemoryFile<string, string> file) 
        {

            foreach (ILegacySaveConverter converter in LegacySaveConverters)
            {
                foreach (string key in converter.Keys)
                {
                    if (converter.VersionsToConvert.Contains(file.SaveVersion) && file.Keys.Contains(key))
                    {
                        converter.Convert(file[key], key);
                    }
                }
            }
        }

#if UNITY_CLOUD_SAVE

        public static async Task<ReadFileResult> ReadUnityCloudFile(string fileName)
        {
            ReadFileResult cloudResult;

            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                if (MemorySettings.Instance.DebugModeValue >= MemorySettings.DebugMode.Defualt)
                    Debug.Log($"{MESSAGE_PREFFIX} Cannot get unity cloud save. Unity service was not initialized");

                cloudResult = new()
                {
                    SaveSource = FileSource.UnityServices,
                    ReadResult = State.Failed
                };
                return cloudResult;
            }

            try
            {
                byte[] bytes = await CloudSaveService.Instance.Files.Player.LoadBytesAsync(fileName);
                string JSONSave = Encoding.UTF8.GetString(bytes);
                MemoryFile<string, string> cloudSave = JsonUtility.FromJson<MemoryFile<string, string>>(JSONSave);
                ValidateConverters(cloudSave);

                cloudResult = new()
                {
                    SaveSource = FileSource.UnityServices,
                    ReadResult = State.Success,
                    Save = cloudSave
                };

                if (MemorySettings.Instance.DebugModeValue >= MemorySettings.DebugMode.Defualt)
                    Debug.Log($"{MESSAGE_PREFFIX} Read of cloud untiy save completed with result: {cloudResult.ReadResult}");

                foreach (Action action in _loadedActions)
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }

                return cloudResult;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to connect to unity cloud. Reson:\n{e}");
                cloudResult = new()
                {
                    SaveSource = FileSource.UnityServices,
                    ReadResult = State.Failed
                };
                return cloudResult;
            }
        }

        public static async Task<WriteFileResult> WriteFileToUnityCloud(string fileName, string JSON)
        {
            WriteFileResult cloudResult;

            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                cloudResult = new()
                {
                    SaveSource = FileSource.UnityServices,
                    WriteResult = State.Failed
                };

                return cloudResult;
            }

            byte[] file = Encoding.UTF8.GetBytes(JSON);
            await CloudSaveService.Instance.Files.Player.SaveAsync(fileName, file);

            cloudResult = new()
            {
                SaveSource = FileSource.UnityServices,
                WriteResult = State.Success
            };

            return cloudResult;
        }

#endif

        public static void WriteToFile()
        {
            WriteToFile(DefaultFileName);
        }

#pragma warning disable CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        public static async void WriteToFile(string fileName)
#pragma warning restore CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        {
            foreach (Action action in _beforeSaveActions)
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            WriteFileToLocal(fileName);

#if UNITY_CLOUD_SAVE
            if (Settings.CloudSaveMode == MemorySettings.CloudMode.Unity)
            {
                WriteFileResult cloudResult = await WriteFileToUnityCloud(fileName, JSONSave);

                if (Settings.DebugModeValue >= MemorySettings.DebugMode.Defualt)
                {
                    Debug.Log($"{MESSAGE_PREFFIX} Cloud save completed");
                }
            }
#endif
        }

        public static void WriteFileToLocal() => WriteToFile(DefaultFileName);

        public static void WriteFileToLocal(string fileName)
        {
            string filePath = GetFilePath(fileName);
            saves.SaveDate = DateTime.Now.ToString(DATE_FORMAT);
            saves.SaveVersion = Application.version;
            saves.ProductName = Application.productName;
            string JSONSave = JsonUtility.ToJson(saves, true);

            bool accessed = TryAccessFile(() => File.WriteAllText(filePath, JSONSave), filePath, "writing");

            if (accessed && MemorySettings.Instance.DebugModeValue >= MemorySettings.DebugMode.Defualt)
            {
                Debug.Log($"{MESSAGE_PREFFIX} Local save completed");
            }
        }

        public static void DeleteFile()
        {
            DeleteFile(DefaultFileName);
        }

        public static void DeleteFile(string fileName)
        {
            string filePath = GetFilePath(fileName);

            if (File.Exists(filePath) == false)
            {
                return;
            }

            bool deleted = TryAccessFile(() => File.Delete(filePath), filePath, "deleting");

            if (deleted && (int)MemorySettings.Instance.DebugModeValue > 0)
            {
                Debug.Log($"{MESSAGE_PREFFIX} Save file {fileName} was deleted.");
            }
        }

        private static bool TryAccessFile(Action method, string filePath, string description = "creating")
        {
            try
            {
                method();
            }
            catch (Exception ex)
            {
                Debug.LogError($"{MESSAGE_PREFFIX} Unknown error while {description} save file at {filePath}!\n {ex.Message}");
                return false;
            }

            return true;
        }
    }
}