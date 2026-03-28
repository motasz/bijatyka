using System;
using Attack;
using UnityEngine;

namespace PlayerCharacter
{
    public class HitDetector: MonoBehaviour
    {
        private PlayerState _playerState;

        private void Awake()
        {
            _playerState = GetComponentInParent<PlayerState>();
        }

        private void OnTriggerEnter2D (Collider2D other)
        {
            var projectile = other.gameObject.GetComponent<Projectile>();

            if (projectile?.attackData == null) return;

            if (other.transform.parent.CompareTag(transform.tag)) return;
            
            _playerState.ModifyHp(-projectile.attackData.damage);
            Destroy(other.gameObject);
        }
    }
}