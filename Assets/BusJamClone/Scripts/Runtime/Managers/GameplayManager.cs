using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BusJamClone.Scripts.Data.Config;
using BusJamClone.Scripts.Runtime.Models;
using DG.Tweening;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Managers
{
    [DefaultExecutionOrder(-1)]
    public class GameplayManager : MonoBehaviour
    {
        public static GameplayManager instance;

        [Header("Cached References")] 
        [SerializeField] private BusScript currentGoalBus;
        [SerializeField] private List<BusScript> allBuses;
        [SerializeField] private List<BusScript> completedBuses;
        [SerializeField] private List<Stickman> stickmanThroughBus;
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private UIManager uiManager;

        [Header("Game Flags")] 
        [SerializeField] private bool isChangingGoal;
        [SerializeField] private bool isAudioOn;
        [SerializeField] private bool isVibrationOn;
        private bool initialSettingsSet = true;
        private bool isInitialBusArrived;

        [Header("Actions")] 
        public Action onBusChangeDone;
        public Action onGameLost;

        private void Awake()
        {
            InitializeSingleton();
            HandleGameConfig();
        }

        private void InitializeSingleton()
        {
            if (instance) return;
            instance = this;
        }

        private void HandleGameConfig()
        {
            isAudioOn = gameConfig.isAudioOn == 1;
            isVibrationOn = gameConfig.isVibrationOn == 1;
            uiManager.HandleSwitches(isAudioOn, isVibrationOn);
            initialSettingsSet = false;
        }

        public BusScript GetCurrentBus()
        {
            return currentGoalBus;
        }

        private void AssignFirstGoal()
        {
            currentGoalBus = allBuses[0];
        }

        public void MoveToNextGoal()
        {
            var completedBus = currentGoalBus;
            allBuses.Remove(currentGoalBus);
            completedBuses.Add(completedBus);
            completedBus.transform.DOLocalMoveX(completedBus.transform.localPosition.x - 20f, 1.5f)
                .OnComplete(() => { completedBus.gameObject.SetActive(false); });

            if (allBuses.Count <= 0)
            {
                isChangingGoal = false;
                WinGame();
                return;
            }

            HandleLevelBusMovements();
        }

        private void HandleLevelBusMovements()
        {
            StartCoroutine(BusMovements());
        }

        private IEnumerator BusMovements()
        {
            var isGoalChanceCalled = false;
            foreach (var bus in allBuses.ToList())
            {
                bus.transform.DOLocalMoveX(bus.transform.localPosition.x - 5.75f, 0.5f).SetEase(Ease.InSine).OnComplete(
                    () =>
                    {
                        if (!isInitialBusArrived)
                        {
                            isInitialBusArrived = true;
                            DOVirtual.DelayedCall(1f, () =>
                            {
                                if (AudioManager.instance)
                                {
                                    AudioManager.instance.PlayBusHorn();
                                }
                            });
                        }

                        if (isGoalChanceCalled) return;
                        currentGoalBus = allBuses[0];
                        onBusChangeDone?.Invoke();
                        isChangingGoal = false;
                        isGoalChanceCalled = true;
                    });
                yield return new WaitForSeconds(0.25f);
            }
        }

        public bool GetIsChangingGoal()
        {
            return isChangingGoal;
        }

        public void SetIsChangingGoal(bool flag)
        {
            isChangingGoal = flag;
        }

        public void SetBuses(List<BusScript> levelBuses)
        {
            allBuses = levelBuses;
            HandleLevelBusMovements();
            AssignFirstGoal();
        }

        public bool GetVibration()
        {
            return isVibrationOn;
        }

        public bool GetAudio()
        {
            return isAudioOn;
        }

        public void ToggleVibration()
        {
            if (initialSettingsSet) return;
            isVibrationOn = !isVibrationOn;
            SaveConfig();
        }

        public void ToggleAudio()
        {
            if (initialSettingsSet) return;
            isAudioOn = !isAudioOn;
            AudioListener.volume = isAudioOn ? 1 : 0;
            SaveConfig();
        }

        private void SaveConfig()
        {
            gameConfig.Save(isAudioOn ? 1 : 0, isVibrationOn ? 1 : 0);
        }

        private void WinGame()
        {
            LevelManager.instance.isGamePlayable = false;
            uiManager.LevelCompleteEvents();
            TimeManager.instance.PauseTimer();
            
            if (VibrationManager.instance)
            {
                VibrationManager.instance.Win();
            }
            
            if (AudioManager.instance)
            {
                AudioManager.instance.PlayLevelComplete();
            }

            DOVirtual.DelayedCall(3f, () =>
            {
                if (LevelManager.instance.isTestScene)
                {
                    EditorApplication.isPlaying = false;
                    EditorApplication.playModeStateChanged += GoToLevelCreator;
                    return;
                }
                LevelManager.instance.LevelIncrease();
            });
        }

        public void LoseGame(bool isTimeLose)
        {
            if (!LevelManager.instance.isGamePlayable || LevelManager.instance.isLevelFailed) return;
            LevelManager.instance.isGamePlayable = false;
            LevelManager.instance.isLevelFailed = true;

            onGameLost?.Invoke();
            TimeManager.instance.PauseTimer();
            
            if (LevelManager.instance.isTestScene)
            {
                EditorApplication.isPlaying = false;
                EditorApplication.playModeStateChanged += GoToLevelCreator;
                return;
            }
            
            DOVirtual.DelayedCall(1f, () =>
            {
                
                if (VibrationManager.instance)
                {
                    VibrationManager.instance.Fail();
                }
                
                if (AudioManager.instance)
                {
                    AudioManager.instance.PlayLevelFail();
                }

                if (isTimeLose)
                {
                    uiManager.SetTimeLost();
                }
                
                uiManager.OpenLoseScreen();
            });
        }

        public void AddStickmanThroughBus(Stickman stickman)
        {
            if (stickmanThroughBus.Contains(stickman)) return;
            stickmanThroughBus.Add(stickman);
        }

        public void RemoveStickmanThroughBus(Stickman stickman)
        {
            if (!stickmanThroughBus.Contains(stickman)) return;
            stickmanThroughBus.Remove(stickman);
        }

        private void GoToLevelCreator(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.EnteredEditMode)
            {
                EditorSceneManager.OpenScene("Assets/BusJamClone/Scenes/LevelCreator.unity");
            }
            
        }
    }
}