using PixelCrew.Creatures.Hero;
using UnityEngine;

namespace PixelCrew.Effects
{
    public class BounceEffect : MonoBehaviour
    {
        [SerializeField] private float _bounceForce;
        private Hero _hero;
        
        private void Start()
        {
            _hero = FindObjectOfType<Hero>();
        }
        
        public void Bounce()
        {
            _hero.Bounce(_bounceForce);
        }
    }
}