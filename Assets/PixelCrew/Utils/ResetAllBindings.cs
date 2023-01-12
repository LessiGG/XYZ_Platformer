using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelCrew.Utils
{
    public class ResetAllBindings : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActions;

        public void ResetBindings()
        {
            foreach (var map in _inputActions.actionMaps)
            {
                map.RemoveAllBindingOverrides();
            }
            PlayerPrefs.DeleteKey("rebinds");
        }
    }
}