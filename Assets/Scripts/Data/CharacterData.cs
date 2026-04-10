using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Character", menuName = "Data/Character")]
    public class CharacterData: ScriptableObject
    {
        public string name;
        public Sprite sprite;
        public Sprite portrait;
    }
}