using System.Collections;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public InputsReceiver inputs;

    [Header("Bondaries")] 
    public float horizontalClamp = 8f;
    public float verticalClamp = -2.5f;
    
    [Header("Movement")] 
    public float hopTime = 0.2f;

    public float hopDistance = 1f;

    public float midAirSpeed = 1f;

    public float gravityForce = -20f;
    public float jumpForce = 10f;

    private Coroutine? moveCoroutine = null;
    
    private bool isGrounded = false;
    private float verticalVelocity = 0f;

    void Update()
    { 
        GravityEffect();
        Jump();
        HorizontalMove();  
        VerticalMove();
        BlockHorizontalMovement();
    }

    private void HorizontalMove()
    {
        
        var moveValue = inputs.move.x;
        
        if (moveValue == 0) return;
        
        if (!isGrounded)
        {
            transform.position += new Vector3(midAirSpeed * Time.deltaTime * moveValue, 0, 0);
            return;
        }

        if (moveCoroutine == null) moveCoroutine = StartCoroutine(HopProcedure(moveValue));
    }

    private void Jump()
    {
        if (isGrounded && inputs.jump)
        {
            verticalVelocity = jumpForce;
        }
    }

    private void GravityEffect()
    {
        if (transform.position.y <= verticalClamp)
        {
            isGrounded = true;
            transform.position = new Vector3(transform.position.x, verticalClamp, transform.position.z);
            verticalVelocity = 0f;
            return;
        }
        
        verticalVelocity += gravityForce * Time.deltaTime;
        inputs.jump = false;
        isGrounded = false;
    }

    private void VerticalMove()
    {
        transform.position += new Vector3(0, verticalVelocity * Time.deltaTime, 0);
    }

    private void BlockHorizontalMovement()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -horizontalClamp, horizontalClamp),
            transform.position.y, transform.position.z);
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

            if (transform.position.x < -horizontalClamp || transform.position.x > horizontalClamp)
            {
                break;
            }
            
            yield return null;
        }
        
        moveCoroutine = null;
    }
}
