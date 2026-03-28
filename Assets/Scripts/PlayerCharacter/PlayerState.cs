using System;
using UnityEngine;

namespace PlayerCharacter
{
    public class PlayerState: MonoBehaviour
    {
        public int maxHp = 100;
        public int currentHp;

        public event Action<int> OnHpChange;

        private void Awake()
        {
            currentHp = maxHp;
        }

        public void ModifyHp(int amount) {
            currentHp += amount;
            currentHp = Mathf.Clamp(currentHp, 0, maxHp);
            
            OnHpChange?.Invoke(currentHp);
        }
    }
}