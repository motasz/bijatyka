using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Attack", menuName = "Data/Attack")]
    public class AttackData: ScriptableObject
    {
        public int damage;
        public float distance = 1;
        public float duration;
        public float windUp;
        public float active;
        public float windDown;
        public float activeMovement;
    }
}