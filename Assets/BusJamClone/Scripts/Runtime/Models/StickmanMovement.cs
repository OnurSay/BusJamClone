using System;
using BusJamClone.Scripts.Runtime.Interfaces;
using BusJamClone.Scripts.Runtime.Managers;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Models
{
    public class StickmanMovement : MonoBehaviour, IPathFollower
    {
        private Vector3[] path;
        public float runSpeed = 5f;
        public float rotationSpeed = 10f;
        public float stoppingDistance = 0.1f;
        private int currentWaypointIndex;
        private bool isMoving;
        private bool isPathSet;
        public Animator anim;
        private static readonly int isRunning = Animator.StringToHash("isRunning");
        private Action onCompleteCallback;

        private void OnEnable()
        {
            GameplayManager.instance.onGameLost += StopMovement;
        }

        private void OnDisable()
        {
            GameplayManager.instance.onGameLost -= StopMovement;
        }

        private void Update()
        {
            if (!isPathSet) return;
            HandleMovement();
        }

        private void HandleMovement()
        {
            if (isMoving && path != null && currentWaypointIndex < path.Length)
            {
                MoveTowardsTarget();
            }
            else
            {
                if(LevelManager.instance.isLevelFailed) return;
                FinalizeMovement();
            }
        }

        private void FinalizeMovement()
        {
            StopMovement();
            isPathSet = false;
            if (onCompleteCallback == null) return;
            onCompleteCallback.Invoke();
            onCompleteCallback = null;
        }

        private void MoveTowardsTarget()
        {
            var targetPosition = path[currentWaypointIndex];
            var direction = (targetPosition - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, runSpeed * Time.deltaTime);

            if (!(Vector3.Distance(transform.position, targetPosition) <= stoppingDistance)) return;
            currentWaypointIndex++;
            if (currentWaypointIndex < path.Length) return;
            FinalizeMovement();
        }

        public void SetPath(Vector3[] newPath, Action onComplete = null)
        {
            path = newPath;
            currentWaypointIndex = 0;
            isPathSet = true;
            isMoving = true;
            onCompleteCallback = onComplete;
        }

        public void ChangePath(Vector3[] newPath, Action onComplete = null)
        {
            StopMovement();

            path = newPath;
            currentWaypointIndex = Mathf.Clamp(currentWaypointIndex, 0, path.Length - 1);
            onCompleteCallback = onComplete;

            StartMovement();
        }

        private void StopMovement()
        {
            path = null;
            isPathSet = false;
            anim.SetBool(isRunning, false);
            isMoving = false;
        }

        private void StartMovement()
        {
            isMoving = true;
            anim.SetBool(isRunning, true);
        }

        public void Run(Vector3[] newPath, Action onComplete = null)
        {
            if (isMoving)
            {
                ChangePath(newPath, onComplete);
                return;
            }

            SetPath(newPath, onComplete);
            StartMovement();
        }

        public void Stop()
        {
            StopMovement();
        }
    }
}