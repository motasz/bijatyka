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
        public Color player1Color;
        public Color player2Color;

        private void Start()
        {
            _button = GetComponent<Selectable>();
            _colors = GetComponent<Selectable>().colors;
            
            SelectionScreenController.Instance.OnTurnChange += OnTurnChange;
        }

        private void OnTurnChange(Turn turn)
        {
            _colors.selectedColor = SelectionScreenController.Instance.currentTurn == Turn.Player1
                ? player1Color
                : player2Color;
            _button.colors = _colors;
        }
    }
}