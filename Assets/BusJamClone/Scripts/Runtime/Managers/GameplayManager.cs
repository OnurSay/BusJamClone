using System;
using System.Collections;
using System.Collections.Generic;
using BusJamClone.Scripts.Runtime.Models;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using LevelGoal = BusJamClone.Scripts.Runtime.LevelCreation.LevelGoal;

namespace BusJamClone.Scripts.Runtime.Managers
{
    [DefaultExecutionOrder(-1)]
    public class GameplayManager : MonoBehaviour
    {
        public static GameplayManager instance;
        [SerializeField] private Material secretMaterial;
        [SerializeField] private bool isChangingGoal;
        [SerializeField] private bool isGridCheckOnProgress;
        [SerializeField] private BusScript currentGoalBus;
        [SerializeField] private List<BusScript> allBuses;
        [SerializeField] private List<BusScript> completedBuses;

        public Action onBusChangeDone;

        private void Awake()
        {
            InitializeSingleton();
        }

        private void InitializeSingleton()
        {
            if (instance) return;
            instance = this;
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
            completedBus.transform.DOLocalMoveX(completedBus.transform.localPosition.x - 20f, 2f)
                .OnComplete(() =>
                {
                    Destroy(completedBus);
                });
            
            if (allBuses.Count <= 0)
            {
                isChangingGoal = false;
                LevelManager.Instance.isGameFinished = true;
                LevelManager.Instance.isLevelCompleted = true;
                LevelManager.Instance.isGamePlayable = false;
                // StartCoroutine(UIManager.Instance.OpenWinScreen());
                return;
            }

            currentGoalBus = allBuses[0];
            HandleLevelBusMovements();
            isChangingGoal = false;
        }

        private void HandleLevelBusMovements()
        {
            StartCoroutine(BusMovements());
        }

        private IEnumerator BusMovements()
        {
            foreach (var bus in allBuses)
            {
                bus.transform.DOLocalMoveX(bus.transform.localPosition.x - 5.75f, 1f).SetEase(Ease.InSine).OnComplete(
                    () =>
                    {
                        onBusChangeDone?.Invoke();
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
            isChangingGoal = true;
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
    }
}