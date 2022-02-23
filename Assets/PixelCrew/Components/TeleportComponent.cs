using System;
using UnityEngine;

namespace PixelCrew.Components
{
    public class TeleportComponent : MonoBehaviour
    {
        [SerializeField] private Transform _destination;

        public void Teleport(GameObject target)
        {
            target.transform.position = _destination.position;
        }
    }
}