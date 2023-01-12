using Cinemachine;
using PixelCrew.Creatures.Hero;
using PixelCrew.Utils;
using UnityEngine;

namespace PixelCrew.Components.CutScenes
{
    public class CameraMovementComponent : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private bool _move;
        private CinemachineVirtualCamera _camera;
        private Transform _hero;

        private void Start()
        {
            _camera = GetComponent<CinemachineVirtualCamera>();
            _hero = FindObjectOfType<Hero>().transform;
        }

        private void Update()
        {
            SetCameraFollow();

            if (_move)
                MoveUp();
        }

        private void MoveUp()
        {
            _camera.transform.position = new Vector3(_camera.transform.position.x,
                _camera.transform.position.y + _speed * Time.deltaTime, 0);
        }

        private void SetCameraFollow()
        {
            _camera.Follow = _move ? null : _hero;
        }
    }
}