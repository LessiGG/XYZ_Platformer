using UnityEngine;
using Random = UnityEngine.Random;

namespace PixelCrew.Components.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class VerticalLevitationComponent : MonoBehaviour
    {
        [SerializeField] private float _frequency;
        [SerializeField] private float _amplitude;
        [SerializeField] private bool _randomize;

        private float _originalY;
        private float _seed;
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _originalY = _rigidbody.position.y;

            if (_randomize)
                _seed = Random.value * Mathf.PI * 2;
        }

        private void Update()
        {
            var position = _rigidbody.position;
            position.y = _originalY + Mathf.Sin(_seed + Time.time * _frequency) * _amplitude;
            _rigidbody.MovePosition(position);
        }
    }
}