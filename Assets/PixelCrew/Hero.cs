using PixelCrew.Components;
using UnityEngine;

namespace PixelCrew
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Hero : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _damageForce;
        [SerializeField] private int _extraJumpsCount;
        [SerializeField] private LayerCheck _groundCheck;
        [SerializeField] private LayerCheck _interactionCheck;
        public int _coinsAmount;
        private int _adjustedJumpsCount;
        private bool _isGrounded;

        private Vector2 _direction;
        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private SpriteRenderer _renderer;

        private static readonly int IsGroundedKey = Animator.StringToHash("is-grounded");
        private static readonly int IsRunningKey = Animator.StringToHash("is-running");
        private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical-velocity");
        private static readonly int HitKey = Animator.StringToHash("hit");

        private void Awake()
        {
            _adjustedJumpsCount = _extraJumpsCount;
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }

        private void Update()
        {
            _isGrounded = IsGrounded();
            if (_isGrounded)
                _extraJumpsCount = _adjustedJumpsCount;
        }

        private void FixedUpdate()
        {
            var xVelocity = CalculateXVelocity();
            var yVelocity = CalculateYVelocity();
            _rigidbody.velocity = new Vector2(xVelocity, yVelocity);

            _animator.SetFloat(VerticalVelocityKey, _rigidbody.velocity.y);
            _animator.SetBool(IsRunningKey, _direction.x != 0);
            _animator.SetBool(IsGroundedKey, _isGrounded);
            
            UpdateSpriteDirection();
        }

        private float CalculateXVelocity()
        {
            var xVelocity = _direction.x * _speed;

            return xVelocity;
        }

        private float CalculateYVelocity()
        {
            var yVelocity = _rigidbody.velocity.y;
            var isJumpPressing = _direction.y > 0;
            
            if (isJumpPressing)
            {
                yVelocity = CalculateJumpVelocity(yVelocity);
            }

            else if (_rigidbody.velocity.y > 0)
                yVelocity *= 0.5f;

            return yVelocity;
        }

        private float CalculateJumpVelocity(float yVelocity)
        {
            var isFalling = _rigidbody.velocity.y <= 0.001f;
            if (!isFalling) return yVelocity;
            if (_isGrounded)
            {
                yVelocity += _jumpForce;
            }
            
            else if (_extraJumpsCount > 0)
            {
                yVelocity = _jumpForce;
                _extraJumpsCount--;
            }
            
            return yVelocity;
        }

        private void UpdateSpriteDirection()
        {
            if (_direction.x > 0)
                _renderer.flipX = false;
            if (_direction.x < 0)
                _renderer.flipX = true;
        }

        private bool IsGrounded()
        {
            return _groundCheck.IsTouchingLayer;
        }

        public void AddCoins(int value)
        {
            _coinsAmount += value;
            Debug.Log($"Вы подобрали монетку номиналом {value}. Теперь у вас {_coinsAmount} монет.");
        }

        public void TakeDamage()
        {
            _animator.SetTrigger(HitKey);
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _damageForce);
        }

        public void Interact()
        {
            if (!_interactionCheck.IsTouchingLayer) return;
            var interactable = _interactionCheck.Collision.GetComponent<InteractableComponent>();
            if (interactable != null)
                interactable.Interact();
        }
    }
}