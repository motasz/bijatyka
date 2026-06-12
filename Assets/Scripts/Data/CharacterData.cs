using PlayerCharacter.Special;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Character", menuName = "Data/Character")]
    public class CharacterData: ScriptableObject
    {
        public string name;
        public RuntimeAnimatorController animatorController;
        public Sprite portrait;
        public SpeciallAttack special;
    }
}