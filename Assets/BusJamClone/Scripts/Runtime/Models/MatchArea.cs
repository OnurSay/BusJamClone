using System.Collections;
using BusJamClone.Scripts.Runtime.Managers;
using DG.Tweening;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Models
{
    public class MatchArea : MonoBehaviour
    {
        [SerializeField] private Stickman ownedStickman;
        [SerializeField] private bool isReserved;

        private void Update()
        {
            CheckReserve();
        }

        private void CheckReserve()
        {
            if (GameplayManager.instance.GetIsGridCheckOnProgress() || !isReserved || ownedStickman) return;
            isReserved = false;
        }

        public void HandleNewGoal()
        {
            if (!HasStickman()) return;
            isReserved = false;
            if (!GameplayManager.instance.GetCurrentBus()) return;
            var currentBus = GameplayManager.instance.GetCurrentBus();
              
            if (ownedStickman.GetColor() == currentBus.GetColor())
            {
                if (ownedStickman.isMoving)
                {
                    ownedStickman.StartCoroutine(ownedStickman.CheckForGoal());
                    return;
                }

                var stickmanTransform = ownedStickman.transform;
                var stickmanPosition = ownedStickman.transform.position;
                var busTransform = currentBus.transform;
                var busPosition = currentBus.transform.position;

                RemoveStickman();
                ownedStickman.transform.SetParent(currentBus.transform);
                currentBus.SetCanComplete(false);
                
                //TODO:MATCH AREA TO BUS MOVEMENT
                // var movePath = new[]
                // {
                //     shapePosition, (shapePosition + goalPosition) / 2f + new Vector3(0f, 1f, 0f),
                //     goalPosition + (shapePosition - goalPosition).normalized * 0.5f + new Vector3(0f, 1f, 0f),
                //     goalPosition + new Vector3(0f, 1f, 0f), goalPosition
                // };
                //
                // shape.gameObject.transform.DOPath(movePath, 0.8f, PathType.CatmullRom).OnComplete(() =>
                // {
                //     shape.DisableTrail();
                //     shape.transform.SetParent(currentBus.transform);
                //     currentBus.SetCanComplete(true);
                //     currentBus.AddShape(shape);
                //     currentBus.PlaceSubShape(shape);
                // });
                // shape.gameObject.transform.DOScale(Vector3.one * 0.6666667f, 0.8f);
            }
            else
            {
                DOVirtual.DelayedCall(0.5f, CheckForEmptyArea);
            }
        }

        private void CheckForEmptyArea()
        {
            var area = MatchAreaManager.instance.GetEmptyArea();
            var matchAreas = MatchAreaManager.instance.GetMatchAreas();
            var targetAreaIndex = matchAreas.IndexOf(area);
            var thisIndex = matchAreas.IndexOf(this);
            if (area != this && targetAreaIndex < thisIndex)
            {
                area.isReserved = true;
                HandleJumpyMovement(targetAreaIndex, thisIndex);
            }
        }

        private void HandleJumpyMovement(int targetIndex, int thisIndex)
        {
            var newIndex = thisIndex - 1;
            if (newIndex < targetIndex)
            {
                isReserved = false;
                return;
            }

        }

        public bool HasStickman()
        {
            return ownedStickman;
        }

        public void AddStickman(Stickman stickman)
        {
            ownedStickman = stickman;
        }

        private void RemoveStickman()
        {
            ownedStickman = null;
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
            if (GameplayManager.instance.GetCompletedBuses().Count > 0 || GameplayManager.instance.GetIsGridCheckOnProgress())
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
    }
}