using System.Collections;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public InputsReceiver inputs;

    [Header("Movement")] 
    public float hopTime = 0.2f;

    public float hopDistance = 1f;

    private Coroutine? moveCoroutine = null;

    void Update()
    {
      HorizontalMove();   
    }

    private void HorizontalMove()
    {
        var moveValue = inputs.move.x;
        
        if (moveValue == 0) return;

        if (moveCoroutine == null) moveCoroutine = StartCoroutine(HopProcedure(moveValue));
    }

    private IEnumerator HopProcedure(float moveVal)
    {
        var elapsedTime = 0f;
        var startPos = transform.position;

        while (elapsedTime < hopTime)
        {
            elapsedTime += Time.deltaTime;
            
            var currentProgress =  elapsedTime / hopTime;
            
            var xDelta = Mathf.Lerp(0, moveVal * hopDistance, currentProgress);
            
            transform.position = startPos + new Vector3(xDelta, 0, 0);
            
            yield return null;
        }
        
        moveCoroutine = null;
    }
}
