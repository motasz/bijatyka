using System;
using TMPro;
using UnityEngine;

namespace UI.Menu
{
    public class EndGameScreen: MonoBehaviour
    {
        public TextMeshProUGUI winnerText;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }

        public void Activate(string winner)
        {
            winnerText.text = $"{winner} WON";
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }
    }
}