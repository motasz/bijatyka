using System;
using PlayerCharacter;
using UnityEngine;

namespace UI
{
    public class HpBar: MonoBehaviour
    {
        public PlayerState playerState;

        private void Awake()
        {
            playerState.OnHpChange += UpdateBar;
        }

        private void UpdateBar(int newHp)
        {
            Debug.Log($"new hp: {newHp}");
        }
    }
}