using PixelCrew.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Components.Interactions
{
    public class InteractableComponent : MonoBehaviour
    {
        [SerializeField] private UnityEvent _action;

        public void Interact()
        {
            _action?.Invoke();
        }
    
#if UNITY_EDITOR
        private GameObject[] _gameObjects;

        private void OnValidate()
        {
            _gameObjects = _action.ToGameObjects();
        }

        private void OnDrawGizmos()
        {
            foreach (var go in _gameObjects)
            {
                if (go == null) continue;
                
                Debug.DrawLine(transform.position, go.transform.position, Color.green);
            }
        }
#endif
    }
}