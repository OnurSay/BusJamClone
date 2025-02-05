using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BusJamClone.Scripts.Data;
using BusJamClone.Scripts.Runtime.Managers;
using DG.Tweening;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Models
{
    public class Stickman : MonoBehaviour
    {
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public GameColors gameColors;
        public Material startMat;
        public GridBase belongedGrid;
        public LevelData.GridColorType stickmanColorType;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Collider triggerCollider;
        public bool isMoving;
        [SerializeField] private bool hasPath;
        [SerializeField] private Outline stickmanOutline;
        [SerializeField] private StickmanMovement stickmanMovement;
        [SerializeField] private GameObject reservedCap;
        [SerializeField] private bool isReserved;

        public void Init(LevelData.GridColorType colorType, GridBase gridCell)
        {
            SetColor(colorType);
            belongedGrid = gridCell;
        }

        private void SetColor(LevelData.GridColorType colorType)
        {
            stickmanColorType = colorType;
            var material = gameColors.ActiveMaterials[(int)stickmanColorType];
            skinnedMeshRenderer.sharedMaterial = material;
        }


        public void ResetColor()
        {
            skinnedMeshRenderer.material = startMat;
        }

        private IEnumerator ReturnParticleInTime(GameObject go)
        {
            yield return new WaitForSeconds(1);
        }

        public void GoToBus(List<GridBase> path)
        {
            transform.SetParent(null);
            isMoving = true;
            DissociateStickman();

            KillTween();

            var currentBus = GameplayManager.instance.GetCurrentBus();

            GameplayManager.instance.AddStickmanThroughBus(this);
            if (path != null)
            {
                var pathPositions = HandlePathPositions(path, currentBus.GetEntranceTransform().position);
                stickmanMovement.Run(pathPositions, JumpToBus);
            }
            else
            {
                var pathPositions = new[] { transform.position, currentBus.GetEntranceTransform().position };
                stickmanMovement.Run(pathPositions, JumpToBus);
            }
        }

        private void DissociateStickman()
        {
            belongedGrid.DissociateStickman();
            belongedGrid = null;
        }

        private Vector3[] HandlePathPositions(List<GridBase> gridBases, Vector3 endPos)
        {
            var newList = gridBases.Select(gridBase => gridBase.transform.position).ToList();
            newList.Insert(0, transform.position);
            newList.Add(endPos);
            return newList.ToArray();
        }

        private void JumpToBus()
        {
            transform.SetParent(null);
            gameObject.SetActive(false);
            GameplayManager.instance.RemoveStickmanThroughBus(this);
            GameplayManager.instance.GetCurrentBus().GetStickman();
        }


        public void GoToMatchArea(MatchArea matchArea, Transform matchAreaPosition, List<GridBase> path)
        {
            isMoving = true;
            transform.SetParent(null);
            DissociateStickman();
            AssignToMatchArea(matchArea);

            if (path != null)
            {
                matchArea.SetReserved(true);
                var pathPositions = HandlePathPositions(path, matchAreaPosition.position);
                stickmanMovement.Run(pathPositions, PlaceToMatchArea);
            }
            else
            {
                matchArea.SetReserved(true);
                var pathPositions = new[] { transform.position, matchAreaPosition.position };
                stickmanMovement.Run(pathPositions, PlaceToMatchArea);
            }
        }

        private void AssignToMatchArea(MatchArea matchArea)
        {
            belongedGrid = matchArea;
            matchArea.AddStickman(this);
        }

        private void PlaceToMatchArea()
        {
            MatchAreaManager.instance.AssignMatchArea(belongedGrid as MatchArea);
            stickmanMovement.Stop();
            transform.SetParent(belongedGrid.transform);
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
        }

        // public void WrongSelectionMovement()
        // {
        //     var oldPos = transform.position;
        //     anim.SetBool("isRunning", true);
        //     transform.DOMove(transform.position + (transform.forward / 4), 0.15f)
        //         .OnComplete(() =>
        //         {
        //             anim.SetBool("isRunning", false);
        //             anim.SetBool("isBackwardRunning", true);
        //             transform.DOMove(oldPos, 0.15f)
        //                 .OnComplete(() => { anim.SetBool("isBackwardRunning", false); });
        //         });
        // }

        public Collider GetCollider()
        {
            return triggerCollider;
        }

        public LevelData.GridColorType GetColor()
        {
            return stickmanColorType;
        }

        public void SetSecret()
        {
            //TODO: Secret Material Set to Stickman
        }

        public void EnableInteraction()
        {
            stickmanOutline.enabled = true;
            hasPath = true;
        }

        public void DisableInteraction()
        {
            stickmanOutline.enabled = false;
            hasPath = false;
        }

        public bool GetHasPath()
        {
            return hasPath;
        }

        public void KillTween()
        {
            DOTween.Kill(transform);
        }
    }
}