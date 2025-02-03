using System.Collections;
using BusJamClone.Scripts.Data;
using BusJamClone.Scripts.Runtime.Managers;
using DG.Tweening;
using UnityEngine;
namespace BusJamClone.Scripts.Runtime.Models
{
    public class Stickman : MonoBehaviour
    {
        public Animator anim;
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public GameColors gameColors;
        public Material startMat;
        public GridCell belongedGrid;
        public LevelData.GridColorType stickmanColorType;
        [SerializeField] private float turnSpeed;
        [SerializeField] private float moveSpeed;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Collider triggerCollider;
        public bool isMovingToBus;
        public bool isMovingToMatchArea;
        private float startZ;
        private float totalMovementz;
        public bool isMoving;

        public void Init(LevelData.GridColorType colorType, GridCell gridCell)
        {
            SetColor(colorType);
            belongedGrid = gridCell;
        }

        private void Start()
        {
            var dir = (GameplayManager.instance.GetCurrentBus().transform.position - transform.position).normalized;
            dir.y = 0;

            transform.rotation = Quaternion.LookRotation(dir);
        }

        private void SetColor(LevelData.GridColorType colorType)
        {
            stickmanColorType = colorType;
            var material = new Material(gameColors.ActiveMaterials[(int)stickmanColorType]);
            skinnedMeshRenderer.sharedMaterial = material;
        }

        public void Hang()
        {
            anim.SetBool("isIdle", false);
            anim.SetBool("isHang", true);
            anim.SetBool("isSit", false);
            anim.SetBool("isRun", false);
        }

        public void Sit()
        {
            anim.SetBool("isIdle", false);
            anim.SetBool("isHang", false);
            anim.SetBool("isSit", true);
            anim.SetBool("isRun", false);
        }

        public void Idle()
        {
            anim.SetBool("isIdle", true);
            anim.SetBool("isHang", false);
            anim.SetBool("isSit", false);
            anim.SetBool("isRun", false);
        }

        public void Run()
        {
            anim.SetBool("isIdle", false);
            anim.SetBool("isHang", false);
            anim.SetBool("isSit", false);
            anim.SetBool("isRun", true);
        }
        

        public void ResetColor()
        {
            skinnedMeshRenderer.material = startMat;
        }

        private IEnumerator ReturnParticleInTime(GameObject go)
        {
            yield return new WaitForSeconds(1);
        }

        public void GoTobus()
        {
            anim.SetBool("isRunning", true);
            CallForBackMove();
            StartCoroutine(GoTobusCoroutine());
        }

        public void CallForBackMove()
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position + new Vector3(0f, 0.5f, 0f), 0.1f,
                    (transform.position - GameplayManager.instance.GetCurrentBus().transform.position).normalized, out hit, 0.5f,
                    LayerMask.GetMask("StickmanParent")))
            {
                if (hit.transform.TryGetComponent(out Stickman stickman))
                {
                    if (stickman.isMoving) return;
                    stickman.isMoving = true;
                    stickman.CallForBackMove();
                    stickman.anim.SetBool("isRunning", true);
                    stickman.transform
                        .DOMove(
                            stickman.transform.position +
                            (GameplayManager.instance.GetCurrentBus().transform.position - stickman.transform.position)
                            .normalized / 2f,
                            0.15f).OnComplete(
                            () =>
                            {
                                stickman.anim.SetBool("isRunning", false);
                                stickman.isMoving = false;
                            });
                }
            }

            if (Physics.SphereCast(transform.position + new Vector3(0f, 0.5f, 0f), 0.1f,
                    (transform.position - GameplayManager.instance.GetCurrentBus().transform.position).normalized / Mathf.Sin(30),
                    out hit, 0.5f,
                    LayerMask.GetMask("StickmanParent")))
            {
                if (hit.transform.TryGetComponent(out Stickman stickman))
                {
                    if (Vector3.Distance(hit.transform.position, GameplayManager.instance.GetCurrentBus().transform.position) <
                        Vector3.Distance(transform.position, GameplayManager.instance.GetCurrentBus().transform.position)) return;
                    if (stickman.isMoving) return;
                    stickman.isMoving = true;
                    stickman.CallForBackMove();
                    stickman.anim.SetBool("isRunning", true);
                    stickman.transform
                        .DOMove(
                            stickman.transform.position +
                            (GameplayManager.instance.GetCurrentBus().transform.position - stickman.transform.position)
                            .normalized / 2f,
                            0.15f).OnComplete(
                            () =>
                            {
                                stickman.anim.SetBool("isRunning", false);
                                stickman.isMoving = false;
                            });
                }
            }

            if (Physics.SphereCast(transform.position + new Vector3(0f, 0.5f, 0f), 0.1f,
                    (transform.position - GameplayManager.instance.GetCurrentBus().transform.position).normalized * Mathf.Sin(30),
                    out hit, 0.5f,
                    LayerMask.GetMask("StickmanParent")))
            {
                if (hit.transform.TryGetComponent(out Stickman stickman))
                {
                    if (stickman.isMoving) return;
                    stickman.isMoving = true;
                    stickman.CallForBackMove();
                    stickman.anim.SetBool("isRunning", true);
                    stickman.transform
                        .DOMove(
                            stickman.transform.position +
                            (GameplayManager.instance.GetCurrentBus().transform.position - stickman.transform.position)
                            .normalized / 2f,
                            0.15f).OnComplete(
                            () =>
                            {
                                stickman.anim.SetBool("isRunning", false);
                                stickman.isMoving = false;
                            });
                }
            }
        }

        private IEnumerator GoTobusCoroutine()
        {
            yield return new WaitForEndOfFrame();
            if (!anim.GetBool("isRunning"))
            {
                anim.SetBool("isRunning", true);
            }

            var busPos = GameplayManager.instance.GetCurrentBus().transform.position;
            busPos.y = 0;
            if (Vector3.Distance(transform.position, busPos) > 1.75f)
            {
                Vector3 dir = (busPos - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(dir);
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);

                transform.position =
                    Vector3.MoveTowards(transform.position, transform.position + transform.forward,
                        moveSpeed * Time.deltaTime);
                StartCoroutine(GoTobusCoroutine());
            }
            else
            {
                JumpToBus();
            }
        }

        private void JumpToBus()
        {
            DOTween.Kill(transform);
            transform.SetParent(null);
            var busPos = GameplayManager.instance.GetCurrentBus().transform.position;
            busPos.y = 0;
            var jumpPath = new[]
            {
                transform.position,
                transform.position + (busPos - transform.position).normalized * 0.25f + new Vector3(0f, 0.45f, 0f),
                transform.position + (busPos - transform.position).normalized * 0.5f + new Vector3(0f, 0.75f, 0f),
                transform.position + (busPos - transform.position).normalized * 0.75f + new Vector3(0f, 0.25f, 0f),
                transform.position + (busPos - transform.position).normalized * 1.5f + new Vector3(0f, -2.5f, 0f),
                transform.position + (busPos - transform.position).normalized * 2f + new Vector3(0f, -7.5f, 0f)
            };

            GameplayManager.instance.GetCurrentBus().GetStickman();
            transform.DOPath(jumpPath, 0.25f, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(
                () =>
                {
                    // transform.DOMoveY(-10, 1.5f).SetEase(Ease.OutCubic).OnComplete(() =>
                    // {

                    gameObject.SetActive(false);
                    // });
                });
        }

        public void IncreaseForwardStep(bool isFull)
        {
            totalMovementz += (isFull ? 1f : 0.5f);
        }

        public void GoForward()
        {
            anim.SetBool("isRunning", true);

            var newZ = startZ + totalMovementz;
            transform.DOLocalMoveZ(newZ, 0.2f)
                .OnComplete(() => { anim.SetBool("isRunning", false); });
        }

        public void GoToMatchArea(MatchArea matchArea, Transform matchAreaPosition,
            bool fromLane)
        {
            if (fromLane)
            {
                IncreaseForwardStep(fromLane);
                GoForward();
            }

            DOVirtual.DelayedCall(0.2f, () =>
            {
                anim.SetBool("isRunning", true);
                var path = CreateCirclePath(transform.position, matchAreaPosition.transform.position, fromLane);
                if (DOTween.IsTweening(transform) && !isMovingToBus)
                {
                    DOTween.Kill(transform);
                }

                transform.DOPath(path, path.Length is 3 or 2 ? 0.25f : 0.75f, PathType.CatmullRom).SetEase(Ease.Linear)
                    .OnComplete(
                        () =>
                        {

                            isMovingToMatchArea = false;
                            transform.position = matchAreaPosition.position;
                            anim.SetBool("isRunning", false);
                            Vector3 dir = (GameplayManager.instance.GetCurrentBus().transform.position - transform.position)
                                .normalized;
                            Quaternion lookRotation = Quaternion.LookRotation(dir);
                            transform.DORotateQuaternion(lookRotation, 0.25f).OnComplete(() =>
                            {
                                if (!DOTween.IsTweening(transform))
                                {
                                    // transform.SetParent(prevGroup.transform);
                                }

                                transform.position = matchAreaPosition.position;
                            });
                        }).SetLookAt(0.01f);
            });
        }


        public Vector3[] CreateCirclePath(Vector3 startPosition, Vector3 targetPosition, bool fromLane)
        {
            Vector3[] path = new Vector3[7];
            var busPos = GameplayManager.instance.GetCurrentBus().transform.position;
            // Calculate the start and end angles based on start and target positions
            Vector3 startRelative = startPosition - busPos;
            Vector3 targetRelative = targetPosition - busPos;
            float startAngle = Mathf.Atan2(startRelative.y, startRelative.x);
            float targetAngle = Mathf.Atan2(targetRelative.y, targetRelative.x);

            // Check angle difference and adjust for 360 degree wrap around
            float angleDifference = Mathf.Abs(targetAngle - startAngle);

            if (angleDifference > Mathf.PI)
            {
                angleDifference = 2 * Mathf.PI - angleDifference;
            }

            // Convert angle difference to degrees
            angleDifference *= Mathf.Rad2Deg;
            if (angleDifference > 90f)
            {
                angleDifference -= 180f;
            }

            // If the angle difference is small, go directly
            if (Mathf.Abs(angleDifference) < 180)
            {
                if (fromLane)
                {
                    return new[]
                    {
                        startPosition,
                        targetPosition
                    };
                }

                return new[]
                {
                    startPosition,
                    (startPosition + targetPosition) / 2 + ((startPosition + targetPosition) / 2 - busPos) / 4,
                    targetPosition
                };
            }

            if (targetAngle < startAngle)
            {
                targetAngle += 2 * Mathf.PI;
            }

            float angleStep = (targetAngle - startAngle) / (6 - 1);
            for (int i = 0; i < 6; i++)
            {
                float currentAngle = startAngle + angleStep * i;
                float x = busPos.x + 2.5f * Mathf.Cos(currentAngle);
                float z = busPos.z + 2.5f * Mathf.Sin(currentAngle);
                path[i] = new Vector3(x, transform.position.y, z);
            }

            path[6] = targetPosition;

            return path;
        }

        public void WrongSelectionMovement()
        {
            var oldPos = transform.position;
            anim.SetBool("isRunning", true);
            transform.DOMove(transform.position + (transform.forward / 4), 0.15f)
                .OnComplete(() =>
                {
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isBackwardRunning", true);
                    transform.DOMove(oldPos, 0.15f)
                        .OnComplete(() => { anim.SetBool("isBackwardRunning", false); });
                });
        }

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

        public IEnumerator CheckForGoal()
        {
            var currentGoal = GameplayManager.instance.GetCurrentGoal();
            if (!isMoving)
            {
                // GameplayManager.instance.StartCoroutine(GameplayManager.instance.HandleStickmanMovement());
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(CheckForGoal());
            }
            yield return null;
        }
    }
}