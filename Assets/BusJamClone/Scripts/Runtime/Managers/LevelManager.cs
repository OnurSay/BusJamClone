using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BusJamClone.Scripts.Runtime.Managers
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance;

        [Header("Flags")] 
        public bool isGamePlayable;
        public bool isLevelFailed;
        public bool isGainedThisLevelCoin = false;

        [Header("Parameters")]
        [SerializeField] private int levelIndex;
        [SerializeField] private int totalLevelCount;
        [SerializeField] private int totalPlayedLevelCount;

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
            levelIndex = PlayerPrefs.GetInt("CurrentLevel", 0);
            totalPlayedLevelCount = PlayerPrefs.GetInt("TotalPlayedLevel", 1);
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
            totalPlayedLevelCount++;
            if (levelIndex >= totalLevelCount)
            {
                levelIndex = 0;
            }

            PlayerPrefs.SetInt("CurrentLevel", levelIndex);
            PlayerPrefs.SetInt("TotalPlayedLevel", totalPlayedLevelCount);
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

        public void SetLevelTMP(TextMeshProUGUI levelTMP)
        {
            levelTMP.text = "Level " + totalPlayedLevelCount;
            UIManager.instance.OpenLevelText();
        }
    }
}