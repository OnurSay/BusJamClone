using BusJamClone.Scripts.Runtime.Models;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Managers
{
    public class InteractionManager : MonoBehaviour
    {
        [Header("Parameters")] 
        public LayerMask stickmanLayer;

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
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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

            if (!TimeManager.instance.GetIsTimerActive())
            {
                TimeManager.instance.StartTimer();
                UIManager.instance.StopBlinkTimer();
            }

            if (VibrationManager.instance)
            {
                VibrationManager.instance.Light();
            }

            if (!stickman.GetHasPath() && stickman.GetBelongedGrid().GetYAxis() != 0) return;
            if (stickman.GetIsMoving()) return;
            var currentGoal = GameplayManager.instance.GetCurrentBus();

            var path = stickman.GetBelongedGrid().GetClosestPath();
            if (stickman.GetColor() == currentGoal.GetColor() &&
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