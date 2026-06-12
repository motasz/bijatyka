using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class AutoDestroyer: MonoBehaviour
    {
        public float destroyAfter = 3;

        private void Awake()
        {
            StartCoroutine(DestroyProcedure());
        }

        private IEnumerator DestroyProcedure()
        {
            yield return new WaitForSeconds(destroyAfter);
            if (gameObject.activeInHierarchy) Destroy(gameObject);
        }
    }
}