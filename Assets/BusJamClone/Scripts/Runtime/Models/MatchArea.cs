using System.Collections;
using BusJamClone.Scripts.Runtime.Managers;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Models
{
    public class MatchArea : GridBase
    {
        [SerializeField] private bool isReserved;

        public void HandleNewGoal()
        {
            if (!HasStickman()) return;
            if (!GameplayManager.instance.GetCurrentBus()) return;
            var currentBus = GameplayManager.instance.GetCurrentBus();

            if (stickman.GetColor() != currentBus.GetColor()) return;


            SetReserved(false);
            MatchAreaManager.instance.RemoveMatchArea(this);
            stickman.GoToBus(null);
        }

        public bool HasStickman()
        {
            return stickman;
        }

        public void AddStickman(Stickman man)
        {
            stickman = man;
        }

        public bool IsReserved()
        {
            return isReserved;
        }

        public void SetReserved(bool flag)
        {
            isReserved = flag;
        }
    }
}