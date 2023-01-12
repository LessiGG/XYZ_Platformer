using PixelCrew.Components.Health;
using UnityEngine;

namespace PixelCrew.Quests
{
    public class KillingQuest : Quest
    {
        private void Start()
        {
            foreach (var target in _targets)
            {
                var hp = target.GetComponent<HealthComponent>();
                hp._onDie.AddListener(() => OnTargetDie(target));
            }
        }

        public override void Check()
        {
            if (_targets.Count == 0)
            {
                _onSuccess?.Invoke();
                _isCompleted = true;
                return;
            }
            _onFail.Invoke();
        }

        private void OnTargetDie(GameObject target)
        {
            _targets.Remove(target);
        }
    }
}