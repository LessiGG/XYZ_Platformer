using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Components
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int _health;
        [SerializeField] private UnityEvent _onTakeDamage;
        [SerializeField] private UnityEvent _onDie;

        public void ModifyHealth(int value)
        {
            _health += value;
            if (value < 0)
                _onTakeDamage?.Invoke();

            if (_health <= 0)
                _onDie?.Invoke();
        }
    }
}