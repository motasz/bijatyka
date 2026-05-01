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
    public bool jump;
    public bool attack;
    
    public InputBuffer buffer = new InputBuffer();

    private void Update()
    {
        buffer.Update();
    }

    private void SetMove(Vector2 newVal)
    {
        buffer.AddInput(InputType.Move, newVal);
        move = newVal;
    }

    private void SetJump(bool newVal) => buffer.AddInput(InputType.Jump);

    private void SetAttack(bool newVal) => buffer.AddInput(InputType.Attack);

    public void OnMove(InputValue value) => SetMove(value.Get<Vector2>());
    
    public void OnJump(InputValue value) => SetJump(value.isPressed);

    public void OnAttack(InputValue value) => SetAttack(value.isPressed);
}
