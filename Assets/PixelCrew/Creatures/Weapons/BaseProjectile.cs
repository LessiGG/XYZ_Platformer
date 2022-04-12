using UnityEngine;

namespace PixelCrew.Creatures.Weapons
{
    public class BaseProjectile : MonoBehaviour
    {
        [SerializeField] protected float _speed;
        [SerializeField] private bool _invertX;
        
        protected int Direction;

        protected Rigidbody2D Rigidbody;
        
        protected virtual void Start()
        {
            var modifier = _invertX ? -1 : 1;
            Direction = modifier * transform.lossyScale.x > 0 ? 1 : -1;
            Rigidbody = GetComponent<Rigidbody2D>();
        }
    }
}