﻿using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace PixelCrew.Creatures.Mobs.Boss
{
    public class ChangeLightComponent : MonoBehaviour
    {
        [SerializeField] private Light2D[] _lights;
        [ColorUsage(true, true)]
        [SerializeField] private Color _color;

        public void SetColor()
        {
            SetColor(_color);
        }
        
        public void SetColor(Color color)
        {
            foreach (var light2D in _lights)
                light2D.color = color;
        }
    }
}