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
        [SerializeField] private float _damageVelocity;
        
        [SerializeField] private CoolDown _attackCooldown;
        
        [Space] [Header("Main Checkers")]
        [SerializeField] protected ColliderCheck _groundCheck;
        [SerializeField] private CheckCircleOverlap[] _attackRange;
        [SerializeField] protected SpawnListComponent _particles;
        
        private bool _isJumping;
        
        protected bool IsGrounded;
        protected Vector2 Direction;
        protected Rigidbody2D Rigidbody;
        protected PlaySoundsComponent Sounds;
        protected Animator Animator;

        private static readonly int IsGroundedKey = Animator.StringToHash("is-grounded");
        private static readonly int IsRunningKey = Animator.StringToHash("is-running");
        private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical-velocity");
        private static readonly int HitKey = Animator.StringToHash("hit");
        private static readonly int AttackKey = Animator.StringToHash("attack");

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Sounds = GetComponent<PlaySoundsComponent>();
            Animator = GetComponent<Animator>();
        }

        public void SetDirection(Vector2 direction)
        {
            Direction = direction;
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
            Animator.SetBool(IsRunningKey, Direction.x != 0);
            Animator.SetBool(IsGroundedKey, IsGrounded);
            
            UpdateSpriteDirection(Direction);
        }

        protected virtual float CalculateYVelocity()
        {
            var yVelocity = Rigidbody.velocity.y;
            var isJumpPressing = Direction.y > 0;

            if (IsGrounded)
            {
                _isJumping = false;
            }
            
            if (isJumpPressing)
            {
                _isJumping = true;
                
                var isFalling = Rigidbody.velocity.y <= 0.001f;
                yVelocity = isFalling ? CalculateJumpVelocity(yVelocity) : yVelocity;
            }

            else if (Rigidbody.velocity.y > 0 && _isJumping)
                yVelocity *= 0.5f;

            return yVelocity;
        }
        
        protected virtual float CalculateXVelocity()
        {
            return  Direction.x * _speed;;
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

        public virtual void TakeDamage()
        {
            _isJumping = false;
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
        
        public void OnDoAttack()
        {
            foreach (var checkCircleOverlap in _attackRange)
            {
               checkCircleOverlap.Check(); 
            }
        }
    }
}