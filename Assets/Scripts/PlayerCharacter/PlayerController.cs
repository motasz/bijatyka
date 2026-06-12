using System;
using System.Collections;
using Attack;
using Data;
using PlayerCharacter;
using PlayerCharacter.Inputs;
using PlayerCharacter.Special;
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
    SpecialWindUp = 12,
    SpecialActive = 13,
    SpecialWindDown = 14
}

public class PlayerController : MonoBehaviour
{
    [Header("References")] 
    public InputsReceiver inputs;
    public PlayerController enemy;
    public AttackController topBasicAttackHitbox;
    public AttackController botBasicAttackHitbox;
    public PlayerSelection playerSelection;

    [Header("Boundaries")] 
    public float horizontalClamp = 8f;
    public float verticalClamp = -2.5f;
    public float minimalPlayerDistance = 0.5f;
    
    [Header("Horizontal movement")] 
    public float hopTime = 0.2f;
    public float hopDistance = 1f;
    public float midAirSpeed = 1f;
    public bool horizontalClampEnabled = true;
    public bool canTurn = true;

    [Header("Vertical movement")] 
    public float gravityForce = -20f;
    public float jumpForce = 10f;
    public bool gravityEnabled = true;

    [Header("Attack")] 
    public AttackData basicAttackData;
    public float dodgeDuration = 0.1f;
    public int maxStagger = 11;
    public int hitBlinkCount = 3;
    public float blinkDuration = 0.1f;
    public float stunDuration = 0.5f;
    public float stunMovement = 0.5f;
    public bool isInvincible = false;

    public float buffDuration = 0.3f;

    private Coroutine? moveCoroutine = null;
    private Coroutine? hitCoroutine = null;
    private Coroutine? buffCoroutine = null;
    private int _currentStagger;
    
    [SerializeField]
    private ControllerState  state = ControllerState.Idle;
    
    private HitDetector _hitDetector;
    public bool isGrounded = false;
    public float verticalVelocity = 0f;

    private Animator _animator;
    private SpeciallAttack _specialAttack;
    private SpriteRenderer _renderer;
    private CharacterAudioPlayer _audioPlayer;
    private PlayerState _playerState;

    private int _currentStateFrameCounter = 0;
    private ControllerState _previousStateBuffer;

    private bool _isCounterAttacking = false;
    private bool _isMovementDisabled = false;

    public Action OnHit;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _hitDetector = GetComponent<HitDetector>();
        _renderer = GetComponent<SpriteRenderer>();
        _audioPlayer = GetComponent<CharacterAudioPlayer>();
        _playerState = GetComponent<PlayerState>();
        
        _currentStagger = maxStagger;
        _previousStateBuffer = state;
        
        InitializeSpecificCharacter();
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

    public void MakeVisible()
    {
        _renderer.enabled = true;
    }

    public void MakeInvisible()
    {
        _renderer.enabled = false;
    }

    public void SetMoveRoutine(IEnumerator coroutine)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        
        moveCoroutine = StartCoroutine(coroutine);
    }

    private void ResetCrucialAttributes()
    {
        canTurn = true;
        isInvincible = false;
        gravityEnabled = true;
    }

    public void ResetMoveRoutine()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        ResetCrucialAttributes();
        moveCoroutine = null;
    }

    public void SetState(ControllerState newState)
    {
        state = newState;
    }

    public void Disable()
    {
        _isMovementDisabled = true;
    }

    public void Enable()
    {
        _isMovementDisabled = false;
    }

    private void Kill()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        if (hitCoroutine != null) StopCoroutine(hitCoroutine);
        
        Destroy(gameObject);
    }

    public void GetHit(int staggerVal)
    {
        OnHit?.Invoke();
        if (isInvincible) return;
        
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
        ResetMoveRoutine();
        topBasicAttackHitbox.Deactivate();
        botBasicAttackHitbox.Deactivate();

        moveCoroutine = StartCoroutine(StunProcedure());
    }

    public void CounterAttackBuff()
    {
        _isCounterAttacking = true;
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
        
        if (_playerState.IsDead()) Kill();
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
                newPos = transform.position;
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

    public void BackToIdle()
    {
        state = ControllerState.Idle;
    }

    private void ProcessBufferedInput()
    {
        if (_isMovementDisabled) return;
        var buffer = inputs.Buffer;
        buffer.Update();
        
        var input = buffer.GetOldest();

        if (input == null) return;

        if (ProcessCounter(input))
        {
            buffer.RemoveOldest();
            return;
        }
        
        if (moveCoroutine != null || (!isGrounded && input.Input != InputType.Special)) return;
        
        switch (input.Input)
        {
            case InputType.Move:
                HorizontalMove(input.Value);
                break;
            case InputType.Jump:
                Jump();
                break;
            case InputType.Attack:
                Attack(DodgeState.Top);
                break;
            case InputType.LowAttack: 
                Attack(DodgeState.Bot);
                break;
            case InputType.DodgeUp:
                Dodge(DodgeState.Top);
                break;
            case InputType.DodgeDown:
                Dodge(DodgeState.Bot);
                break;
            case InputType.Special:
                Special();
                break;
        }
        
        buffer.RemoveOldest();
    }

    private void Special()
    {
        if (!_playerState.IsMaxEnergy() || !_specialAttack.Validate(this)) return;
        
        _playerState.ModifyEnergy(-100);
        
        _specialAttack.StartSpecial(this);
    }

    private bool ProcessCounter(BufferedInput input)  
    {
        if (!_isCounterAttacking) return false;
        if (input.Input != InputType.Attack && input.Input != InputType.LowAttack) return false;
        
        ResetMoveRoutine();
        _hitDetector.dodgeState = null;
        
        Attack(input.Input == InputType.LowAttack ? DodgeState.Bot : DodgeState.Top, true);
        _isCounterAttacking = false;
        return true;
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
        if (isGrounded || _isMovementDisabled) return;
        transform.position += new Vector3(midAirSpeed * Time.deltaTime * inputs.move.x, 0, 0);
    }

    private void Jump()
    {
        verticalVelocity = jumpForce;
    }

    private void Attack(DodgeState topOrDown, bool isBuffed = false)
    {
        
        moveCoroutine = StartCoroutine(StandardAttackProcedure(topOrDown == DodgeState.Top, isBuffed));
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
        _isCounterAttacking = false;
        BackToIdle();
    }

    private IEnumerator StandardAttackProcedure(bool isTop, bool isBuffed = false)
    {
        if (!isBuffed)
        {
            state = isTop ? ControllerState.AttackTopWindUp : ControllerState.AttackBotWindUp;
            yield return new WaitForSeconds(basicAttackData.windUp);
        }

        state = isTop ? ControllerState.AttackTopActive : ControllerState.AttackBotActive;
        
        var damage = isBuffed ? basicAttackData.damage * 2 : basicAttackData.damage;
        var staggerDamage = isBuffed ? basicAttackData.damage * 3 : basicAttackData.damage;
        
        if (isTop)
        {
            topBasicAttackHitbox.damage = damage;
            topBasicAttackHitbox.staggerDamage = staggerDamage;
            topBasicAttackHitbox.Activate();
        }
        else
        {
            botBasicAttackHitbox.damage = damage;
            botBasicAttackHitbox.staggerDamage = staggerDamage;
            botBasicAttackHitbox.Activate();
        };

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
        isGrounded = false;
        
        if (!gravityEnabled) return;
        verticalVelocity += gravityForce * Time.deltaTime;
        state = ControllerState.Air;
    }

    private void VerticalMove()
    {
        transform.position += new Vector3(0, verticalVelocity * Time.deltaTime, 0);
    }

    private void ClampHorizontalPosition()
    {
        if (!enemy || !horizontalClampEnabled) return;
        
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
        if (!enemy || !canTurn) return;
        
        var rotateY = transform.rotation.eulerAngles.y;

        var desiredRotationY = IsEnemyToTheRight() ? 0f : 180f;

        if (Mathf.Approximately(desiredRotationY, rotateY)) return;
        
        transform.rotation = Quaternion.Euler(0, desiredRotationY, 0);
    }
    
    public bool IsEnemyToTheRight() => enemy.transform.position.x > transform.position.x;
    
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

    private void InitializeSpecificCharacter()
    {
        var characterData = playerSelection.GetCharacterDataOfPlayer(gameObject.tag);

        _animator.runtimeAnimatorController = characterData.animatorController;
        _specialAttack = characterData.special;
    }
}
