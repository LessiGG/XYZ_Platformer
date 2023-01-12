using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelCrew.Creatures.Hero
{
    public class HeroInputReader : MonoBehaviour
    {
        [SerializeField] private Hero _hero;
        public void OnMovement(InputAction.CallbackContext context)
        {
            _hero.Direction = context.ReadValue<Vector2>();
            if (_hero.Direction.y == 0) _hero.IsJumpButtonPressed = false;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
                _hero.Interact();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                _hero.Attack();
        }

        public void OnUse(InputAction.CallbackContext context)
        {
            if (context.started)
                _hero.StartThrowing();
            
            if (context.canceled)
                _hero.UseItem();
        }

        public void OnNextItem(InputAction.CallbackContext context)
        {
            if (context.performed)
                _hero.NextItem();
        }

        public void OnPauseGame(InputAction.CallbackContext context)
        {
            if (context.performed)
                _hero.PauseGame();
        }

        public void OnUsePerk(InputAction.CallbackContext context)
        {
            if (context.performed)
                _hero.UsePerk();
        }

        public void OnToggleFlashlight(InputAction.CallbackContext context)
        {
            if (context.performed)
                _hero.ToggleFlashlight();
        }

        public void OnDropDown(InputAction.CallbackContext context)
        {
            if (context.performed)
                _hero.DropDown();
        }

        public void OnClimb(InputAction.CallbackContext context)
        {
            if (context.performed)
                _hero.Climb();
        }
    }
}