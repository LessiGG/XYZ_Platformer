using System;
using PixelCrew.Animations;
using PixelCrew.Components.ColliderBased;
using PixelCrew.Utils;
using UnityEngine;

namespace PixelCrew.Creatures.Mobs
{
    public class TotemAI : MonoBehaviour
    {
        public ColliderCheck _vision;
        [SerializeField] private CoolDown _cooldown;
        
        private SpriteAnimation _spriteAnimation;

        private void Awake()
        {
            _spriteAnimation = GetComponent<SpriteAnimation>();
        }

        private void Update()
        {
            if (_vision.IsTouchingLayer && _cooldown.IsReady)
                Shoot();
        }

        public void Shoot()
        {
            _cooldown.Reset();
            _spriteAnimation.SetClip("start-attack");
        }
    }
}