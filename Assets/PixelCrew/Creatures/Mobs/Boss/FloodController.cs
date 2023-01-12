using System.Collections;
using UnityEngine;

namespace PixelCrew.Creatures.Mobs.Boss
{
    public class FloodController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _floodTime;
        private static readonly int Property = Animator.StringToHash("is-flooding");

        private Coroutine _coroutine;

        public void StartFlooding()
        {
            if (_coroutine != null) return;
            _coroutine = StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            _animator.SetBool(Property, true);
            yield return new WaitForSeconds(_floodTime);
            _animator.SetBool(Property, false);
            _coroutine = null;
        }
    }
}