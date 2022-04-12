using PixelCrew.Utils;
using UnityEngine;

namespace PixelCrew.Components.ColliderBased
{
    public class EnterTriggerComponent : MonoBehaviour
    {
        [SerializeField] private string[] _tag;
        [SerializeField] private LayerMask _layer = ~0;
        [SerializeField] private EnterEvent _action;
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            foreach (var tagg in _tag)
            {
                if (collision.gameObject.CompareTag(tagg))
                    _action?.Invoke(collision.gameObject);
            }
            
            if (!collision.gameObject.IsInLayer(_layer)) return;
                
            _action?.Invoke(collision.gameObject);
        }
    }
}