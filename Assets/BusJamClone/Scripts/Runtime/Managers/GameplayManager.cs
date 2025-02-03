using System.Collections;
using System.Collections.Generic;
using BusJamClone.Scripts.Runtime.Models;
using UnityEngine;
using UnityEngine.Serialization;
using LevelGoal = BusJamClone.Scripts.Runtime.LevelCreation.LevelGoal;

namespace BusJamClone.Scripts.Runtime.Managers
{
    [DefaultExecutionOrder(-1)]
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] public List<LevelGoal> levelGoals;
        public static GameplayManager instance;
        [SerializeField] private Material secretMaterial;
        [SerializeField] private bool isChangingGoal;
        [SerializeField] private bool isGridCheckOnProgress;
        [SerializeField] private BusScript currentGoalBus;
        [SerializeField] private List<BusScript> allBuses;
        [SerializeField] private List<BusScript> completedBuses;

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
        
        public LevelGoal GetCurrentGoal()
        {
            return levelGoals.Count > 0 ? levelGoals[0] : null;
        }

        public void AddGoal(LevelGoal goal)
        {
            levelGoals.Add(goal);
        }

        public void ClearGoals()
        {
            levelGoals.Clear();
        }
        
        
        
        // public void MoveToNextGoal()
        // {
        //     levelGoals.RemoveAt(0);
        //     if (levelGoals.Count <= 0)
        //     {
        //         isChangingGoal = false;
        //         LevelManager.Instance.isGameFinished = true;
        //         LevelManager.Instance.isLevelCompleted = true;
        //         LevelManager.Instance.isGamePlayable = false;
        //         StartCoroutine(UIManager.Instance.OpenWinScreen());
        //         return;
        //     }
        //     var currentGoal = levelGoals[0];
        //
        //     hole.ChangeColor(currentGoal.colorType);
        //     hole.SetNextGoalColor(levelGoals.Count == 1 ? null : levelGoals[1]);
        //     MatchAreaManager.instance.CheckForLastJump();
        //     StartCoroutine(HandleNewGoal());
        //     isChangingGoal = false;
        // }

        public IEnumerator HandleNewGoal()
        {
            isGridCheckOnProgress = true;
            yield return new WaitForSeconds(0.25f);
            var matchAreas = MatchAreaManager.instance.GetMatchAreas();
            for (var i = 0; i < matchAreas.Count; i++)
            {
                matchAreas[i].HandleNewGoal();
            }

            yield return new WaitForSeconds(0.8f);
            isGridCheckOnProgress = false;
            
        }
        
        public void AssignFirstGoal()
        {
            currentGoalBus.Init(levelGoals[0].colorType);
        }

        public void MoveToNextGoal()
        {
            levelGoals.RemoveAt(0);
            if (levelGoals.Count <= 0)
            {
                isChangingGoal = false;
                LevelManager.Instance.isGameFinished = true;
                LevelManager.Instance.isLevelCompleted = true;
                LevelManager.Instance.isGamePlayable = false;
                // StartCoroutine(UIManager.Instance.OpenWinScreen());
                return;
            }
            var currentGoal = levelGoals[0];
            //Handle new bus movement;
            MatchAreaManager.instance.CheckForLastJump();
            StartCoroutine(HandleNewGoal());
            isChangingGoal = false;
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
    }
}