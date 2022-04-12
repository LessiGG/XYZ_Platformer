using System.Collections.Generic;
using System.Linq;
using PixelCrew.Components.Health;
using PixelCrew.Utils;
using UnityEngine;

namespace PixelCrew.Creatures.Mobs
{
    public class TotemTower : MonoBehaviour
    {
        [SerializeField] private List<TotemAI> _totems;
        [SerializeField] private CoolDown _cooldown;

        private int _currentTotem;

        private void Start()
        {
            foreach (var totemAI in _totems)
            {
                totemAI.enabled = false;
                var hp = totemAI.GetComponent<HealthComponent>();
                hp._onDie.AddListener(() => OnTotemDie(totemAI));
            }
        }

        private void OnTotemDie(TotemAI totemAI)
        {
            var index = _totems.IndexOf(totemAI);
            _totems.Remove(totemAI);

            if (index < _currentTotem)
                _currentTotem--;
        }

        private void Update()
        {
            if (_totems.Count == 0)
            {
                enabled = false;
                Destroy(gameObject, 1f);
            }
            
            var hasAnyTarget = _totems.Any(x => x._vision.IsTouchingLayer);
            if (!hasAnyTarget || !_cooldown.IsReady) return;
            
            _totems[_currentTotem].Shoot();
            _cooldown.Reset();
            _currentTotem = (int) Mathf.Repeat(_currentTotem + 1, _totems.Count);
        }
    }
}