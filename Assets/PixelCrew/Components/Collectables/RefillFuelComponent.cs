using PixelCrew.Model;
using UnityEngine;

namespace PixelCrew.Components.Collectables
{
    public class RefillFuelComponent : MonoBehaviour
    {
        [SerializeField] private float _fuelPerSecond;
        private GameSession _session;

        private void Start()
        {
            _session = GameSession.Instance;
        }

        public void Refill()
        {
            var fuel = _session.Data.Fuel.Value;
            fuel += _fuelPerSecond;
            fuel = Mathf.Min(fuel, 100);
            _session.Data.Fuel.Value = fuel;
        }
    }
}