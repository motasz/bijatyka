using System;
using Attack;
using UnityEngine;

namespace PlayerCharacter
{
    [Serializable]
    public enum DodgeState
    {
        Top,
        Bot
    }
    public class HitDetector: MonoBehaviour
    {
        public DodgeState? dodgeState = null;
        private PlayerState _playerState;
        private PlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _playerState = GetComponentInParent<PlayerState>();
        }

        private void OnTriggerEnter2D (Collider2D other) 
        {
            Debug.Log($"Collision at {dodgeState}");
            var attack = other.gameObject.GetComponent<AttackController>();

            if (attack == null || attack.damage == 0 || (attack.dodgeableBy != null && attack.dodgeableBy == dodgeState)) return;

            if (other.transform.parent.CompareTag(transform.tag)) return;
            
            _playerController.GetHit(attack.staggerDamage);
            attack.ActivateImpact();
            _playerState.ModifyHp(-attack.damage);
        }
    }
}