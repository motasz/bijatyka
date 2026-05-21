using UnityEngine;

namespace DefaultNamespace
{
    public class SelfDestroyer: MonoBehaviour
    {
        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}