using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PixelCrew.Utils
{
    public class CheatController : MonoBehaviour
    {
        [SerializeField] private float _inputLivingTime;
        [SerializeField] private CheatItem[] _cheats;
        
        private string _currentInput;
        private float _inputTime;
        
        private void Awake()
        {
            Keyboard.current.onTextInput += OnTextInput;
        }

        private void OnDestroy()
        {
            Keyboard.current.onTextInput -= OnTextInput;
        }

        private void Update()
        {
            if (_inputTime < 0)
                _currentInput = string.Empty;
            else
                _inputTime -= Time.deltaTime;
        }

        private void OnTextInput(char inputChar)
        {
            _currentInput += inputChar;
            _inputTime = _inputLivingTime;
            FindAnyCheats();
        }

        private void FindAnyCheats()
        {
            foreach (var cheatItem in _cheats)
            {
                if (!_currentInput.Contains(cheatItem.Name)) continue;
                cheatItem.Action?.Invoke();
                _currentInput = string.Empty;
            }
        }
    }

    [Serializable]
    public class CheatItem
    {
        public string Name;
        public UnityEvent Action;
    }
}