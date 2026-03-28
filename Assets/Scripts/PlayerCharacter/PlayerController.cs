using System.Collections;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")] 
    public InputsReceiver inputs;
    public PlayerController enemy;

    [Header("Boundaries")] 
    public float horizontalClamp = 8f;
    public float verticalClamp = -2.5f;
    public float minimalPlayerDistance = 0.5f;
    
    [Header("Horizontal movement")] 
    public float hopTime = 0.2f;
    public float hopDistance = 1f;
    public float midAirSpeed = 1f;

    [Header("Vertical movement")] 
    public float gravityForce = -20f;
    public float jumpForce = 10f;

    private Coroutine? moveCoroutine = null;
    
    public bool isGrounded = false;
    private float verticalVelocity = 0f;

    void Update()
    { 
        GravityEffect();
        Jump();
        HorizontalMove();  
        VerticalMove();
        ClampHorizontalPosition();
        RotatePlayer();
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

    private void ClampHorizontalPosition()
    {
        var boundaries = GetBoundaries();
        
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, boundaries.left, boundaries.right),
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
            
            var newPos = startPos + new Vector3(xDelta, 0, 0);

            if (!ValidatePosition(newPos)) break;
            
            transform.position = newPos;
            
            yield return null;
        }
        
        moveCoroutine = null;
    }

    private void RotatePlayer()
    {
        var rotateY = transform.rotation.eulerAngles.y;

        var desiredRotationY = IsEnemyToTheRight() ? 0f : 180f;

        if (Mathf.Approximately(desiredRotationY, rotateY)) return;
        
        transform.rotation = Quaternion.Euler(0, desiredRotationY, 0);
    }
    
    private bool IsEnemyToTheRight() => enemy.transform.position.x > transform.position.x;
    
    private float GetEffectivePlayerBoundary() => enemy.transform.position.x + (IsEnemyToTheRight() ? -minimalPlayerDistance : minimalPlayerDistance);

    private (float left, float right) GetBoundaries()
    {
        if (!isGrounded || !enemy.isGrounded) return (-horizontalClamp, horizontalClamp);
        
        return (IsEnemyToTheRight() ? -horizontalClamp : GetEffectivePlayerBoundary(),
            IsEnemyToTheRight() ? GetEffectivePlayerBoundary() : horizontalClamp);
    }

    private bool ValidatePosition(Vector3 pos)
    {
        var boundaries = GetBoundaries();
        return boundaries.left < pos.x && pos.x < boundaries.right;
    }
}
