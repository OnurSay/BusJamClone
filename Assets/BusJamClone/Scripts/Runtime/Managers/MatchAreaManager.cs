using System.Collections.Generic;
using System.Linq;
using BusJamClone.Scripts.Runtime.Models;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Managers
{
    public class MatchAreaManager : MonoBehaviour
    {
        [SerializeField] private List<MatchArea> matchAreas;
        [SerializeField] private List<MatchArea> claimedMatchAreas;
        public bool isSlideProcessing;

        public static MatchAreaManager instance;

        private void Awake()
        {
            InitializeSingleton();
        }

        private void InitializeSingleton()
        {
            if (instance) return;
            instance = this;
        }

        public void AddMatchArea(MatchArea matchArea)
        {
            if (matchAreas.Contains(matchArea)) return;
            matchAreas.Add(matchArea);
        }

        public List<MatchArea> GetMatchAreas()
        {
            return matchAreas;
        }

        public List<MatchArea> GetClaimedMatchAreas()
        {
            return claimedMatchAreas;
        }

        public MatchArea GetEmptyArea()
        {
            return matchAreas.Find(x => !x.HasStickman() && !x.IsReserved() ? x : null);
        }

        public MatchArea GetClosestEmptyArea(Transform referenceTransform)
        {
            return matchAreas
                .OrderBy(area =>
                    Vector3.Distance(referenceTransform.position,
                        area.transform.position)) // Order by distance to the referenceTransform
                .FirstOrDefault();
        }

        public void CheckForLastJump()
        {
            // if (claimedMatchAreas.Count == 3 && GameplayManager.instance.levelGoals.Count == 1)
            // {
            //     var colorType = claimedMatchAreas.Find(x => x.HasStickman()).GetCurrentStickmanGroup()
            //         .stickmanColorType;
            //     if (GameplayManager.instance.isChangingGoal)
            //     {
            //         GameplayManager.instance.isChangingGoal = false;
            //     }
            //
            //     if (GameplayManager.instance.GetCurrentGoal() == null)
            //     {
            //         GameplayManager.instance.AddGoal(new LevelGoal()
            //         {
            //             colorType = colorType
            //         });
            //         GameplayManager.instance.goalBus.ChangeColor(colorType);
            //     }
            //     else
            //     {
            //         GameplayManager.instance.GetCurrentGoal().colorType = colorType;
            //         GameplayManager.instance.goalBus.ChangeColor(colorType);
            //     }
            //
            //     foreach (var matchArea in claimedMatchAreas)
            //     {
            //         matchArea.HandleNewGoal();
            //     }
            // }
        }
    }
}