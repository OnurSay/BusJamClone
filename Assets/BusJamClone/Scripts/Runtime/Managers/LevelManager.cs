using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BusJamClone.Scripts.Runtime.Managers
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance;

        [Header("Flags")] public bool isGamePlayable;
        public bool isLevelFailed = false;
        public bool isGainedThisLevelCoin = false;

        public int levelIndex;
        [SerializeField] private int totalLevelCount;

        private void Awake()
        {
            HandleFPS();

            MakeSingleton();

            FetchPlayerPrefs();
        }

        private void HandleFPS()
        {
            if (Application.targetFrameRate != 60)
            {
                Application.targetFrameRate = 60;
            }
        }

        private void MakeSingleton()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void FetchPlayerPrefs()
        {
            levelIndex = PlayerPrefs.GetInt("CurrentLevel", 1);
        }

        private void Start()
        {
            LevelStarted();
        }

        private void LevelStarted()
        {
        }

        public void LevelIncrease()
        {
            IncreaseLevelIndex();
            LoadLevel();
        }
        
        private void LoadLevel()
        {
            var asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            asyncOperation.allowSceneActivation = false;
            UIManager.instance.OpenTransition(() => { asyncOperation.allowSceneActivation = true; });
        }

        private void IncreaseLevelIndex()
        {
            levelIndex++;
            if (levelIndex >= totalLevelCount)
            {
                levelIndex = 0;
            }

            PlayerPrefs.SetInt("CurrentLevel", levelIndex);
        }

        public void RestartLevel()
        {
            LoadLevel();
        }

        public int GetLevelIndex()
        {
            return levelIndex;
        }

        public void SetTotalLevelCount(int levelCount)
        {
            totalLevelCount = levelCount;
        }
    }
}