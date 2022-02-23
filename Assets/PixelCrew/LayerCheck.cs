using System;
using UnityEngine;

namespace PixelCrew
{
    [RequireComponent(typeof(Collider2D))]
    public class LayerCheck : MonoBehaviour
    {
        [SerializeField] private LayerMask _checkingLayer;
        private Collider2D _collider;

        public bool IsTouchingLayer;
        [HideInInspector] public GameObject Collision;
        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            IsTouchingLayer = _collider.IsTouchingLayers(_checkingLayer);
            Collision = other.gameObject;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            IsTouchingLayer = _collider.IsTouchingLayers(_checkingLayer);
            Collision = other.gameObject;
        }
    }
}