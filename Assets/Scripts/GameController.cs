using System;
using Data;
using UI;
using UI.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace DefaultNamespace
{
    public class GameController: MonoBehaviour
    {
        public PlayerController player1;
        public PlayerController player2;
        public EndGameScreen endGameScreen;
        public PlayerSelection playerSelection;

        public static GameController Instance;

        private void Awake()
        {
            Instance = this;
        }
        

        public void EndGame(string killedPlayerName)
        {
            player1.Disable();
            player2.Disable();
            endGameScreen.Activate(player1.CompareTag(killedPlayerName) ? player2.tag : player1.tag);
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene("Menu");
            UISoundPlayer.Instance.PlaySelect();
            playerSelection.CleanUp();
        }

        public void PlayAgain()
        {
            SceneManager.LoadScene("Game");
            UISoundPlayer.Instance.PlaySelect();
        }
        
        public void Quit()
        {
            UISoundPlayer.Instance.PlaySelect();
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}