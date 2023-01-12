using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Creatures.Mobs.Boss.Bombs
{
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private float _timeToLive;
        [SerializeField] private UnityEvent _onExplode;
        
        private Coroutine _coroutine;

        private void OnEnable()
        {
            TryStopRoutine();
            _coroutine = StartCoroutine(WaitAndExplode());
        }

        private IEnumerator WaitAndExplode()
        {
            yield return new WaitForSeconds(_timeToLive);
            Explode();
            _coroutine = null;
        }

        public void Explode()
        {
            _onExplode?.Invoke();
        }

        private void OnDisable()
        {
            TryStopRoutine();
        }

        private void TryStopRoutine()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}