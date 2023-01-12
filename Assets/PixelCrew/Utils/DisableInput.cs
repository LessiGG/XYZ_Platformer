using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelCrew.Utils
{
    public static class DisableInput
    {
        public static void SetInput(bool isEnabled)
        {
            var playerInput = Object.FindObjectOfType<PlayerInput>();

            playerInput.enabled = isEnabled;
        }
    }
}