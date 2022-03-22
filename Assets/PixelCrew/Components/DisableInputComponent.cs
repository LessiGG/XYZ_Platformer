using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelCrew.Components
{
    public class DisableInputComponent : MonoBehaviour
    {
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = FindObjectOfType<PlayerInput>();
        }

        public void SwitchInput()
        {
            if (_playerInput != null)
                _playerInput.enabled = !_playerInput.enabled;
        }
    }
}