using PixelCrew.Components.Audio;
using PixelCrew.Components.ColliderBased;
using PixelCrew.Components.GoBased;
using PixelCrew.Utils;
using UnityEngine;

namespace PixelCrew.Creatures.Mobs
{
    public class Creature : MonoBehaviour
    {
        [Space] [Header("Params")]
        [SerializeField] private bool _invertScale = false;
        [SerializeField] protected float _speed;
        [SerializeField] protected float _jumpImpulse;
        [SerializeField] private float _heavyLandingVelocity;
        [SerializeField] private float _damageVelocity;
        
        [SerializeField] private CoolDown _attackCooldown;
        
        [Space] [Header("Main Checkers")]
        [SerializeField] protected ColliderCheck _groundCheck;
        [SerializeField] protected CheckCircleOverlap _attackRange;
        [SerializeField] protected SpawnListComponent _particles;
        [SerializeField] private LayerMask _groundLayer;
        
        protected bool IsJumping;
        protected bool IsGrounded;
        private Vector2 _direction;
        protected Rigidbody2D Rigidbody;
        protected PlaySoundsComponent Sounds;
        protected Animator Animator;

        private static readonly int IsGroundedKey = Animator.StringToHash("is-grounded");
        private static readonly int IsRunningKey = Animator.StringToHash("is-running");
        private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical-velocity");
        private static readonly int HitKey = Animator.StringToHash("hit");
        private static readonly int AttackKey = Animator.StringToHash("attack");

        public Vector2 Direction
        {
            get => _direction;
            set => _direction = value;
        }
        
        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Sounds = GetComponent<PlaySoundsComponent>();
            Animator = GetComponent<Animator>();
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }

        protected virtual void Update()
        {
            IsGrounded = _groundCheck.IsTouchingLayer;
        }

        protected virtual void FixedUpdate()
        {
            var xVelocity = CalculateXVelocity();
            var yVelocity = CalculateYVelocity();
            Rigidbody.velocity = new Vector2(xVelocity, yVelocity);

            Animator.SetFloat(VerticalVelocityKey, Rigidbody.velocity.y);
            Animator.SetBool(IsRunningKey, _direction.x != 0);
            Animator.SetBool(IsGroundedKey, IsGrounded);
            
            UpdateSpriteDirection(_direction);
        }

        protected virtual float CalculateYVelocity()
        {
            var yVelocity = Rigidbody.velocity.y;
            var isJumpPressing = _direction.y > 0;

            if (IsGrounded)
            {
                IsJumping = false;
            }
            
            if (isJumpPressing)
            {
                IsJumping = true;
                
                var isFalling = Rigidbody.velocity.y <= 0.001f;
                yVelocity = isFalling ? CalculateJumpVelocity(yVelocity) : yVelocity;
            }

            else if (Rigidbody.velocity.y > 0 && IsJumping)
                yVelocity *= 0.5f;

            return yVelocity;
        }
        
        protected virtual float CalculateXVelocity()
        {
            return _direction.x * CalculateSpeed();
        }

        protected virtual float CalculateSpeed()
        {
            return _speed;
        }

        protected virtual float CalculateJumpVelocity(float yVelocity)
        {
            if (!IsGrounded) return yVelocity;
            
            yVelocity += _jumpImpulse;
            DoJumpVfx();

            return yVelocity;
        }

        protected void DoJumpVfx()
        {
            _particles.Spawn("Jump");
            Sounds.Play("Jump");
        }

        public void UpdateSpriteDirection(Vector2 direction)
        {
            var scaleModifier = _invertScale ? -1 : 1;
            
            if (direction.x > 0)
                transform.localScale = new Vector3(scaleModifier, 1, 1);

            else if (direction.x < 0)
                transform.localScale = new Vector3(-1 * scaleModifier, 1, 1);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.IsInLayer(_groundLayer))
            {
                var contact = collision.contacts[0];
                if (contact.relativeVelocity.y >= _heavyLandingVelocity)
                {
                    _particles.Spawn("HeavyLanding");
                }
            }
        }

        public virtual void TakeDamage()
        {
            IsJumping = false;
            Animator.SetTrigger(HitKey);
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, _damageVelocity);
        }
        
        public virtual void Attack()
        {
            if (!_attackCooldown.IsReady) return;
            Animator.SetTrigger(AttackKey);
            _attackCooldown.Reset();
            _particles.Spawn("Attack");
            Sounds.Play("Attack");
        }
        
        public virtual void OnDoAttack()
        {
            _attackRange.Check(); 
        }
    }
}