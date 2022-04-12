using System;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Components.Health
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int _health;
        [SerializeField] private UnityEvent _onTakeDamage;
        [SerializeField] private UnityEvent _onHeal;
        [SerializeField] public UnityEvent _onDie;
        [SerializeField] private HealthChangeEvent _onChange;

        public void ModifyHealth(int value)
         {
            if (_health <= 0) return;
            
            _health += value;
            _onChange?.Invoke(_health);
            
            if (value < 0)
                _onTakeDamage?.Invoke();
            
            if (value > 0)
                _onHeal?.Invoke();

            if (_health <= 0)
                _onDie?.Invoke();
         }
        
#if UNITY_EDITOR
        [ContextMenu("Update Health")]
        private void UpdateHealth()
        {
            _onChange.Invoke(_health);
        }
        
#endif

        private void OnDestroy()
        {
            _onDie.RemoveAllListeners();
        }

        public void SetHealth(int health)
        {
            _health = health;
        }
    
        [Serializable]
        public class HealthChangeEvent : UnityEvent<int>{}
    }
}