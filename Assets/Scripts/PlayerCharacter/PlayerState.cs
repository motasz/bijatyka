using System;
using UnityEngine;

namespace PlayerCharacter
{
    public class PlayerState: MonoBehaviour
    {
        public int maxHp = 100;
        public int currentHp;
        public float maxEnergy = 100f;
        public float currentEnergy;
        public float passiveGainEnergyValue;
        public float passiveGainEnergyInterval;


        private float _passiveGainTimer;
        public event Action<int> OnHpChange;
        public event Action<float> OnEnergyChange;
        private void Awake()
        {
            currentHp = maxHp;
            currentEnergy = 0;
            OnEnergyChange?.Invoke(currentEnergy);
        }

        private void Update()
        {
            _passiveGainTimer += Time.deltaTime;

            if (_passiveGainTimer >= passiveGainEnergyInterval)
            {
                ModifyEnergy(passiveGainEnergyValue);
                _passiveGainTimer = 0;
            }
        }

        public void ModifyHp(int amount) {
            currentHp += amount;
            currentHp = Mathf.Clamp(currentHp, 0, maxHp);
            
            OnHpChange?.Invoke(currentHp);
        }

        public void ModifyEnergy(float amount)
        {
            currentEnergy += amount;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            OnEnergyChange?.Invoke(currentEnergy);
        }
    }
}