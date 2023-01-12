using PixelCrew.Components.Health;
using PixelCrew.Utils.Disposables;
using UnityEngine;

namespace PixelCrew.UI.Widgets
{
    public class HealthBarWidget : MonoBehaviour
    {
        [SerializeField] private ProgressBarWidget _healthBar;
        [SerializeField] private HealthComponent _healthComponent;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private int _maxHp;
        private void Start()
        {
            if (_healthComponent == null)
                _healthComponent = GetComponentInParent<HealthComponent>();
            _maxHp = _healthComponent.Health.Value;

            _trash.Retain(_healthComponent._onDie.Subscribe(OnDie));
            _trash.Retain(_healthComponent._onChange.Subscribe(OnHpChanged));
        }

        private void OnDie()
        {
            Destroy(gameObject);
        }

        private void OnHpChanged(int hp)
        {
            var progress = (float) hp / _maxHp;
            _healthBar.SetProgress(progress);
        }

        private void OnDestroy()
        {
            _trash.Dispose();
        }
    }
}