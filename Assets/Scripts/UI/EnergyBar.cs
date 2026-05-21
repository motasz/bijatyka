using PlayerCharacter;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EnergyBar: MonoBehaviour
    {
        public PlayerState playerState;
        private Image image;

        private Animator _animator;

        private void Awake()
        {
            playerState.OnEnergyChange += UpdateBar;
            image = GetComponent<Image>();
            _animator = GetComponent<Animator>();
        }

        private void UpdateBar(float newEnergy)
        {
            image.fillAmount = newEnergy/100f;
            _animator.SetBool("isMax", newEnergy > 99);
        }
    }
}