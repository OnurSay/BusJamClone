using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BusJamClone.Scripts.Config;
using BusJamClone.Scripts.Runtime.Models;
using DG.Tweening;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Managers
{
    [DefaultExecutionOrder(-1)]
    public class GameplayManager : MonoBehaviour
    {
        public static GameplayManager instance;
        
        [Header("Cached References")]
        [SerializeField] private Material secretMaterial;
        [SerializeField] private BusScript currentGoalBus;
        [SerializeField] private List<BusScript> allBuses;
        [SerializeField] private List<BusScript> completedBuses;
        [SerializeField] private List<Stickman> stickmanThroughBus;
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private UIManager uiManager;
        
        [Header("Game Flags")]
        [SerializeField] private bool isChangingGoal;
        [SerializeField] private bool isGridCheckOnProgress;
        [SerializeField] private bool isAudioOn;
        [SerializeField] private bool isVibrationOn;
        private bool initialSettingsSet = true;
        
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
                        if (isGoalChanceCalled) return;
                        currentGoalBus = allBuses[0];
                        onBusChangeDone?.Invoke();
                        isChangingGoal = false;
                        isGoalChanceCalled = true;
                    });
                yield return new WaitForSeconds(0.5f);
            }
        }

        public bool GetIsGridCheckOnProgress()
        {
            return isGridCheckOnProgress;
        }

        public List<BusScript> GetCompletedBuses()
        {
            return completedBuses;
        }

        public bool GetIsChangingGoal()
        {
            return isChangingGoal;
        }

        public void SetIsChangingGoal(bool flag)
        {
            isChangingGoal = flag;
        }

        public void AddBus(BusScript busScript)
        {
            allBuses.Add(busScript);
        }

        public void ResetLevelGoals()
        {
            allBuses.Clear();
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
            DOVirtual.DelayedCall(3f, () => { LevelManager.instance.LevelIncrease(); });
        }

        public void LoseGame()
        {
            if (!LevelManager.instance.isGamePlayable || LevelManager.instance.isLevelFailed) return;
            LevelManager.instance.isGamePlayable = false;
            LevelManager.instance.isLevelFailed = true;
            onGameLost?.Invoke();
            TimeManager.instance.PauseTimer();
            DOVirtual.DelayedCall(1f, () => { uiManager.OpenLoseScreen(); });
        }

        public void AddStickmanThroughBus(Stickman stickman)
        {
            if (stickmanThroughBus.Contains(stickman)) return;
            stickmanThroughBus.Add(stickman);
        }

        public List<Stickman> GetStickmanThroughBus()
        {
            return stickmanThroughBus;
        }

        public void RemoveStickmanThroughBus(Stickman stickman)
        {
            if (!stickmanThroughBus.Contains(stickman)) return;
            stickmanThroughBus.Remove(stickman);
        }
    }
}