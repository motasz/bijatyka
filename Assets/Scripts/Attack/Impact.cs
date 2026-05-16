using System;
using UnityEngine;

namespace Attack
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Impact: MonoBehaviour
    {
        public Sprite[] frames;
        public float frameRate = 0.03f;
        
        private SpriteRenderer _spriteRenderer;
        private int _currentFrame;
        private float _timer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            _currentFrame = 0;
            _timer = 0;
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer > frameRate)
            {
                _timer = 0;
                _currentFrame++;

                if (_currentFrame >= frames.Length)
                {
                    gameObject.SetActive(false);
                    return; 
                }
                
                _spriteRenderer.sprite = frames[_currentFrame];
            }
            
        }
    }
}