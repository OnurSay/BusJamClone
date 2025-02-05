using System.Collections;
using System.Collections.Generic;
using BusJamClone.Scripts.Runtime.Models;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Managers
{
    public class MatchAreaManager : MonoBehaviour
    {
        [SerializeField] private List<MatchArea> matchAreas;
        [SerializeField] private List<MatchArea> claimedMatchAreas;

        public static MatchAreaManager instance;

        private void OnEnable()
        {
            GameplayManager.instance.onBusChangeDone += HandleNewGoal;
        }

        private void OnDisable()
        {
            GameplayManager.instance.onBusChangeDone -= HandleNewGoal;
        }

        private void Awake()
        {
            InitializeSingleton();
        }

        private void InitializeSingleton()
        {
            if (instance) return;
            instance = this;
        }

        private void HandleNewGoal()
        {
            foreach (var matchArea in matchAreas)
            {
                matchArea.HandleNewGoal();
            }

            CheckMatchAreas();
        }

        public void AddMatchArea(MatchArea matchArea)
        {
            if (matchAreas.Contains(matchArea)) return;
            matchAreas.Add(matchArea);
        }

        public MatchArea GetEmptyArea()
        {
            return matchAreas.Find(x => !x.HasStickman() && !x.IsReserved() ? x : null);
        }

        private void CheckMatchAreas()
        {
            if (GameplayManager.instance.GetIsChangingGoal() ||
                GameplayManager.instance.GetStickmanThroughBus().Count > 0) return;

            if (claimedMatchAreas.Count != matchAreas.Count || claimedMatchAreas.Count == 0 ||
                matchAreas.Count == 0) return;
            GameplayManager.instance.LoseGame();
        }

        public void AssignMatchArea(MatchArea claimedArea)
        {
            if (!claimedMatchAreas.Contains(claimedArea))
            {
                claimedMatchAreas.Add(claimedArea);
            }

            CheckMatchAreas();
        }

        public void RemoveMatchArea(MatchArea matchArea)
        {
            if (claimedMatchAreas.Contains(matchArea))
            {
                claimedMatchAreas.Remove(matchArea);
            }
        }
    }
}