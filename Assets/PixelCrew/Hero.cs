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
        [SerializeField] private SpawnComponent _footstepParticles;
        [SerializeField] private SpawnComponent _jumpParticles;
        [SerializeField] private SpawnComponent _fallParticles;
        [SerializeField] private ParticleSystem _dropCoinsParticle;
        
        private int _coins;
        private int _adjustedJumpsCount;
        private float _lastIdleTime;
        private bool _isGrounded;
        private bool _isJumping;

        private Vector2 _direction;
        private Rigidbody2D _rigidbody;
        private Animator _animator;

        private static readonly int IsGroundedKey = Animator.StringToHash("is-grounded");
        private static readonly int IsRunningKey = Animator.StringToHash("is-running");
        private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical-velocity");
        private static readonly int HitKey = Animator.StringToHash("hit");
        private static readonly int AfkKey = Animator.StringToHash("afk");

        private void Awake()
        {
            _lastIdleTime = Time.time;
            _adjustedJumpsCount = _extraJumpsCount;
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
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
            if (xVelocity != 0 || yVelocity != 0)
                _lastIdleTime = Time.time;

            _animator.SetFloat(VerticalVelocityKey, _rigidbody.velocity.y);
            _animator.SetBool(IsRunningKey, _direction.x != 0);
            _animator.SetBool(IsGroundedKey, _isGrounded);
            _animator.SetFloat(AfkKey, IdleCheck());
            
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
                _isJumping = true;
                yVelocity = CalculateJumpVelocity(yVelocity);
            }

            else if (_rigidbody.velocity.y > 0 && _isJumping)
                yVelocity *= 0.5f;

            return yVelocity;
        }

        private float CalculateJumpVelocity(float yVelocity)
        {
            var isFalling = _rigidbody.velocity.y <= 0.001f;
            if (!isFalling) return yVelocity;
            if (_isGrounded)
            {
                _isJumping = false;
                yVelocity += _jumpForce;
                SpawnJumpParticles();
            }
            
            else if (_extraJumpsCount > 0)
            {
                yVelocity = _jumpForce;
                SpawnJumpParticles();
                _extraJumpsCount--;
            }
            
            return yVelocity;
        }

        private void UpdateSpriteDirection()
        {
            if (_direction.x > 0)
                transform.localScale = Vector3.one;

            else if (_direction.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        private bool IsGrounded()
        {
            return _groundCheck.IsTouchingLayer;
        }

        public void AddCoins(int value)
        {
            _coins += value;
            Debug.Log($"Вы подобрали монетку номиналом {value}. Теперь у вас {_coins} монет.");
        }

        public void TakeDamage()
        {
            _isJumping = false;
            _animator.SetTrigger(HitKey);
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _damageForce);

            if (_coins > 0)
            {
                DropCoins();
            }
        }

        private void DropCoins()
        {
            var burst = _dropCoinsParticle.emission.GetBurst(0);
            var numCoinsToDispose = Mathf.Min(_coins, 5);
            _coins -= numCoinsToDispose;
            burst.count = numCoinsToDispose;
            _dropCoinsParticle.emission.SetBurst(0, burst);
            
            _dropCoinsParticle.gameObject.SetActive(true);
            _dropCoinsParticle.Play();
        }

        public void Interact()
        {
            if (!_interactionCheck.IsTouchingLayer) return;
            var interactable = _interactionCheck.Collision.GetComponent<InteractableComponent>();
            if (interactable != null)
                interactable.Interact();
        }

        private void SpawnFootsteps()
        {
            _footstepParticles.Spawn();   
        }

        private void SpawnJumpParticles()
        {
            _jumpParticles.Spawn();
        }

        private void SpawnFallParticles()
        {
            _fallParticles.Spawn();
        }
        
        private float IdleCheck()
        {
            return Time.time - _lastIdleTime;
        }
    }
}