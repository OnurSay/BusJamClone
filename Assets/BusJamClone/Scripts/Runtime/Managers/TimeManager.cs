using System;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Managers
{
    public class TimeManager : MonoBehaviour
    {

        [SerializeField] private GameplayManager gameplayManager;
        [SerializeField] private float levelTime;
        [SerializeField] private bool isTimerActive;

        private void Update()
        {
            HandleTimer();
        }

        private void HandleTimer()
        {
            if(!isTimerActive) return;
            levelTime -= Time.deltaTime;
            if (levelTime <= 0f)
            {
                //TODO LOSE GAME;
            }
        }

        public void SetTimer(int time)
        {

            levelTime = time;
        }

        public void StartTimer()
        {
            isTimerActive = true;
        }
        

    }
}
