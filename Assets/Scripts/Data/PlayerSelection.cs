using System;
using UI.Menu;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Menu/PlayerSelection")]
    public class PlayerSelection: ScriptableObject
    {
        public event Action<CharacterData, Turn> OnCharacterSelected;
        
        public CharacterData[] characters = new CharacterData[2];

        public CharacterData GetCharacter(int index) =>  characters[index];
        
        public void SetCharacter(Turn index, CharacterData character)
        {
            OnCharacterSelected?.Invoke(character, index);
            characters[(int)index] = character;
        }
        
        public void ResetCharacter(int index) =>  characters[index] = null;
        
        public void ResetAll()
        {
            OnCharacterSelected?.Invoke(null, Turn.Player1);
            OnCharacterSelected?.Invoke(null, Turn.Player2);
            characters = new CharacterData[2];
        }
    }
}