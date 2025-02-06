using System;
using System.Linq;
using BusJamClone.Scripts.Data;
using BusJamClone.Scripts.Runtime.Managers;
using DG.Tweening;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace BusJamClone.Scripts.Utilities
{
    public class AddressablePrefabLoader : MonoBehaviour
    {
        [Header("Cached References")] 
        [SerializeField] private GameplayManager gameplayManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private TimeManager timeManager;
        private GameObject loadedPrefabInstance;

        [Header("Variables")] public string levelGroupName = "LevelsGroup";

#if UNITY_EDITOR
        [Header("Editor")] public Action<GameObject> callbackAction;
#endif

        private void Start()
        {
            var prefabAddress = $"Level_{LevelManager.instance.GetLevelIndex()}";
            LoadPrefab(prefabAddress);
            AssignLevelCount();
        }

        private void AssignLevelCount()
        {
            var count =
                AddressableAssetSettingsDefaultObject.Settings.groups.FirstOrDefault(g => g.Name == levelGroupName)!
                    .entries.Count;
            LevelManager.instance.SetTotalLevelCount(count);
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

                HandleTransitions();

                Debug.Log($"Loaded and instantiated prefab: {handle.Result.name}");
            }
            else
            {
                Debug.LogError($"Failed to load prefab from Addressables group: {levelGroupName}");
            }
        }
#if UNITY_EDITOR

        private void LoadPrefabEditor(string prefabAddress)
        {
            Addressables.LoadAssetAsync<GameObject>(prefabAddress).Completed += OnPrefabLoadedEditor;
        }

        private void OnPrefabLoadedEditor(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedPrefabInstance = Instantiate(handle.Result);
                callbackAction?.Invoke(loadedPrefabInstance);

                Debug.Log($"Loaded and instantiated prefab: {handle.Result.name}");
            }
            else
            {
                Debug.LogError($"Failed to load prefab from Addressables group: {levelGroupName}");
            }
        }

#endif

        private void HandleTransitions()
        {
            DOVirtual.DelayedCall(Random.Range(0f, 0.5f), () =>
            {
                UIManager.instance.OpenTransition(null);
                DOVirtual.DelayedCall(Random.Range(0.5f, 1f), () =>
                {
                    UIManager.instance.CloseLoadingScreen();
                    UIManager.instance.CloseTransition(() =>
                    {
                        TimeManager.instance.SetTimerTMP(UIManager.instance.GetTimerTMP());
                        LevelManager.instance.SetLevelTMP(UIManager.instance.GetLevelTMP());
                        UIManager.instance.EnableSettingsButton();
                    });
                });
            });
        }

        private void OnDestroy()
        {
            if (loadedPrefabInstance)
            {
                Addressables.ReleaseInstance(loadedPrefabInstance);
            }
        }

#if UNITY_EDITOR

        public GameObject ManualPrefabLoader(string prefabAddress, Action<GameObject> callback)
        {
            callbackAction = callback;
            LoadPrefabEditor(prefabAddress);
            return loadedPrefabInstance;
        }
#endif
    }
}