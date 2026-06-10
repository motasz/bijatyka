using UnityEngine;

namespace UI
{
    public class UISoundPlayer: MonoBehaviour
    {
        public static UISoundPlayer Instance;
        public AudioClip navigationSound;
        public AudioClip selectSound;
        private AudioSource _audioSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlaySelect()
        {
            _audioSource.PlayOneShot(selectSound);
        }

        public void PlayNavigation()
        {
            _audioSource.PlayOneShot(navigationSound);
        }
    }
}