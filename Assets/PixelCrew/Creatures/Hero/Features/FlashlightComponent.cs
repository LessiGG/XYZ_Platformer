﻿using PixelCrew.Model;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace PixelCrew.Creatures.Hero.Features
{
    public class FlashlightComponent : MonoBehaviour
    {
        [SerializeField] private float _consumePerSecond;
        [SerializeField] private Light2D _light;
            
        private GameSession _session;
        private float _defaultIntensity;

        private void Start()
        {
            _session = GameSession.Instance;
            _defaultIntensity = _light.intensity;
        }

        private void Update()
        {
            var consumed = Time.deltaTime * _consumePerSecond;
            var currentvalue = _session.Data.Fuel.Value;
            var nextValue = currentvalue - consumed;
            nextValue = Mathf.Max(nextValue, 0);
            _session.Data.Fuel.Value = nextValue;

            var progress = Mathf.Clamp(nextValue / 20, 0, 1);
            _light.intensity = Mathf.Max(_defaultIntensity * progress, 0.3f);
        }
    }
}