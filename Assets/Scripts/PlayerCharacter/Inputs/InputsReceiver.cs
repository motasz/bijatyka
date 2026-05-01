using System;
using PlayerCharacter.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputType
{
    Jump,
    Move,
    Attack
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
        Buffer.AddInput(InputType.Move, newVal);
        move = newVal;
    }

    private void SetJump(bool newVal) => Buffer.AddInput(InputType.Jump);

    private void SetAttack(bool newVal) => Buffer.AddInput(InputType.Attack);

    public void OnMove(InputValue value) => SetMove(value.Get<Vector2>());
    
    public void OnJump(InputValue value) => SetJump(value.isPressed);

    public void OnAttack(InputValue value) => SetAttack(value.isPressed);
}
