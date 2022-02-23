using UnityEngine;

namespace PixelCrew.Components
{
    public class DealDamageComponent : MonoBehaviour
    {
        [SerializeField] private int _damage;

        public void ApplyDamage(GameObject target)
        {
            var healthComponent = target.GetComponent<HealthComponent>();
            if (healthComponent != null)
                healthComponent.ApplyDamage(_damage);
        }
    }
}