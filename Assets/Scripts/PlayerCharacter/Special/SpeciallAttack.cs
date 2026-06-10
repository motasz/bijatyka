using System;
using UnityEngine;

namespace PlayerCharacter.Special
{
    public abstract class SpeciallAttack: ScriptableObject
    {
        public abstract void StartSpecial(PlayerController playerController);

        public abstract bool Validate(PlayerController playerController);
    }
}