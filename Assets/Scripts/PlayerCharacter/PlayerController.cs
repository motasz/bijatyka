using System;
using System.Collections;
using Attack;
using Data;
using PlayerCharacter;
using PlayerCharacter.Inputs;
using Unity.Mathematics.Geometry;
using UnityEngine;
using Math = System.Math;

public enum ControllerState
{
    Idle = 0,
    Walk = 1,
    Air = 2,
    Hit = 3,
    AttackTopWindUp = 4,
    AttackTopActive = 5,
    AttackTopWindDown = 6,
    AttackBotWindUp = 7,
    AttackBotActive = 8,
    AttackBotWindDown = 9,
    DodgeTop = 10,
    DodgeBot = 11,
}

public class PlayerController : MonoBehaviour
{
    [Header("References")] 
    public InputsReceiver inputs;
    public PlayerController enemy;
    public AttackController topBasicAttackHitbox;
    public AttackController botBasicAttackHitbox;

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
    public float dodgeDuration = 0.1f;
    public int maxStagger = 11;
    public int hitBlinkCount = 3;
    public float blinkDuration = 0.1f;
    public float stunDuration = 0.5f;
    public float stunMovement = 0.5f;

    private Coroutine? moveCoroutine = null;
    private Coroutine? hitCoroutine = null;
    private int _currentStagger;
    
    [SerializeField]
    private ControllerState  state = ControllerState.Idle;
    
    private HitDetector _hitDetector;
    public bool isGrounded = false;
    public bool isAfterDodge = false;
    private float verticalVelocity = 0f;

    private Animator _animator;
    private SpriteRenderer _renderer;
    private CharacterAudioPlayer _audioPlayer;

    private int _currentStateFrameCounter = 0;
    private ControllerState _previousStateBuffer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _hitDetector = GetComponent<HitDetector>();
        _renderer = GetComponent<SpriteRenderer>();
        _audioPlayer = GetComponent<CharacterAudioPlayer>();
        
        _currentStagger = maxStagger;
        _previousStateBuffer = state;
    }

    void Update()
    { 
        UpdateFrameCounter();
        GravityEffect();
        ProcessBufferedInput(); 
        AerialMove();
        VerticalMove();
        ClampHorizontalPosition();
        RotatePlayer();
        UpdateAnimator();
    }

    public void GetHit(int staggerVal)
    {
        _audioPlayer.PlayHit();
        
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }

        hitCoroutine = StartCoroutine(HitProcedure());
        
        if (state != ControllerState.Hit) 
        {
            _currentStagger -= staggerVal;
            
            if (_currentStagger <= 0)
            {
                Stun();
            }
        }
    }

    void Stun()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(StunProcedure());
    }

    IEnumerator HitProcedure()
    {
        for (var i = 0; hitBlinkCount > i; i++)
        {
            _renderer.enabled = false;
            yield return new WaitForSeconds(blinkDuration);
            _renderer.enabled = true;
            yield return new WaitForSeconds(blinkDuration);
        }

        _currentStagger = maxStagger;
        hitCoroutine = null;
    }

    IEnumerator StunProcedure()
    {
        state = ControllerState.Hit;
        var elapsedTime = 0f;
        var startPos =  transform.position;
        
        while (elapsedTime < stunDuration)
        {
            elapsedTime += Time.deltaTime;

            var xDelta = Mathf.Lerp(0, stunMovement, elapsedTime / stunDuration);
            
            var newPos = startPos + new Vector3(xDelta, 0, 0) * (IsEnemyToTheRight() ? -1 : 1);

            if (!ValidatePosition(newPos)) 
            {
                yield return null;
            }
            
            transform.position = newPos;
            yield return null;
        }
        
        moveCoroutine = null;
        BackToIdle();
    }

    void UpdateFrameCounter()
    {
        if (_previousStateBuffer == state)
        {
            ++_currentStateFrameCounter;
            return;
        }

        _previousStateBuffer = state;
        _currentStateFrameCounter = 0;
    }

    private int GetAnimationState()
    {
        if (state == ControllerState.Walk || state == ControllerState.DodgeTop) return (int)ControllerState.Air;

        return (int)state;
    }
    private void UpdateAnimator() 
    {
        var animationState = GetAnimationState();
        
        if (!_animator || _animator.GetInteger("state") == animationState) return;
        
        _animator.SetInteger("state", animationState);
    }

    private void BackToIdle()
    {
        state = ControllerState.Idle;
    }

    private void ProcessBufferedInput()
    {
        if (moveCoroutine != null || !isGrounded || state == ControllerState.Air) return;
        
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
            case InputType.DodgeUp:
                Dodge(DodgeState.Top);
                break;
            case InputType.DodgeDown:
                Dodge(DodgeState.Bot);
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
        var isTop = inputs.move.y >= 0;

        moveCoroutine = StartCoroutine(StandardAttackProcedure(isTop));
    }

    private void Dodge(DodgeState position)
    {
        moveCoroutine = StartCoroutine(DodgeProcedure(position));
    }

    private IEnumerator DodgeProcedure(DodgeState position)
    {
        ControllerState dodgeState = position == DodgeState.Top ? ControllerState.DodgeTop : ControllerState.DodgeBot;

        state = dodgeState;

        _hitDetector.dodgeState = position;
        yield return new WaitForSeconds(dodgeDuration);

        _hitDetector.dodgeState = null;

        moveCoroutine = null;
        BackToIdle();
    }

    private IEnumerator StandardAttackProcedure(bool isTop)
    {
        state = isTop ? ControllerState.AttackTopWindUp : ControllerState.AttackBotWindUp;
        yield return new WaitForSeconds(basicAttackData.windUp);

        state = isTop ? ControllerState.AttackTopActive : ControllerState.AttackBotActive;
        
        if (isTop)
        {
            topBasicAttackHitbox.Activate();
        }
        else botBasicAttackHitbox.Activate();

        var elapsedTime = 0f;
        var startPos = transform.position;

        while (elapsedTime < basicAttackData.active)
        {
            elapsedTime += Time.deltaTime;
            var xDelta = Mathf.Lerp(0, basicAttackData.activeMovement, elapsedTime / basicAttackData.active);
            
            var newPos = startPos + new Vector3(xDelta, 0, 0) * (IsEnemyToTheRight() ? 1 : -1);
            
            if (!ValidatePosition(newPos))
            {
              yield return null;  
            }

            transform.position = newPos;
            yield return null;
        }
        
        //yield return new WaitForSeconds(basicAttackData.active);

        topBasicAttackHitbox.Deactivate();
        botBasicAttackHitbox.Deactivate();
        state = isTop ? ControllerState.AttackTopWindDown : ControllerState.AttackBotWindDown;
        yield return new WaitForSeconds(basicAttackData.windDown);
        
        moveCoroutine = null;
        BackToIdle();
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

            if (state == ControllerState.Air)
            {
                BackToIdle();
            }
            
            isGrounded = true;
            transform.position = new Vector3(transform.position.x, verticalClamp, transform.position.z);
            verticalVelocity = 0f;
            return;
        }
        
        verticalVelocity += gravityForce * Time.deltaTime;
        state = ControllerState.Air;
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
        while (_currentStateFrameCounter < 15) 
        {
            yield return null;
        }
        
        state = ControllerState.Walk;
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
        BackToIdle();
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
