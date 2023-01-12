using System.Collections;
using PixelCrew.Components;
using PixelCrew.Components.ColliderBased;
using PixelCrew.Components.GoBased;
using PixelCrew.Components.Health;
using PixelCrew.Creatures.Hero.Features;
using PixelCrew.Creatures.Mobs;
using PixelCrew.Effects.CameraRelated;
using PixelCrew.Model;
using PixelCrew.Model.Data;
using PixelCrew.Model.Definitions;
using PixelCrew.Model.Definitions.Player;
using PixelCrew.Model.Definitions.Repositories;
using PixelCrew.Model.Definitions.Repositories.Items;
using PixelCrew.Utils;
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
        [SerializeField] private float _slippingVelocity;
        [SerializeField] private CoolDown _throwCooldown;
        [SerializeField] private ShieldComponent _shield;
        [SerializeField] private FlashlightComponent _flashlight;

        [Space] [Header("Super throw")] 
        [SerializeField] private CoolDown _superThrowCooldown;
        [SerializeField] private int _superThrowParticles;
        [SerializeField] private float _superThrowDelay;
        [SerializeField] private SpawnComponent _throwSpawner;
        
        [Space] [Header("Extra checkers")]
        [SerializeField] private ColliderCheck _wallCheck;
        [SerializeField] private CheckCircleOverlap _interactionRange;

        [Space] [Header("AnimatorControllers")] 
        [SerializeField] private RuntimeAnimatorController _unarmed;
        [SerializeField] private RuntimeAnimatorController _armed;

        [Space] [Header("Particles")] 
        [SerializeField] private ProbabilityDropComponent _dropCoins;
        
        private static readonly int ThrowKey = Animator.StringToHash("throw");
        private static readonly int WallKey = Animator.StringToHash("is-on-wall");
        private static readonly int ClimbKey = Animator.StringToHash("is-climbing");

        private int _maxHealth;
        public bool IsJumpButtonPressed { get; set; }
        private bool _allowDoubleJump;
        
        private float _defaultGravityScale;
        private bool _isOnWall;
        private bool _isClimbing;
        private bool _isArmed;
        private bool _superThrow;


        private readonly CoolDown _speedUpCooldown = new CoolDown();
        private float _additionalSpeed;

        private HealthComponent _health;
        private GameSession _session;
        private CameraShakeEffect _cameraShake;

        private const string SwordId = "Sword";

        private int SwordsCount => _session.Data.Inventory.Count(SwordId);
        private int CoinsCount => _session.Data.Inventory.Count("Coin");

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
            _defaultGravityScale = Rigidbody.gravityScale;
        }

        private void Start()
        {
            _cameraShake = FindObjectOfType<CameraShakeEffect>();
            _session = GameSession.Instance;
            _health = GetComponent<HealthComponent>();
            _session.Data.Inventory.OnChanged += OnInventoryChanged;
            _session.StatsModel.OnUpgraded += OnHeroUpgraded;

            var health = (int) _session.StatsModel.GetValue(StatId.Hp);
            _maxHealth = health;
            _health.SetHealth(health);
            UpdateHeroWeapon();
        }

        private void OnHeroUpgraded(StatId statId)
        {
            switch (statId)
            {
                case StatId.Hp:
                    var health = (int) _session.StatsModel.GetValue(statId);
                    _session.Data.Hp.Value = health;
                    _health.SetHealth(health);
                    break;
            }
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
            Animator.SetBool(ClimbKey, _isClimbing);
        }

        protected override float CalculateYVelocity()
        {
            var isJumpPressing = Direction.y > 0;

            if (!isJumpPressing && _isOnWall)
            {
                return -_slippingVelocity;
            }
            
            Rigidbody.gravityScale = _defaultGravityScale;
            _isClimbing = false;
            return base.CalculateYVelocity();
        }

        protected override float CalculateSpeed()
        {
            if (_speedUpCooldown.IsReady)
                _additionalSpeed = 0f;
            var defaultSpeed = _session.StatsModel.GetValue(StatId.Speed);
            return defaultSpeed + _additionalSpeed;
        }

        protected override float CalculateJumpVelocity(float yVelocity)
        {
            if (IsGrounded && !IsJumpButtonPressed)
            {
                yVelocity = DoJump();
            }
            else if (_allowDoubleJump && !IsJumpButtonPressed && _session.PerksModel.IsDoubleJumpSupported)
            {
                yVelocity = DoJump();
                _allowDoubleJump = false;
                _session.PerksModel.CoolDown.Reset();
            }

            return yVelocity;
        }

        private float DoJump()
        {
            _particles.Spawn("Jump");
            Sounds.Play("Jump");
            IsJumpButtonPressed = true;

            return _jumpImpulse;
        }
        public void Climb()
        {
            _isClimbing = true;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.IsInLayer(_groundCheck.CheckingLayer)) return;
            
            var contact = other.contacts[0];
            if (contact.relativeVelocity.y >= _slamDownVelocity)
            {
                _particles.Spawn("SlamDown");
            }
        }

        public void AddInInventory(string id, int value)
        {
            _session.Data.Inventory.Add(id, value);
        }

        public override void TakeDamage()
        {
            base.TakeDamage();
            _cameraShake?.Shake();
            
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

        public override void OnDoAttack()
        {
            ApplyDamageStat(_attackRange.gameObject, StatId.MeleeCriticalDamage, StatId.MeleeDamage);
            _attackRange.Check();
        }

        private void UpdateHeroWeapon()
        {
            _isArmed = SwordsCount > 0;
            Animator.runtimeAnimatorController = _isArmed ? _armed : _unarmed;
        }

        public void OnDoThrow()
        {
            if (_superThrow && _session.PerksModel.IsSuperThrowSupported)
            {
                var throwableCount = _session.Data.Inventory.Count(SelectedItemId);
                var possibleCount = SelectedItemId == SwordId ? throwableCount - 1 : throwableCount;
                
                var numThrows = Mathf.Min(_superThrowParticles, possibleCount);
                _session.PerksModel.CoolDown.Reset();
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
            var instance = _throwSpawner.SpawnInstance();
            ApplyDamageStat(instance, StatId.RangeCriticalDamage, StatId.RangeDamage);
            
            _session.Data.Inventory.Remove(throwableId, 1);
        }

        private void ApplyDamageStat(GameObject prefab, StatId critMode, StatId damageMode)
        {
            var healthModifier = prefab.GetComponent<ModifyHealthComponent>();
            var damageValue = (int) _session.StatsModel.GetValue(damageMode);
            damageValue = ModifyDamageByCrit(damageValue, critMode);
            healthModifier.SetDelta(-damageValue);
        }

        private int ModifyDamageByCrit(int damage, StatId critMode)
        {
            var critChance = _session.StatsModel.GetValue(critMode);
            if (Random.value * 100 <= critChance)
            {
                return damage * 2;
            }
            return damage;
        }


        public void StartThrowing()
        {
            _superThrowCooldown.Reset();
        }

        public void UseItem()
        {
            if (IsSelectedItem(ItemTag.Throwable))
                PerformThrowing();
            
            else if (IsSelectedItem(ItemTag.Potion))
                UsePotion();
        }

        private void UsePotion()
        {
            var potion = DefsFacade.I.Potions.Get(SelectedItemId);
            
            switch (potion.Effect)
            {
                case Effect.AddHp:
                    _session.Data.Hp.Value = Mathf.Min((int) potion.Value, _maxHealth);
                    _health.SetHealth(_session.Data.Hp.Value);
                    break;
                case Effect.SpeedUp:
                    _speedUpCooldown.Value = _speedUpCooldown.RemainingTime + potion.Time;
                    _additionalSpeed = Mathf.Max(potion.Value, _additionalSpeed);
                    _speedUpCooldown.Reset();
                    break;
            }

            _session.Data.Inventory.Remove(potion.Id, 1);
        }

        private bool IsSelectedItem(ItemTag itemTag)
        {
            return _session.QuickInventory.SelectedDef.HasTag(itemTag);
        }

        private void PerformThrowing()
        {
            if (!CanThrow || !_throwCooldown.IsReady) return;

            if (_superThrowCooldown.IsReady) _superThrow = true;
            
            Animator.SetTrigger(ThrowKey);
            _throwCooldown.Reset();
        }

        public void PauseGame()
        {
            if (GameObject.FindGameObjectWithTag("PauseMenu") == true) return;
            WindowUtils.CreateWindow("UI/PauseMenuWIndow");
        }
        public void NextItem()
        {
            _session.QuickInventory.SetNextItem();
        }

        public void UsePerk()
        {
            if (!_session.PerksModel.IsShieldSupported) return;
            _shield.Use();
            _session.PerksModel.CoolDown.Reset();
        }

        public void ToggleFlashlight()
        {
            if (!_session.PerksModel.IsFlashlightSupported) return;
            var isActive = _flashlight.gameObject.activeSelf;
            _flashlight.gameObject.SetActive(!isActive);
        }
        
        public void DropDown()
        {
            var endPosition = transform.position + new Vector3(0, -1);
            var hit = Physics2D.Linecast(transform.position, endPosition, _groundCheck.CheckingLayer);
            if (hit.collider == null) return;
            var component = hit.collider.GetComponent<TmpDisableCollider>();
            if (component == null) return;
            component.DisableCollider();
        }

        public void Bounce(float bounceForce)
        {
            IsJumping = false;
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, bounceForce);
        }
    }
}