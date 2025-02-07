using System.Collections.Generic;
using BusJamClone.Scripts.Data;
using BusJamClone.Scripts.Runtime.Managers;
using DG.Tweening;
using TMPro;
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

        [Header("Reserve Settings")] 
        [SerializeField] private GameObject flagParent;
        [SerializeField] private GameObject reservedFlagParent;
        [SerializeField] private GameObject doneFlagParent;
        [SerializeField] private TextMeshPro reservedFlagTMP;
        [SerializeField] private int reserveCount;

        [Header("Parameters")] 
        [SerializeField] private int seatedStickmanCount; 
        [SerializeField] private int comingStickmanCount;
        [SerializeField] private int emptySeatIndex;
        private static readonly int startMovement = Animator.StringToHash("startMovement");

        public void Init(LevelData.GridColorType colorType, int count)
        {
            busColor = colorType;
            HandleColorSet();
            HandleReserved(count);
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

        private void HandleReserved(int count)
        {
            if (count == 0) return;
            reserveCount = count;
            flagParent.SetActive(true);
            reservedFlagParent.SetActive(true);
            reservedFlagTMP.text = "R  " + reserveCount;
        }

        private void CompleteAnimation()
        {
            var localScale = busParentObject.transform.localScale;
            busParentObject.transform.DOScale(localScale * 1.1f, 0.15f).OnComplete(() =>
            {
                completeConfetti.Play();

                if (AudioManager.instance)
                {
                    AudioManager.instance.PlayMatchComplete();
                }

                busParentObject.transform.DOScale(localScale, 0.15f).OnComplete(() =>
                {
                    DOVirtual.DelayedCall(0.25f, GameplayManager.instance.MoveToNextGoal);
                });
            });
        }


        public void GetStickman(bool reserved)
        {
            if (GameplayManager.instance.GetIsChangingGoal()) return;
            
            HandleCarDoorAnimation();
            HandleSeat(reserved);
            AddStickman(1, reserved);
            IncreaseSeatIndex();
            
            if (seatedStickmanCount != 3 || GameplayManager.instance.GetIsChangingGoal()) return;
            
            GameplayManager.instance.SetIsChangingGoal(true);
            DOVirtual.DelayedCall(0.3f, CompleteAnimation);
        }

        private void HandleCarDoorAnimation()
        {
            carDoorAnimator.SetTrigger(startMovement);
        }

        private void HandleSeat(bool reserved)
        {
            var seat = GetEmptySeat();
            seat.stickmanRenderer.material.color = gameColors.activeMaterials[(int)busColor].color;
            seat.seatedStickman.SetActive(true);
            seat.seatedStickmanHat.SetActive(reserved);
               
            if (AudioManager.instance)
            {
                AudioManager.instance.PlayPlaceStickman();
            }
            
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

        private void AddStickman(int addValue, bool reserved)
        {
            seatedStickmanCount += addValue;
            if (reserved)
            {
                HandleReservedSit();
            }
        }

        public void DecreaseReservedCount()
        {
            reserveCount--;
        }

        private void HandleReservedSit()
        {
            if (reserveCount <= 0)
            {
                CompleteReserve();
            }
        }

        private void CompleteReserve()
        {
            reservedFlagParent.SetActive(false);
            doneFlagParent.SetActive(true);
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

        public int GetReservedCount()
        {
            return reserveCount;
        }

        public bool IsLastSeat()
        {
            return emptySeatIndex == seats.Count - 1 || comingStickmanCount == seats.Count - 1;
        }
    }

    [System.Serializable]
    public class Seat
    {
        public GameObject seatParent;
        public List<Renderer> seatRenderers;
        public GameObject seatedStickman;
        public GameObject seatedStickmanHat;
        public Renderer stickmanRenderer;
    }
}