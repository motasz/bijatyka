using System;
using Attack;
using DefaultNamespace;
using UnityEngine;

namespace PlayerCharacter
{
    [Serializable]
    public enum DodgeState
    {
        Top,
        Bot,
        None
    }
    public class HitDetector: MonoBehaviour
    {
        public DodgeState? dodgeState = null;
        private PlayerState _playerState;
        private PlayerController _playerController;
        private CharacterAudioPlayer _audioPlayer;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _playerState = GetComponentInParent<PlayerState>();
            _audioPlayer = GetComponent<CharacterAudioPlayer>();
        }

        private void OnTriggerEnter2D (Collider2D other) 
        {
            var attack = other.gameObject.GetComponent<AttackController>();
            
            if (attack == null || (attack.damage == 0 && attack.staggerDamage == 0)) return;

            if (other.transform.parent.CompareTag(transform.tag)) return;
            
            if (attack.dodgeableBy != null && attack.dodgeableBy == dodgeState)
            {
                Debug.Log($"Counter attack buff for {transform.tag}");
                _playerController.CounterAttackBuff();
                _audioPlayer.PlayWhoosh();
                _playerState.GainDodgeEnergy();
                return;
            }
            
            _playerController.GetHit(attack.staggerDamage);
            attack.ActivateImpact();
            attack.AwardEnergy();
            
            if (!_playerController.isInvincible)
            {
                _playerState.ModifyHp(-attack.damage);
            }
        }
    }
}