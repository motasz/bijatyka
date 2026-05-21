using System;
using PlayerCharacter.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputType
{
    Jump,
    Move,
    Attack,
    DodgeUp,
    DodgeDown,
    Special
}
public class InputsReceiver : MonoBehaviour
{
    public Vector2 move;
    public float downTapThreshold = 0.1f;
    
    public readonly InputBuffer Buffer = new ();
    
    private bool _downPressed;
    private float _downPressedTime;

    private void Update()
    {
        Buffer.Update();
    }

    private void SetMove(Vector2 newVal)
    {
        HandleInputDown(newVal);
        
        move = newVal;
        if (newVal.x == 0) return;
        
        Buffer.AddInput(InputType.Move, newVal);
    }

    private void HandleInputDown(Vector2 moveVal)
    {
        bool pressingDown = moveVal.y < -0.5f;

        if (pressingDown & !_downPressed)
        {
            _downPressed = true;
            _downPressedTime = Time.time;
        }
        
        if (!pressingDown &&  _downPressed)
        {
            _downPressedTime = Time.time - _downPressedTime;

            if (_downPressedTime <= downTapThreshold)
            {
                Buffer.AddInput(InputType.DodgeDown);
            }

            _downPressed = false;
        }
    }

    private void SetJump() => Buffer.AddInput(InputType.Jump);

    private void SetAttack() => Buffer.AddInput(InputType.Attack);
    
    private void  SetDodgeUp() => Buffer.AddInput(InputType.DodgeUp);
    
    private void  SetDodgeDown() => Buffer.AddInput(InputType.DodgeDown);
    
    private void SetSpecial() => Buffer.AddInput(InputType.Special);
    
    public void OnMove(InputValue value) => SetMove(value.Get<Vector2>());
    
    public void OnJump(InputValue value) => SetJump();

    public void OnAttack(InputValue value) => SetAttack();

    public void OnSpecial(InputValue value) => SetSpecial();
    
    public void OnDodgeUp(InputValue value) =>  SetDodgeUp();
}
