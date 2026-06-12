using UnityEngine;

namespace DefaultNamespace
{
    public class SfxPlayer: MonoBehaviour
    {
        public static SfxPlayer Instance;
        private AudioSource _audioSource;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(AudioClip clip)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}