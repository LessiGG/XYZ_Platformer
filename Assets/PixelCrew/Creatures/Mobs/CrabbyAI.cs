﻿using System.Collections;
using PixelCrew.Components.ColliderBased;
using PixelCrew.Components.GoBased;
using PixelCrew.Creatures.Mobs.Patrolling;
using UnityEngine;

namespace PixelCrew.Creatures.Mobs
{
    public class CrabbyAI : MonoBehaviour
    {
        [SerializeField] private ColliderCheck _vision;
        [SerializeField] private ColliderCheck _canAttackLeft;
        [SerializeField] private ColliderCheck _canAttackRight;
        
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private float _exclamationDelay = 0.5f;
        [SerializeField] private float _questionDelay = 0.5f;

        private IEnumerator _current;
        private GameObject _target;

        private SpawnListComponent _particles;
        private Creature _creature;
        private Animator _animator;
        private Patrol _patrol;

        private bool _isDead;
        
        private static readonly int IsDeadKey = Animator.StringToHash("is-dead");

        private void Awake()
        {
            _particles = GetComponent<SpawnListComponent>();
            _creature = GetComponent<Creature>();
            _animator = GetComponent<Animator>();
            _patrol = GetComponent<Patrol>();
        }

        private void Start()
        {
            StartState(_patrol.DoPatrol());
        }

        public void OnHeroInVision(GameObject goInVision)
        {
            if (_isDead) return;
            
            _target = goInVision;
            
            StartState(AgroToHero());
        }

        private IEnumerator AgroToHero()
        {
            LookAtHero();
            
            _particles.Spawn("Exclamation");

            yield return new WaitForSeconds(_exclamationDelay);

            StartState(GoToHero());
        }

        private void LookAtHero()
        {
            var direction = GetDirectionToTarget();
            _creature.SetDirection(Vector2.zero);
            _creature.UpdateSpriteDirection(direction);
        }

        private IEnumerator GoToHero()
        {
            while (_vision.IsTouchingLayer)
            {
                if (_canAttackLeft.IsTouchingLayer || _canAttackRight.IsTouchingLayer)
                {
                    StartState(Attack());
                }
                else
                {
                    SetDirectionToTarget();
                }
                
                yield return null;
            }
            
            _creature.SetDirection(Vector2.zero);
            _particles.Spawn("Question");
            yield return new WaitForSeconds(_questionDelay);
            
            StartState(_patrol.DoPatrol());
        }

        private IEnumerator Attack()
        {
            while (_canAttackLeft.IsTouchingLayer || _canAttackRight.IsTouchingLayer)
            {
                _creature.Attack();
                yield return new WaitForSeconds(_attackCooldown);
            }
            
            StartState(GoToHero());
        }

        private void SetDirectionToTarget()
        {
            var direction = GetDirectionToTarget();
            _creature.SetDirection(direction);
        }

        private Vector2 GetDirectionToTarget()
        {
            var direction = _target.transform.position - transform.position;
            direction.y = 0f;
            return direction.normalized;
        }

        private void StartState(IEnumerator coroutine)
        {
            _creature.SetDirection(Vector2.zero);
            
            if (_current != null)
                StopCoroutine(_current);
            
            _current = coroutine;
            StartCoroutine(coroutine);
        }

        public void OnDie()
        {
            _creature.SetDirection(Vector2.zero);
            _isDead = true;
            _animator.SetBool(IsDeadKey, true);
            
            if (_current != null)
                StopCoroutine(_current);
        }
    }
}