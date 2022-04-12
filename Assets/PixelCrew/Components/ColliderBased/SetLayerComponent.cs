using UnityEngine;

namespace PixelCrew.Components.ColliderBased
{
    public class SetLayerComponent : MonoBehaviour
    {
        [SerializeField] private int _layer;

        public void SetLayer()
        {
            gameObject.layer = _layer;
        }
    }
}