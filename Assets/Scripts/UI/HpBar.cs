using System;
using PlayerCharacter;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HpBar: MonoBehaviour
    {
        public PlayerState playerState;
        private Image image;

        private void Awake()
        {
            playerState.OnHpChange += UpdateBar;
            image = GetComponent<Image>();
        }

        private void UpdateBar(int newHp)
        {
            Debug.Log($"new hp: {newHp}, %: {newHp/100f}");
            Debug.Log(image);
            image.fillAmount = (float)newHp/100f;
        }
    }
}