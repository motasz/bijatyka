using System;
using UnityEngine;

namespace PlayerCharacter
{
    public class PlayerState: MonoBehaviour
    {
        public int maxHp = 100;
        public int currentHp;

        private void Awake()
        {
            currentHp = maxHp;
        }

        public void ModifyHp(int amount) => currentHp += amount;
    }
}