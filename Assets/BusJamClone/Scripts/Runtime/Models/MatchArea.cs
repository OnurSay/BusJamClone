using System;
using System.Collections;
using BusJamClone.Scripts.Runtime.Managers;
using DG.Tweening;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Models
{
    public class MatchArea : GridBase
    {
        [SerializeField] private bool isReserved;

        private void OnEnable()
        {
            GameplayManager.instance.onBusChangeDone += HandleNewGoal;
        }

        private void OnDisable()
        {
            GameplayManager.instance.onBusChangeDone -= HandleNewGoal;
        }

        private void Update()
        {
            CheckReserve();
        }

        private void CheckReserve()
        {
            // if (GameplayManager.instance.GetIsGridCheckOnProgress() || !isReserved || stickman) return;
            // isReserved = false;
        }

        private void HandleNewGoal()
        {
            if (!HasStickman()) return;
            isReserved = false;
            if (!GameplayManager.instance.GetCurrentBus()) return;
            var currentBus = GameplayManager.instance.GetCurrentBus();

            if (stickman.GetColor() != currentBus.GetColor()) return;
            if (stickman.isMoving)
            {
                stickman.KillTween();
            }

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

        private void RemoveStickman()
        {
            stickman = null;
        }


        public bool IsReserved()
        {
            return isReserved;
        }

        public void CheckForLose()
        {
            StartCoroutine(LoseCheckCoroutine());
        }

        private IEnumerator LoseCheckCoroutine()
        {
            if (GameplayManager.instance.GetCompletedBuses().Count > 0 ||
                GameplayManager.instance.GetIsGridCheckOnProgress())
            {
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(LoseCheckCoroutine());
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                // if (!GameplayManager.instance.DoesHaveForwardShape() || (!GameplayManager.instance.isChangingGoal && !GameplayManager.instance.isGridCheckOnProgress))
                // {
                // if (!GameplayManager.instance.isGridCheckOnProgress &&
                //     ((GameplayManager.instance.completedBuses.Count > 0 &&
                //       !GameplayManager.instance
                //           .DoesHaveForwardShape()) ||
                //      GameplayManager.instance.completedBuses.Count <= 0))
                // {
                //     if (MatchAreaManager.instance.GetMatchAreas()[^1].ownedStickman)
                //     {
                //         if (LevelManager.Instance.isGameFinished && !LevelManager.Instance.isGamePlayable)
                //             yield return null;
                //         if (LevelManager.Instance.isLevelFailed) yield break;
                //         LevelManager.Instance.isGameFinished = true;
                //         LevelManager.Instance.isGamePlayable = false;
                //         
                //         // UIManager.Instance.OpenNoSpaceLeft();
                //         // StartCoroutine(UIManager.Instance.OpenLoseScreen());
                //     }
                // }
            }
        }

        public void SetReserved(bool flag)
        {
            isReserved = true;
        }
    }
}