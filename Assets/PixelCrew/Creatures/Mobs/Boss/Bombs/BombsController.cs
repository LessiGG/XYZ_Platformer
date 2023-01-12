using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrew.Components.GoBased;
using UnityEngine;

namespace PixelCrew.Creatures.Mobs.Boss.Bombs
{
    public class BombsController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _platforms;
        [SerializeField] private BombSequence[] _sequences;
        private Coroutine _coroutine;

        [ContextMenu("Start bombing")]
        public void StartBombing()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(BombingSequence());
        }

        private IEnumerator BombingSequence()
        {
            _platforms.ForEach(x =>  x.SetActive(false));
            
            foreach (var bombSequence in _sequences)
            {
                foreach (var spawnComponent in bombSequence.Bombs)
                    spawnComponent.Spawn();

                yield return new WaitForSeconds(bombSequence.Delay);
            }

            _coroutine = null;
            _platforms.ForEach(x =>  x.SetActive(true));
        }
    }

    [Serializable]
    public class BombSequence
    {
        [SerializeField] private SpawnComponent[] _bombs;
        [SerializeField] private float _delay;

        public SpawnComponent[] Bombs => _bombs;
        public float Delay => _delay;
    }
}