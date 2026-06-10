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

        public Color mediumHpColor;
        public Color highHpColor;
        public Color lowHpColor;

        private void Awake()
        {
            playerState.OnHpChange += UpdateBar;
            image = GetComponent<Image>();
            image.color = highHpColor;
        }

        private Color GetHpColor(float percentage)
        {
            if (percentage < 0.3) return lowHpColor;
            if (percentage < 0.7) return mediumHpColor;
            
            return highHpColor;
        }

        private void UpdateBar(int newHp)
        {
            var newHpNormalized = (float)newHp / 100f;
            image.fillAmount = newHpNormalized;
            image.color = GetHpColor(newHpNormalized);
        }
    }
}