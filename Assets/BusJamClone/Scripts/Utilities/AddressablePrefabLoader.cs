using BusJamClone.Scripts.Data;
using BusJamClone.Scripts.Runtime.Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BusJamClone.Scripts.Utilities
{
    public class AddressablePrefabLoader : MonoBehaviour
    {
        [Header("Cached References")]
        [SerializeField] private GameplayManager gameplayManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private TimeManager timeManager;
        private GameObject loadedPrefabInstance;
        
        [Header("Variables")]
        public string levelGroupName = "LevelsGroup";
        [SerializeField] private int levelIndex;

        private void Start()
        {
            levelIndex = PlayerPrefs.GetInt("CurrentLevel", 0);
            var prefabAddress = $"Level_{levelIndex}";
            LoadPrefab(prefabAddress);
        }

        private void LoadPrefab(string prefabAddress)
        {
            Addressables.LoadAssetAsync<GameObject>(prefabAddress).Completed += OnPrefabLoaded;
        }

        private void OnPrefabLoaded(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedPrefabInstance = Instantiate(handle.Result);
                if (loadedPrefabInstance.TryGetComponent(out LevelContainer levelContainer))
                {
                    levelContainer.InitializeVariables(gameplayManager, gridManager, timeManager);
                }

                Debug.Log($"Loaded and instantiated prefab: {handle.Result.name}");
            }
            else
            {
                Debug.LogError($"Failed to load prefab from Addressables group: {levelGroupName}");
            }
        }

        private void OnDestroy()
        {
            if (loadedPrefabInstance)
            {
                Addressables.ReleaseInstance(loadedPrefabInstance);
            }
        }
    }
}