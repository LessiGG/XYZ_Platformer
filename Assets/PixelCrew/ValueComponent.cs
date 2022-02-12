using PixelCrew;
using UnityEngine;

namespace PixelCrew
{
    public class ValueComponent : MonoBehaviour
    {
        [SerializeField] private int _value;
        private Hero _hero;
        private float _positionX;

        private void Awake()
        {
            _hero = FindObjectOfType<Hero>();
        }

        public void AddCoins()
        {
            _hero.AddCoins(_value);
        }
    }
}