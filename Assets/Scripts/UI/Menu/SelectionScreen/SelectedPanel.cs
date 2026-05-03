using System;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.SelectionScreen
{
    public class SelectedPanel: MonoBehaviour
    {
        public Turn id;
        public PlayerSelection playerSelection;
        public Image characterSprite;
        public TextMeshProUGUI playerName;
        public TextMeshProUGUI characterName;
        public Sprite defaultPortrait;


        private void OnSelected(CharacterData? character, Turn index)
        {
            if (id != index) return;
            
            characterSprite.sprite = character?.portrait;
            characterName.text = character?.name;
        }

        private void Awake()
        {
            playerSelection.OnCharacterSelected += OnSelected;
        }

        private void Start()
        {
            playerName.text = id == Turn.Player1 ? "P1" : "P2";
            TurnAround();
        }

        private void OnEnable()
        {
            characterSprite.sprite = defaultPortrait;
        }

        private void TurnAround()
        {
            if (id == Turn.Player1) return;
            
            transform.Rotate(new Vector3(0, 180, 0));
            characterName.transform.Rotate(new Vector3(0, 180, 0));
            playerName.transform.Rotate(new Vector3(0, 180, 0));
        }
    }
}