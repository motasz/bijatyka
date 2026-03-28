using System;
using System.Collections;
using Data;
using UnityEngine;

namespace Attack
{
    public class Projectile: MonoBehaviour
    {
        public AttackData attackData;

        public void Initialize(AttackData data)
        {
            attackData = data;
            StartCoroutine(Launch());
        }

        private IEnumerator Launch()
        {
            var startPos = transform.localPosition;
            var elapsed = 0f;

            while (elapsed < attackData.duration)
            {
                elapsed += Time.deltaTime;
                var deltaX = Mathf.Lerp(0, attackData.distance, elapsed / attackData.duration);
                
                transform.localPosition = new Vector3(startPos.x + deltaX, transform.localPosition.y, transform.localPosition.z);
                
                yield return null;
            }
            
            Destroy(gameObject);
        }
    }
}