using System;
using UnityEngine;

namespace PixelCrew.Components.Movement
{
    public class CircularMovementComponent : MonoBehaviour
    {
        [SerializeField] private float _radius;
        [SerializeField] private float _speed;
        
        private Rigidbody2D[] _bodies;
        private Vector2[] _positions;
        private float _time; 
        private void Awake()
        {
            UpdateContent();
        }

        private void UpdateContent()
        {
            _bodies = GetComponentsInChildren<Rigidbody2D>();
            _positions = new Vector2[_bodies.Length];
        }

        private void Update()
        {
            CalculatePositions();
            var isAnyCoinLeft = false;
            
            for (var i = 0; i < _bodies.Length; i++)
            {
                if (!_bodies[i]) continue;
                _bodies[i].MovePosition(_positions[i]);
                isAnyCoinLeft = true;
            }

            if (!isAnyCoinLeft)
            {
                enabled = false;
                Destroy(gameObject, 1f);
            }
            
            _time += Time.deltaTime;
        }

        private void CalculatePositions()
        { 
            var step = 2 * Mathf.PI / _bodies.Length;
            Vector2 containerPosition = transform.position;

            for (var i = 0; i < _bodies.Length; i++)
            {
                var angle = step * i;
                var position = new Vector2(
                    Mathf.Cos(angle + _time * _speed) * _radius,
                    Mathf.Sin(angle + _time * _speed) * _radius
                );
                _positions[i] = containerPosition + position;
            }
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateContent();
            CalculatePositions();
            for (var i = 0; i < _bodies.Length; i++)
            {
                _bodies[i].transform.position = _positions[i];
            }
        }

        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, _radius);
        }
#endif
    }
}