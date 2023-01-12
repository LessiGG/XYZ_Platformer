using Cinemachine;
using PixelCrew.Creatures.Hero;
using UnityEngine;

namespace PixelCrew.Components.CutScenes
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class SetFollowComponent : MonoBehaviour
    {
        private CinemachineVirtualCamera _vCamera;

        private void Start()
        {
            _vCamera = GetComponent<CinemachineVirtualCamera>();
            _vCamera.Follow = FindObjectOfType<Hero>().transform;
        }

        public void SetFollow(Transform followTarget)
        {
            _vCamera.Follow = followTarget;
        }
    }
}