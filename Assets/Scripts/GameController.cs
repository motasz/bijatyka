using System;
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
        }

        public void PlayAgain()
        {
            SceneManager.LoadScene("Game");
        }
        
        public void Quit()
        {
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}