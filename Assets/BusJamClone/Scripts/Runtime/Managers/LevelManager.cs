using System.Collections.Generic;
using UnityEngine;

//using UnityEngine.iOS;

namespace BusJamClone.Scripts.Runtime.Managers
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;
        public bool isGamePlayable;
        public bool isGameFinished = false;
        public bool isLevelCompleted = false;
        public bool isLevelFailed = false;
        public bool isGainedThisLevelCoin = false;
        public bool isLevelCompleteEventSent = false;
        public bool isPlayingTutorial;
        public bool isInMenu;

        public int totalPlayedLevel;
        public int totalPlayedLevelToDisplay;
        public List<GameObject> activePopUps = new List<GameObject>();
        public bool isAudio = true;
        public bool isVibration = true;

        // private UIManager _uIManager;
        // private ScenesManager _scenesManager;

        private void Awake()
        {
            if (Application.targetFrameRate != 60)
            {
                Application.targetFrameRate = 60;
            }
        
            MakeSingleton();

            totalPlayedLevel = PlayerPrefs.GetInt("totalPlayedLevel", 1);
            totalPlayedLevelToDisplay = PlayerPrefs.GetInt("totalPlayedLevelToDisplay", totalPlayedLevel);
            isAudio = PlayerPrefs.GetInt("isAudio", 1) == 1;
            isVibration = PlayerPrefs.GetInt("isVibration", 1) == 1;
        }

        private void MakeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        private void Start()
        {
            // _uIManager = UIManager.Instance;
            // _scenesManager = ScenesManager.Instance;
            AssignSettings();
            // _uIManager.AssignToggleSettings();
            // _uIManager.StartUpdates();
            LevelStarted();
        }

        private void LevelStarted()
        {
        }

        public void LevelComplete()
        {
            isLevelCompleteEventSent = true;
        
            IncreaseLevelIndex();
        }

        public void LevelFail()
        {
        
        }

        public void GetNextLevel()
        {
            // ScenesManager.Instance.LoadScene();
        }

        public void IncreaseLevelIndex()
        {
            totalPlayedLevel++;
            PlayerPrefs.SetInt("totalPlayedLevel", totalPlayedLevel);

            // bool isGroupCompleted = (ScenesManager.Instance.groupLevelIndex + 1) >=
            //                         ScenesManager.Instance.currentLevelGroup.levels.Count;
            // if (isGroupCompleted)
                // totalPlayedLevelToDisplay++;
            PlayerPrefs.SetInt("totalPlayedLevelToDisplay", totalPlayedLevelToDisplay);


            // ScenesManager.Instance.IncreaseLevelIndex();
        }

        public void PopUpOpened(GameObject popUp)
        {
            if (!activePopUps.Contains(popUp))
                activePopUps.Add(popUp);
            isInMenu = true;
        }

        public void PopUpClosed(GameObject popUp)
        {
            if (activePopUps.Contains(popUp))
                activePopUps.Remove(popUp);
            if (activePopUps.Count == 0)
                isInMenu = false;
        }

        public void AssignSettings()
        {
            isVibration = PlayerPrefs.GetInt("isVibration", 1) == 1;
            isAudio = PlayerPrefs.GetInt("isAudio", 1) == 1;
        }

        private void OnApplicationQuit()
        {
            if (isLevelCompleted)
            {
                if (!isGainedThisLevelCoin)
                {
                    // ScoreManager.Instance.UpdateCoin(ScoreManager.Instance.thisLevelReward, "level_complete");
                }

                if (!isLevelCompleteEventSent)
                    LevelComplete();
            }
        }

        public void FailDecreaseLevelIndex()
        {
            // int amount = ScenesManager.Instance.groupLevelIndex;
            // totalPlayedLevel -= amount;
            // ScenesManager.Instance.levelIndex -= amount;
            // ScenesManager.Instance.groupLevelIndex = 0;
            // PlayerPrefs.SetInt("LevelIndex", ScenesManager.Instance.levelIndex);
            // PlayerPrefs.SetInt("totalPlayedLevel", totalPlayedLevel);
            // PlayerPrefs.SetInt("groupLevelIndex", 0);
        }
    }
}