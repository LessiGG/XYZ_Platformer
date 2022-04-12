using System.Collections;
using PixelCrew.Components;
using PixelCrew.Components.ColliderBased;
using PixelCrew.Components.GoBased;
using PixelCrew.Components.Health;
using PixelCrew.Creatures.Mobs;
using PixelCrew.Model;
using PixelCrew.Model.Data;
using PixelCrew.Model.Definitions;
using PixelCrew.UI;
using PixelCrew.Utils;
using UnityEditor.Animations;
using UnityEngine;

namespace PixelCrew.Creatures.Hero
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Hero : Creature, ICanAddInInventory
    {
        [Space] [Header("Extra params")]
        [SerializeField] private float _slamDownVelocity;
        //[SerializeField] private float _fallDamageVelocity;
        [SerializeField] private float _slippingVelocity;
        [SerializeField] private int _extraJumpsCount;
        
        [SerializeField] private CoolDown _throwCooldown;

        [Space] [Header("Super throw")] 
        [SerializeField] private CoolDown _superThrowCooldown;
        [SerializeField] private int _superThrowParticles;
        [SerializeField] private float _superThrowDelay;
        [SerializeField] private SpawnComponent _throwSpawner;
        
        [Space] [Header("Extra checkers")]
        [SerializeField] private ColliderCheck _wallCheck;
        [SerializeField] private CheckCircleOverlap _interactionRange;

        [Space] [Header("AnimatorControllers")] 
        [SerializeField] private AnimatorController _unarmed;
        [SerializeField] private AnimatorController _armed;

        [Space] [Header("Particles")] 
        [SerializeField] private ProbabilityDropComponent _dropCoins;
        
        private static readonly int ThrowKey = Animator.StringToHash("throw");
        private static readonly int WallKey = Animator.StringToHash("is-on-wall");
        
        private int _adjustedJumpsCount;
        private float _defaultGravityScale;
        private bool _isOnWall;
        private bool _isArmed;
        private bool _superThrow;
        [SerializeField] private float _dashImpulse;
        
        private HealthComponent _health;
        private GameSession _session;
        private DisplayWindow _displayWindow;

        private const string SwordId = "Sword";

        private int SwordsCount => _session.Data.Inventory.GetCount(SwordId);
        private int CoinsCount => _session.Data.Inventory.GetCount("Coin");

        private string SelectedItemId => _session.QuickInventory.SelectedItem.Id;

        private bool CanThrow
        {
            get
            {
                if (SelectedItemId == SwordId)
                    return SwordsCount > 1;
                
                var def = DefsFacade.I.Items.Get(SelectedItemId);
                return def.HasTag(ItemTag.Throwable);
            }
        }
        
        protected override void Awake()
        {
            base.Awake();
            _adjustedJumpsCount = _extraJumpsCount;
            _defaultGravityScale = Rigidbody.gravityScale;
        }

        private void Start()
        {
            _displayWindow = GetComponent<DisplayWindow>();
            _session = FindObjectOfType<GameSession>();
            _health = GetComponent<HealthComponent>();
            _session.Data.Inventory.OnChanged += OnInventoryChanged;
            
            _health.SetHealth(_session.Data.Hp.Value);
            UpdateHeroWeapon();
        }

        private void OnDestroy()
        {
            _session.Data.Inventory.OnChanged -= OnInventoryChanged;
        }

        private void OnInventoryChanged(string id, int value)
        {
            Debug.Log($"Inventory changed: {id} : {value}");
            if (id == SwordId)
                UpdateHeroWeapon();
        }
        public void OnHealthChanged(int currentHealth)
        {
            _session.Data.Hp.Value = currentHealth;
        }

        protected override void Update()
        {
            base.Update();

            var moveToSameDirection = Direction.x * transform.lossyScale.x > 0;
            
            if (IsGrounded)
            {
                _extraJumpsCount = _adjustedJumpsCount;
            }

            if (_wallCheck.IsTouchingLayer && moveToSameDirection)
            {
                _isOnWall = true;
                Rigidbody.gravityScale = 0;
            }
            else
            {
                _isOnWall = false;
                Rigidbody.gravityScale = _defaultGravityScale;
            }
            
            Animator.SetBool(WallKey, _isOnWall);
        }
        protected override float CalculateYVelocity()
        {
            var isJumpPressing = Direction.y > 0;

            if (!isJumpPressing && _isOnWall)
            {
                return -_slippingVelocity;
            }
            
            return base.CalculateYVelocity();
        }

        protected override float CalculateJumpVelocity(float yVelocity)
        {
            if (!IsGrounded && _extraJumpsCount > 0 && !_isOnWall)
            {
                DoJumpVfx();
                _extraJumpsCount--;
                return _jumpImpulse;
            }
            
            return base.CalculateJumpVelocity(yVelocity);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.IsInLayer(_groundCheck.CheckingLayer)) return;
            
            var contact = other.contacts[0];
            if (contact.relativeVelocity.y >= _slamDownVelocity)
            {
                _particles.Spawn("SlamDown");
            }
            
            //Урон от падения
            /*if (!(contact.relativeVelocity.y >= _fallDamageVelocity)) return;
            _health.ModifyHealth(-1);*/
        }

        public void AddInInventory(string id, int value)
        {
            _session.Data.Inventory.Add(id, value);
        }

        public override void TakeDamage()
        {
            base.TakeDamage();
            
            if (CoinsCount > 0)
                DropCoins();
        }

        private void DropCoins()
        {
            var numCoinsToDispose = Mathf.Min(CoinsCount, 5);
            _session.Data.Inventory.Remove("Coin", numCoinsToDispose);

            _dropCoins.SetCount(numCoinsToDispose);
            _dropCoins.CalculateDrop();
        }

        public void Interact()
        {
            _interactionRange.Check();
        }
        
        public override void Attack()
        {
            if (!_isArmed) return;
            base.Attack();
        }

        private void UpdateHeroWeapon()
        {
            _isArmed = SwordsCount > 0;
            Animator.runtimeAnimatorController = _isArmed ? _armed : _unarmed;
        }

        public void OnDoThrow()
        {
            if (_superThrow)
            {
                var throwableCount = _session.Data.Inventory.GetCount(SelectedItemId);
                var possibleCount = SelectedItemId == SwordId ? throwableCount - 1 : throwableCount;
                
                var numThrows = Mathf.Min(_superThrowParticles, possibleCount);
                StartCoroutine(DoSuperThrow(numThrows));
            }
            else
                Throw();

            _superThrow = false;
        }

        private IEnumerator DoSuperThrow(int numThrows)
        {
            for (var i = 0; i < numThrows; i++)
            {
                Throw();
                yield return new WaitForSeconds(_superThrowDelay);
            }
        }

        private void Throw()
        {
            Sounds.Play("Throw");

            var throwableId = _session.QuickInventory.SelectedItem.Id;
            var throwableDef = DefsFacade.I.Throwables.Get(throwableId);
            _throwSpawner.SetPrefab(throwableDef.Projectile);
            _throwSpawner.Spawn();
            
            _session.Data.Inventory.Remove(throwableId, 1);
        }

        public void StartThrowing()
        {
            _superThrowCooldown.Reset();
        }

        public void PerformThrowing()
        {
            if (!CanThrow || !_throwCooldown.IsReady) return;

            if (_superThrowCooldown.IsReady) _superThrow = true;
            
            Animator.SetTrigger(ThrowKey);
            _throwCooldown.Reset();
        }

        public void PauseGame()
        {
            _displayWindow.OnShowWindow();
        }

        public void Dash()
        {
            var newPosition = Rigidbody.position + new Vector2(_dashImpulse * transform.localScale.x, 0);
            Rigidbody.MovePosition(newPosition);
        }

        public void NextItem()
        {
            _session.QuickInventory.SetNextItem();
        }

        public void UseItem()
        {
            var item = _session.QuickInventory.SelectedItem.Id;
            var itemDef = DefsFacade.I.Items.Get(item);
            
            if (itemDef.HasTag(ItemTag.Usable) && !itemDef.HasTag(ItemTag.Throwable))
                _session.Data.Inventory.Remove(item, 1);
        }
    }
}