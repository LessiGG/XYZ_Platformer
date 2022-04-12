using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelCrew.Creatures.Hero
{
    public class DisableInputComponent : MonoBehaviour
    {
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = FindObjectOfType<PlayerInput>();
        }

        public void DisableInput()
        {
            if (_playerInput != null)
                _playerInput.enabled = false;
        }
        
        public void EnableInput()
        {
            if (_playerInput != null)
                _playerInput.enabled = true;
        }
    }
}