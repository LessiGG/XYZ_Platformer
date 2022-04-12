using UnityEngine;

namespace PixelCrew.UI
{
    public class DisplayWindow : MonoBehaviour
    {
        [SerializeField] private string _path;

        private GameObject _window;
        private Canvas _canvas;

        private void Start()
        {
            _window = Resources.Load<GameObject>(_path);
            _canvas = FindObjectOfType<Canvas>();
        }

        public void OnShowWindow()
        {
            Instantiate(_window, _canvas.transform);
        }
    }
}