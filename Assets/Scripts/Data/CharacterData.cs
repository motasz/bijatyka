using PlayerCharacter.Special;
using UnityEditor.Animations;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Character", menuName = "Data/Character")]
    public class CharacterData: ScriptableObject
    {
        public string name;
        public AnimatorController animatorController;
        public Sprite portrait;
        public SpeciallAttack special;
    }
}