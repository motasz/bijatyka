using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Menu
{
    [System.Serializable]
    public record ScreenData
    {
        public List<Button> buttons;
        public string name;
    }
    
    public class MainMenuController: MonoBehaviour
    {
        public List<ScreenData> screens;
        public string defaultScreenName = "main";
        
        private ScreenData currentScreen;
        private int selectedButtonIndex = 0;
        private InputAction moveAction;

        private void Awake()
        {
            var defaultScreen = screens.Find(x => x.name == defaultScreenName);

            if (defaultScreen != null)
            {
                currentScreen = defaultScreen;
            }
            else
            {
               Debug.LogError("Screen " + defaultScreenName + " not found");
               return;
            }
            
            SelectButton(selectedButtonIndex);

            moveAction = InputSystem.actions.FindAction("Move");
        }

        private void Update()
        {
            if (moveAction.WasPressedThisFrame())
            {
                ChangeSelectedButton((int)-Mathf.Sign(moveAction.ReadValue<Vector2>().y));
            }
        }

        private void ChangeSelectedButton(int change)
        {
            if (change == 0) return;
            
            var buttonsCount = currentScreen.buttons.Count;
            var newIndex = (selectedButtonIndex + change + buttonsCount) % buttonsCount;
    
            SelectButton(newIndex);
        }

        private void SelectButton(int index)
        {
            selectedButtonIndex = index;
            var button = currentScreen.buttons[selectedButtonIndex];
            
            button.Select();
        }
    }
}