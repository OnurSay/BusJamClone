using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        [Header("Cached References")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip popClip;
        [SerializeField] private AudioClip busSit;
        [SerializeField] private AudioClip matchClip;
        [SerializeField] private AudioClip busHornClip;
        [SerializeField] private AudioClip levelCompleteClip;
        [SerializeField] private AudioClip levelFailClip;

        private void Awake()
        {
            InitializeSingleton();
        }

        private void InitializeSingleton()
        {
            if (!instance)
            {
                instance = this;
            }
        }

        public void PlayPop()
        {
            if (!GameplayManager.instance.GetAudio())
                return;
            audioSource.PlayOneShot(popClip);
        } 
        
        public void PlayPlaceStickman()
        {
            if (!GameplayManager.instance.GetAudio())
                return;
            audioSource.PlayOneShot(busSit);
        }

        public void PlayBusHorn()
        {
            if (!GameplayManager.instance.GetAudio())
                return;
            audioSource.PlayOneShot(busHornClip);
        }

        public void PlayMatchComplete()
        {
            if (!GameplayManager.instance.GetAudio())
                return;
            audioSource.PlayOneShot(matchClip);
        }

        public void PlayLevelComplete()
        {
            if (!GameplayManager.instance.GetAudio())
                return;
            audioSource.PlayOneShot(levelCompleteClip);
        }

        public void PlayLevelFail()
        {
            if (!GameplayManager.instance.GetAudio())
                return;
            audioSource.PlayOneShot(levelFailClip);
        }
    }
}