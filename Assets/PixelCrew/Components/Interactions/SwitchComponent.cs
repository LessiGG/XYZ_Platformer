using UnityEngine;

namespace PixelCrew.Components.Interactions
{
    public class SwitchComponent : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private bool _state;
        [SerializeField] private string _animationKey;
        [SerializeField] private bool _switchOnStart;

        private void Start()
        {
            if (_switchOnStart)
                _animator.SetBool(_animationKey, _state);
        }

        public void Switch()
        {
            _state = !_state;
            _animator.SetBool(_animationKey, _state);
        }

        [ContextMenu("Switch")]
        public void ContextSwitch()
        {
            Switch();
        }
    }
}