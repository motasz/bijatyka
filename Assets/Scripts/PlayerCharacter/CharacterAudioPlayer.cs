using System;
using UnityEngine;

namespace PlayerCharacter
{
    public class CharacterAudioPlayer: MonoBehaviour
    {
        public AudioClip hit;
        
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayHit()
        {
            _audioSource.PlayOneShot(hit);
        }
    }
}