using System;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;

namespace UI.Menu.SelectionScreen
{
    public class CharacterButton : MonoBehaviour
    {
        private Selectable _button;
        private ColorBlock _colors;

        private void Start()
        {
            _button = GetComponent<Selectable>();
            _colors = GetComponent<Selectable>().colors;
            
            SelectionScreenController.Instance.OnTurnChange += OnTurnChange;
        }

        private void OnTurnChange(Turn turn)
        {
            _colors.selectedColor = SelectionScreenController.Instance.currentTurn == Turn.Player1
                ? Color.red
                : Color.blue;
            _button.colors = _colors;
        }
}
}