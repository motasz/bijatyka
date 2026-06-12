using System;
using UnityEngine;

namespace PlayerCharacter
{
    public class CharacterAudioPlayer: MonoBehaviour
    {
        public AudioClip hit;
        public AudioClip whoosh;
        
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayHit()
        {
            _audioSource.PlayOneShot(hit);
        }

        public void PlayWhoosh()
        {
            _audioSource.PlayOneShot(whoosh);
        }

        public void Play(AudioClip clip)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}