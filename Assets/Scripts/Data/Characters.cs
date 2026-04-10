using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "AllCharacters", menuName = "Data/Characters List")]
    public class Characters: ScriptableObject
    {
        public CharacterData[] characters;
    }
}