using System;
using PlayerCharacter;
using UnityEngine;

namespace Attack
{
    public class AttackController : MonoBehaviour
    {
        public int damage = 5;
        public int staggerDamage = 5;
        public DodgeState dodgeableBy;
        public Impact impact;
        
        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _collider.isTrigger = true;
            _collider.enabled = false;
        }

        public void ActivateImpact()
        {
            if (impact == null) return;
            
            impact.gameObject.SetActive(true);
        }

        public void Activate()
        {
            _collider.enabled = true;
        }

        public void Deactivate()
        {
            _collider.enabled = false;
        }
}
}