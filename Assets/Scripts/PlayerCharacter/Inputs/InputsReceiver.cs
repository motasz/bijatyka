using UnityEngine;
using UnityEngine.InputSystem;

public class InputsReceiver : MonoBehaviour
{
    public Vector2 move;
    private void SetMove(Vector2 newVal)
    {
        move = newVal;
    }

    public void OnMove(InputValue value) => SetMove(value.Get<Vector2>());
}
