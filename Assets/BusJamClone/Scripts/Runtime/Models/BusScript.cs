using System.Collections;
using System.Collections.Generic;
using BusJamClone.Scripts.Data;
using BusJamClone.Scripts.Runtime.LevelCreation;
using BusJamClone.Scripts.Runtime.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace BusJamClone.Scripts.Runtime.Models
{
    public class BusScript : MonoBehaviour
    {
        [SerializeField] private List<Renderer> busRenderers;
        [SerializeField] private List<Seat> seats;
        [SerializeField] private LevelData.GridColorType busColor;
        [SerializeField] private int seatedStickmanCount;
        [SerializeField] private int comingStickmanCount;
        [SerializeField] private int emptySeatIndex;
        [SerializeField] private Material filledSeatMaterial;
        [SerializeField] private GameObject busParentObject;
        [SerializeField] private ParticleSystem completeConfetti;
        [SerializeField] private Animator carDoorAnimator;
        [SerializeField] private bool isMoving;
        [SerializeField] private bool canComplete;
        [SerializeField] private Transform busEntranceTransform;

        public GameColors gameColors;
        private static readonly int startMovement = Animator.StringToHash("startMovement");

        public void Init(LevelData.GridColorType colorType)
        {
            busColor = colorType;
            HandleColorSet();
        }

        private void HandleColorSet()
        {
            var sharedMat = gameColors.ActiveMaterials[(int)busColor];
            foreach (var busRenderer in busRenderers)
            {
                var materialArray = busRenderer.sharedMaterials;
                materialArray[0] = sharedMat;
                busRenderer.sharedMaterials = materialArray;
            }
        }

        private void CheckForComplete()
        {
            if (seatedStickmanCount != 3 || GameplayManager.instance.GetIsChangingGoal()) return;
            GameplayManager.instance.SetIsChangingGoal(true);
            CompleteAnimation();
        }

        private void CompleteAnimation()
        {
            var localScale = busParentObject.transform.localScale;
            busParentObject.transform.DOScale(localScale * 1.1f, 0.15f).OnComplete(() =>
            {
                completeConfetti.Play();
                busParentObject.transform.DOScale(localScale, 0.15f).OnComplete(() =>
                {
                    DOVirtual.DelayedCall(0.25f, GameplayManager.instance.MoveToNextGoal);
                });
            });
        }


        public void GetStickman()
        {
            if (GameplayManager.instance.GetIsChangingGoal()) return;
            HandleCarDoorAnimation();
            HandleSeat();
            AddStickman(1);
            IncreaseSeatIndex();
        }

        private void HandleCarDoorAnimation()
        {
            carDoorAnimator.SetTrigger(startMovement);
        }

        private void HandleSeat()
        {
            var seat = GetEmptySeat();
            seat.stickmanRenderer.material.color = gameColors.ActiveMaterials[(int)busColor].color;
            seat.seatedStickman.SetActive(true);
            foreach (var seatRenderer in seat.seatRenderers)
            {
                seatRenderer.material = filledSeatMaterial;
            }

            var localScale = seat.seatParent.transform.localScale;
            seat.seatParent.transform.DOScale(localScale * 1.1f, 0.15f).OnComplete(() =>
            {
                seat.seatParent.transform.DOScale(localScale, 0.15f).OnComplete(CheckForComplete);
            });
        }

        private Seat GetEmptySeat()
        {
            return seats[emptySeatIndex];
        }

        public int GetComingStickmanCount()
        {
            return comingStickmanCount;
        }

        public void AddComingStickman(int value)
        {
            comingStickmanCount += value;
        }

        private void AddStickman(int addValue)
        {
            seatedStickmanCount += addValue;
        }


        private void IncreaseSeatIndex()
        {
            emptySeatIndex++;
        }

        public LevelData.GridColorType GetColor()
        {
            return busColor;
        }

        public bool GetIsMoving()
        {
            return isMoving;
        }

        public void SetCanComplete(bool flag)
        {
            canComplete = flag;
        }

        public Transform GetEntranceTransform()
        {
            return busEntranceTransform;
        }
    }

    [System.Serializable]
    public class Seat
    {
        public GameObject seatParent;
        public List<Renderer> seatRenderers;
        public GameObject seatedStickman;
        public Renderer stickmanRenderer;
    }
}