using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardSplitter : MonoBehaviour
{
    public PlayerInput player1;
    public PlayerInput player2;

    void Start()
    {
        var keyboard = Keyboard.current;

        player1.SwitchCurrentControlScheme("Keyboard1", keyboard);
        player2.SwitchCurrentControlScheme("Keyboard2", keyboard);
    }
}
