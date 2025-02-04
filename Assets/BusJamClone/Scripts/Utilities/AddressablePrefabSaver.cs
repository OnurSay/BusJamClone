using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace BusJamClone.Scripts.Utilities
{
    public class AddressablePrefabSaver : MonoBehaviour
    {
        public string prefabBasePath = "Assets/Prefabs/Levels/";
        public string addressableGroupName = "MyLevelsGroup";

        public void SaveAndAssignPrefab(GameObject levelRoot, int levelIndex)
        {
            if (!Directory.Exists(prefabBasePath))
            {
                Directory.CreateDirectory(prefabBasePath);
                AssetDatabase.Refresh();
            }
            var prefabName = $"Level_{levelIndex}";
            var prefabPath = prefabBasePath + prefabName + ".prefab";

            var prefab = PrefabUtility.SaveAsPrefabAsset(levelRoot, prefabPath);
            Debug.Log($"Prefab saved at: {prefabPath}");

            AddToAddressables(prefab, prefabPath, addressableGroupName);
        }
        
        private void AddToAddressables(GameObject prefab, string prefabPath, string groupName)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (!settings)
            {
                Debug.LogError("Addressables settings not found. Please initialize Addressables.");
                return;
            }
            
            var group = settings.FindGroup(groupName);
            if (!group)
            {
                group = settings.CreateGroup(groupName, false, false, true, settings.DefaultGroup.Schemas);
            }

            var assetGUID = AssetDatabase.AssetPathToGUID(prefabPath);
            var entry = settings.FindAssetEntry(assetGUID);

            if (entry != null) return;
            entry = settings.CreateOrMoveEntry(assetGUID, group);
            entry.address = Path.GetFileNameWithoutExtension(prefabPath);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, entry, true);
            AssetDatabase.SaveAssets();
            Debug.Log($"Prefab assigned to Addressables group: {groupName}");
        }
    }
}
