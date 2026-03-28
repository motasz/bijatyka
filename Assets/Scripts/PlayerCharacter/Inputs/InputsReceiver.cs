using UnityEngine;
using UnityEngine.InputSystem;

public class InputsReceiver : MonoBehaviour
{
    public Vector2 move;
    public bool jump;
    public bool attack;
    private void SetMove(Vector2 newVal) => move = newVal;

    private void SetJump(bool newVal) => jump = newVal;

    private void SetAttack(bool newVal) => attack = newVal;

    public void OnMove(InputValue value) => SetMove(value.Get<Vector2>());
    
    public void OnJump(InputValue value) => SetJump(value.isPressed);

    public void OnAttack(InputValue value) => SetAttack(value.isPressed);
}
