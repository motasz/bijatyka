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
    DodgeDown
}
public class InputsReceiver : MonoBehaviour
{
    public Vector2 move;
    
    public readonly InputBuffer Buffer = new ();

    private void Update()
    {
        Buffer.Update();
    }

    private void SetMove(Vector2 newVal)
    {
        if (newVal.y == -1)
        {
            Buffer.AddInput(InputType.DodgeDown);
        }
        
        move = newVal;
        Debug.Log(move);
        if (newVal.x == 0) return;
        Buffer.AddInput(InputType.Move, newVal);
    }

    private void SetJump() => Buffer.AddInput(InputType.Jump);

    private void SetAttack() => Buffer.AddInput(InputType.Attack);
    
    private void  SetDodgeUp() => Buffer.AddInput(InputType.DodgeUp);
    
    private void  SetDodgeDown() => Buffer.AddInput(InputType.DodgeDown);
    
    public void OnMove(InputValue value) => SetMove(value.Get<Vector2>());
    
    public void OnJump(InputValue value) => SetJump();

    public void OnAttack(InputValue value) => SetAttack();
    
    public void OnDodgeUp(InputValue value) =>  SetDodgeUp();
}
