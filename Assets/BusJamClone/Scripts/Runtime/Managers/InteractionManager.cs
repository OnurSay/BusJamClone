using BusJamClone.Scripts.Runtime.Models;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Managers
{
    public class InteractionManager : MonoBehaviour
    {
        private Camera mainCam;
        public LayerMask stickmanLayer;
        private GameplayManager gameplayManager;
        public float selectRadius;

        private void Start()
        {
            mainCam = Camera.main;
            gameplayManager = GameplayManager.instance;
        }

        void Update()
        {
            if (!ShouldProcessInput())
            {
                return;
            }


            if (Input.GetMouseButtonDown(0))
            {
                ProcessRaycastInteraction();
            }
        }

        private bool ShouldProcessInput()
        {
            return LevelManager.instance.isGamePlayable && !LevelManager.instance.isLevelFailed;
        }

        void ProcessRaycastInteraction()
        {
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (!TryRayCast(ray, out var hitInfo, stickmanLayer)) return;
            if (!hitInfo.transform || !hitInfo.transform.CompareTag("Stickman")) return;
            TrySelectStickman(hitInfo);
        }

        private bool TryRayCast(Ray ray, out RaycastHit hitInfo, LayerMask layer)
        {
            return Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layer);
        }

        private void TrySelectStickman(RaycastHit hitInfo)
        {
            if (!hitInfo.transform.TryGetComponent(out Stickman stickman)) return;

            if (VibrationManager.instance)
            {
                VibrationManager.instance.Light();
            }

            if (!stickman.GetHasPath() && stickman.belongedGrid.y != 0) return;
            if (stickman.isMoving) return;
            var currentGoal = GameplayManager.instance.GetCurrentBus();

            var path = stickman.belongedGrid.closestPath;
            if (stickman.stickmanColorType == currentGoal.GetColor() &&
                currentGoal.GetComingStickmanCount() + 1 <= 3)
            {
                currentGoal.AddComingStickman(1);
                stickman.DisableInteraction();
                stickman.GoToBus(path);
                GridManager.instance.RecalculatePaths();
            }
            else
            {
                var availableMatchArea = MatchAreaManager.instance.GetEmptyArea();
                if (!availableMatchArea) return;
                stickman.DisableInteraction();
                stickman.GoToMatchArea(availableMatchArea, availableMatchArea.transform, path);
                GridManager.instance.RecalculatePaths();
            }
        }
    }
}