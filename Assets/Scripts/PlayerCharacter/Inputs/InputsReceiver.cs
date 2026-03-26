using UnityEngine;
using UnityEngine.InputSystem;

public class InputsReceiver : MonoBehaviour
{
    public Vector2 move;
    public bool jump;
    private void SetMove(Vector2 newVal) => move = newVal;

    private void SetJump(bool newVal) => jump = newVal;

    public void OnMove(InputValue value) => SetMove(value.Get<Vector2>());
    
    public void OnJump(InputValue value) => SetJump(value.isPressed);
}
