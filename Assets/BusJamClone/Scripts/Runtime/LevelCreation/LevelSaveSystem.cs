using System.IO;
using BusJamClone.Scripts.Data;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.LevelCreation
{
    public static class LevelSaveSystem
    {
        static readonly string levelDataPath = Application.dataPath + $"/Resources/LevelData/";
        static readonly string backupLevelDataPath = Application.dataPath + $"/Resources/BackupLevelData/";

        public static LevelData LoadLevel(int levelIndex)
        {
            if (!Directory.Exists(levelDataPath))
            {
                Debug.LogWarning("LevelData directory not found!");
                return null;
            }

            var filePath = levelDataPath + $"Level{levelIndex}.json";
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"Level{levelIndex}.json not found in the {filePath}");
                return null;
            }

            var json = Resources.Load<TextAsset>($"LevelData/Level{levelIndex}").text;
            return JsonConvert.DeserializeObject<LevelData>(json);
        }

        public static void SaveLevel(LevelData levelGrid, int levelIndex)
        {
            if (!Directory.Exists(levelDataPath))
            {
                Directory.CreateDirectory(levelDataPath);
            }

            var filePath = levelDataPath + $"Level{levelIndex}.json";
            if (File.Exists(filePath))
            {
                BackupLevel(levelIndex);
            }

            string json = JsonConvert.SerializeObject(levelGrid);
            File.WriteAllText(filePath, json);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        static void BackupLevel(int levelIndex)
        {
            if (!Directory.Exists(backupLevelDataPath))
            {
                Directory.CreateDirectory(backupLevelDataPath);
            }

            var backupLevel = Resources.Load<TextAsset>($"LevelData/Level{levelIndex}").text;
            var filePath = backupLevelDataPath + $"Level{levelIndex}.json";
            File.WriteAllText(filePath, backupLevel);
        }

        public static bool IsLevelExists(int levelIndex)
        {
            return Resources.Load<TextAsset>($"LevelData/Level{levelIndex}") != null;
        }
    }
}