using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BusJamClone.Scripts.Runtime.Managers
{
    public class UIManager : MonoBehaviour
    {
        [Header("Cached References")] [SerializeField]
        private Image screenTransitionImage;

        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private GameObject loseScreen;

        [SerializeField] private GameObject levelCompleteConfetti;
        public static UIManager instance;

        private void Awake()
        {
            InitializeSingleton();
        }

        private void InitializeSingleton()
        {
            if (instance) return;
            instance = this;
        }

        public void OpenTransition(Action callback)
        {
            var color = screenTransitionImage.color;
            screenTransitionImage.DOColor(new Color(color.r, color.g, color.b, 1f), 0.5f).OnComplete(() =>
            {
                callback?.Invoke();
            });
        }

        public void CloseTransition()
        {
            var color = screenTransitionImage.color;
            screenTransitionImage.DOColor(new Color(color.r, color.g, color.b, 0f), 0.5f);
        }

        public void CloseLoadingScreen()
        {
            loadingScreen.SetActive(false);
        }

        public void OpenLoseScreen()
        {
            loseScreen.transform.parent.gameObject.SetActive(true);
            loseScreen.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
        }

        private void CloseLoseScreen(Action callBack)
        {
            loseScreen.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
            {
                loseScreen.transform.parent.gameObject.SetActive(false);
                callBack?.Invoke();
            });
        }

        public void RestartButton()
        {
            CloseLoseScreen(() => { LevelManager.instance.RestartLevel(); });
        }

        public void LevelCompleteEvents()
        {
            levelCompleteConfetti.SetActive(true);
        }
    }
}