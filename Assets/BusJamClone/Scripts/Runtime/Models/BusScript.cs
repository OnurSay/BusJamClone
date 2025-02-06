using System.Collections.Generic;
using BusJamClone.Scripts.Data;
using BusJamClone.Scripts.Runtime.Managers;
using DG.Tweening;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Models
{
    public class BusScript : MonoBehaviour
    {
        [Header("Cached References")]
        [SerializeField] private List<Renderer> busRenderers;
        [SerializeField] private List<Seat> seats;
        [SerializeField] private LevelData.GridColorType busColor;
        [SerializeField] private Material filledSeatMaterial;
        [SerializeField] private GameObject busParentObject;
        [SerializeField] private ParticleSystem completeConfetti;
        [SerializeField] private Animator carDoorAnimator;
        [SerializeField] private Transform busEntranceTransform;
        [SerializeField] private GameColors gameColors;
        
        [Header("Parameters")]
        [SerializeField] private int seatedStickmanCount;
        [SerializeField] private int comingStickmanCount;
        [SerializeField] private int emptySeatIndex;

        private static readonly int startMovement = Animator.StringToHash("startMovement");

        public void Init(LevelData.GridColorType colorType)
        {
            busColor = colorType;
            HandleColorSet();
        }

        private void HandleColorSet()
        {
            var sharedMat = gameColors.activeMaterials[(int)busColor];
            foreach (var busRenderer in busRenderers)
            {
                var materialArray = busRenderer.sharedMaterials;
                materialArray[0] = sharedMat;
                busRenderer.sharedMaterials = materialArray;
            }
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
            if (seatedStickmanCount != 3 || GameplayManager.instance.GetIsChangingGoal()) return;
            GameplayManager.instance.SetIsChangingGoal(true);
            DOVirtual.DelayedCall(0.3f, CompleteAnimation);
        }

        private void HandleCarDoorAnimation()
        {
            carDoorAnimator.SetTrigger(startMovement);
        }

        private void HandleSeat()
        {
            var seat = GetEmptySeat();
            seat.stickmanRenderer.material.color = gameColors.activeMaterials[(int)busColor].color;
            seat.seatedStickman.SetActive(true);
            foreach (var seatRenderer in seat.seatRenderers)
            {
                seatRenderer.material = filledSeatMaterial;
            }

            var localScale = seat.seatParent.transform.localScale;
            seat.seatParent.transform.DOScale(localScale * 1.1f, 0.15f).OnComplete(() =>
            {
                seat.seatParent.transform.DOScale(localScale, 0.15f);
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