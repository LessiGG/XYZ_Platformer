using UnityEngine;

namespace Scripts
{
    public class ValueComponent : MonoBehaviour
    {
        [SerializeField] private int _value;
        private Hero _hero;

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