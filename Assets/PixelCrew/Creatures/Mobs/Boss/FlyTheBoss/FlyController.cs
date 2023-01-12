using PixelCrew.Components.ColliderBased;
using UnityEngine;

namespace PixelCrew.Creatures.Mobs.Boss.FlyTheBoss
{
    public class FlyController : MonoBehaviour
    {
        [Header("Idle")] 
        [SerializeField] private float _idleMoveSpeed;
        [SerializeField] private Vector2 _idleMoveDirection;
        
        [Header("AttackUpNDown")] 
        [SerializeField] private float _attackMoveSpeed;
        [SerializeField] private Vector2 _attackMoveDirection;
        
        [Header("AttackPlayer")] 
        [SerializeField] private float _attackPlayerSpeed;
        [SerializeField] private Transform _playerTransform;
        private Vector2 _attackPosition;
        private bool _hasAttackPosition;

        [Header("Other")] 
        [SerializeField] private ColliderCheck _groundCheckUp;
        [SerializeField] private ColliderCheck _groundCheckDown;
        [SerializeField] private ColliderCheck _groundCheckWall;

        private Rigidbody2D _rigidbody;
        private Animator _animator;

        private bool _isGoingUp = true;
        private bool _facingLeft = true;
        
        private static readonly int GetSlammedKey = Animator.StringToHash("getSlammed");
        private static readonly int AttackUpNDownKey = Animator.StringToHash("attackUpNDown");
        private static readonly int AttackPlayerKey = Animator.StringToHash("attackPlayer");

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _idleMoveDirection.Normalize();
            _attackMoveDirection.Normalize();
        }

        public void IdleState()
        {
            if (_groundCheckUp.IsTouchingLayer && _isGoingUp)
                ChangeDirection();
            else if (_groundCheckDown.IsTouchingLayer && !_isGoingUp)
                ChangeDirection();
            
            if (_groundCheckWall.IsTouchingLayer)
                Flip();
            
            _rigidbody.velocity = _idleMoveSpeed * _idleMoveDirection;
        }
        public void AttackUpNDownState()
        {
            if (_groundCheckUp.IsTouchingLayer && _isGoingUp)
                ChangeDirection();
            else if (_groundCheckDown.IsTouchingLayer && !_isGoingUp)
                ChangeDirection();
            
            if (_groundCheckWall.IsTouchingLayer)
                Flip();
            
            _rigidbody.velocity = _attackMoveSpeed * _attackMoveDirection;
        }

        public void AttackPlayer()
        {
            if (!_hasAttackPosition)
            {
                _attackPosition = _playerTransform.position - transform.position;
                _attackPosition.Normalize();
                _hasAttackPosition = true;
            }
            
            if (_hasAttackPosition)
                _rigidbody.velocity = _attackPosition * _attackPlayerSpeed;

            if (_groundCheckDown.IsTouchingLayer || _groundCheckWall.IsTouchingLayer)
            {
                _rigidbody.velocity = Vector2.zero;
                _hasAttackPosition = false;
                _animator.SetTrigger(GetSlammedKey);
            }
        }

        private void ChangeDirection()
        {
            _isGoingUp = !_isGoingUp;
            _idleMoveDirection.y *= -1;
            _attackMoveDirection.y *= -1;
        }

        private void Flip()
        {
            _facingLeft = !_facingLeft;
            _idleMoveDirection.x *= -1;
            _attackMoveDirection.x *= -1;
            transform.Rotate(0, 180, 0);
        }

        private void FlipTowardsPlayer()
        {
            var playerDirection = _playerTransform.position.x - transform.position.x;

            if (playerDirection > 0 && _facingLeft)
                Flip();
            else if (playerDirection < 0 && !_facingLeft)
                Flip();
        }

        public void PickRandomState()
        {
            var randomState = Random.Range(0, 2);
            switch (randomState)
            {
                case 0:
                    _animator.SetTrigger(AttackUpNDownKey);
                    break;
                case 1:
                    _animator.SetTrigger(AttackPlayerKey);
                    break;
            }
        }

        public void StopMoving()
        {
            _rigidbody.velocity = Vector2.zero;
        }
    }
}