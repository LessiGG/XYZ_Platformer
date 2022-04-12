using PixelCrew.Components.ColliderBased;
using PixelCrew.Components.GoBased;
using PixelCrew.Utils;
using UnityEngine;

namespace PixelCrew.Creatures.Mobs
{
    public class SeashellAI : MonoBehaviour
    {
        [SerializeField] private ColliderCheck _vision;
        
        [Space] [Header("Melee")]
        [SerializeField] private ColliderCheck _meleeCanAttack;
        [SerializeField] private CheckCircleOverlap _meleeAttack;
        [SerializeField] private CoolDown _meleeCooldown;
        
        [Space] [Header("Range")]
        [SerializeField] private SpawnComponent _rangeAttack;
        [SerializeField] private CoolDown _rangeCooldown;
        
        private Animator _animator;
        
        private static readonly int MeleeKey = Animator.StringToHash("melee");
        private static readonly int RangeKey = Animator.StringToHash("range");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!_vision.IsTouchingLayer) return;
            
            if (_meleeCanAttack.IsTouchingLayer)
            {
                if (_meleeCooldown.IsReady)
                    MeleeAttack();
                return;
            }

            if (_rangeCooldown.IsReady)
                RangeAttack();
        }

        private void RangeAttack()
        {
            _rangeCooldown.Reset();
            _animator.SetTrigger(RangeKey);
        }

        private void MeleeAttack()
        {
            _meleeCooldown.Reset();
            _animator.SetTrigger(MeleeKey);
        }

        public void OnMeleeAttack()
        {
            _meleeAttack.Check();
        }
        
        public void OnRangeAttack()
        {
            _rangeAttack.Spawn();
        }
    }
}