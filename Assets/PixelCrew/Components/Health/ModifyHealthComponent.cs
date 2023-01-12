﻿using UnityEngine;

namespace PixelCrew.Components.Health
{
    public class ModifyHealthComponent : MonoBehaviour
    {
        [SerializeField] private int _value;

        public void SetDelta(int delta)
        {
            _value = delta;
        }

        public void ModifyHealth(GameObject target)
        {
            var healthComponent = target.GetComponent<HealthComponent>();
            if (healthComponent != null)
                healthComponent.ModifyHealth(_value);
        }
    }
}