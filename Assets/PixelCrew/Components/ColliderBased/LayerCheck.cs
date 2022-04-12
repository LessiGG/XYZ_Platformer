using UnityEngine;

namespace PixelCrew.Components.ColliderBased
{
    public class LayerCheck : MonoBehaviour
    {
        [SerializeField] protected LayerMask _checkingLayer;
        [SerializeField] protected bool _isTouchingLayer;

        public LayerMask CheckingLayer => _checkingLayer;
        public bool IsTouchingLayer => _isTouchingLayer;
    }
}