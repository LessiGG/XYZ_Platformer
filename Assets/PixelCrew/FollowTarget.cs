using UnityEngine;

namespace PixelCrew
{
    public class FollowTarget : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _damping;

        private void LateUpdate()
        {
            var position = transform.position;
            var targetPosition = _target.position;
            var destination = new Vector3(targetPosition.x, targetPosition.y, position.z);
            position = Vector3.Lerp(position, destination, Time.deltaTime * _damping);
            transform.position = position;
        }
    }
}