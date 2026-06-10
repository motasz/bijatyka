using System;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Menu
{
    [Serializable]
    public enum Turn
    {
        Player1,
        Player2,
    }
    public class SelectionScreenController: MonoBehaviour
    {
        public Characters characters;
        public GameObject charactersRow;
        public GameObject panelPrefab;
        public PlayerSelection playerSelection;
        public Button startButton;
        public GameObject mainMenuCanva;
        
        [FormerlySerializedAs("_currentTurn")] public Turn currentTurn = Turn.Player1;
        public event Action<Turn> OnTurnChange;
        private InputAction _navigate;
        private Selectable _firstItem;

        public static SelectionScreenController Instance;

        private void Awake()
        {
            _navigate = InputSystem.actions.FindAction("Navigate");
            Cursor.visible = false;

            if (Instance == null)
            {
                Instance = this;
            }
            
            playerSelection.ResetAll();
        }

        private void Start()
        {
            for (var i = 0; characters.characters.Length > i; i++) 
            {
                var characterData = characters.characters[i];
                var panel = Instantiate(panelPrefab,  charactersRow.transform);
                panel.GetComponent<Image>().sprite = characterData.portrait;
                panel.GetComponent<Button>().onClick.AddListener(() => OnSelect(characterData));
                if (i == 0)
                {
                    _firstItem = panel.GetComponent<Selectable>();
                } 
            }
            
            _firstItem.Select();
        }

        private void OnEnable() 
        {
            if (_firstItem) _firstItem.Select();
            currentTurn = Turn.Player1;
            OnTurnChange?.Invoke(currentTurn);
            playerSelection.ResetAll();

        }

        private void OnSelect(CharacterData characterData)
        {
            UISoundPlayer.Instance.PlaySelect();
            if (currentTurn == Turn.Player2 && playerSelection.characters[(int)Turn.Player2] != null)
            {
                return;
            }

            if (currentTurn == Turn.Player2) startButton.interactable = true;
            
            playerSelection.SetCharacter(currentTurn, characterData);

            if (currentTurn == Turn.Player1) currentTurn = Turn.Player2;
            OnTurnChange?.Invoke(currentTurn);
        }

        public void GoBack()
        {
            UISoundPlayer.Instance.PlaySelect();
            mainMenuCanva.SetActive(true);
            gameObject.SetActive(false);
        }

        public void StartGame()
        {
            UISoundPlayer.Instance.PlaySelect();
            SceneManager.LoadScene("Game");
        }
    }
}