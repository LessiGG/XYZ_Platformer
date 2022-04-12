using PixelCrew.Components.ColliderBased;
using UnityEngine;

namespace PixelCrew.Components.Collectables
{
    public class PhysicalCoin : MonoBehaviour
    {
        [SerializeField] private LayerCheck _groundCheck;
 
        private CircleCollider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<CircleCollider2D>();
        }
        private void Update()
        {
            _collider.enabled = _groundCheck.IsTouchingLayer;
        }
    }
}