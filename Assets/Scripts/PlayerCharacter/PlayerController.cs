using System.Collections;
using Attack;
using Data;
using PlayerCharacter.Inputs;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")] 
    public InputsReceiver inputs;
    public PlayerController enemy;
    public GameObject topProjectileSpawner;
    public GameObject bottomProjectileSpawner;
    public GameObject projectilePrefab;

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

    [Header("Attack")] 
    public AttackData basicAttackData;

    private Coroutine? moveCoroutine = null;
    
    public bool isGrounded = false;
    private float verticalVelocity = 0f;
    void Update()
    { 
        GravityEffect();
        ProcessBufferedInput(); 
        AerialMove();
        VerticalMove();
        ClampHorizontalPosition();
        RotatePlayer();
    }

    private void ProcessBufferedInput()
    {
        if (moveCoroutine != null || !isGrounded) return;
        
        var buffer = inputs.Buffer;
        
        buffer.Update();
        
        var input = buffer.GetOldest();

        if (input == null) return;

        switch (input.Input)
        {
            case InputType.Move:
                HorizontalMove(input.Value);
                break;
            case InputType.Jump:
                Jump();
                break;
            case InputType.Attack:
                Attack();
                break;
        }
        
        buffer.RemoveOldest();
    }

    private void HorizontalMove(Vector2 value)
    {
        var moveValue = value.x;

        if (moveValue == 0) return;

        moveCoroutine = StartCoroutine(HopProcedure(moveValue));
    }

    // gdy jestesmy w powietrzu nie chcemy uzywac input buffera - ruch jest ciagly
    private void AerialMove()
    {
        if (isGrounded) return;
        transform.position += new Vector3(midAirSpeed * Time.deltaTime * inputs.move.x, 0, 0);
    }

    private void Jump()
    {
        
        verticalVelocity = jumpForce;
    }

    private void Attack()
    {
        if (inputs.move.y < 0)
        {
            SpawnAttackProjectile(bottomProjectileSpawner.transform.position);
            return;
        } 
        
        SpawnAttackProjectile(topProjectileSpawner.transform.position);
    }

    private void SpawnAttackProjectile(Vector3 pos)
    {
        var projectile =  Instantiate(projectilePrefab, pos, Quaternion.identity, transform);
        projectile.GetComponent<Projectile>().Initialize(basicAttackData);
    }

    private void GravityEffect()
    {
        if (transform.position.y <= verticalClamp)
        {
            // nie chcemy zeby inputy ruchu zakolejkowane w input bufferze podczas lotu się odpalały ---- w sumie nie jestem tego pewny, na razie niech zostanie wykomentowane
            if (!isGrounded)
            {
               // inputs.Buffer.FlushInputs(InputType.Move);
            }
            isGrounded = true;
            transform.position = new Vector3(transform.position.x, verticalClamp, transform.position.z);
            verticalVelocity = 0f;
            return;
        }
        
        verticalVelocity += gravityForce * Time.deltaTime;
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
