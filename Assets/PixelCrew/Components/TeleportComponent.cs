using System.Collections;
using UnityEngine;

namespace PixelCrew.Components
{
    public class TeleportComponent : MonoBehaviour
    {
        [SerializeField] private GameObject _destination;
        [SerializeField] private float _alphaTime =  1;
        [SerializeField] private float _moveTime = 1;

        public void Teleport(GameObject target)
        {
            StartCoroutine(AnimateTeleportation(target));
        }

        public void HideAlpha(GameObject target)
        {
            StartCoroutine(AnimateAlpha(target));
        }

        private IEnumerator AnimateTeleportation(GameObject target)
        {
            var sprite = target.GetComponent<SpriteRenderer>();
            
            yield return AlphaAnimation(sprite, 0);

            yield return MoveAnimation(target);
            
            yield return AlphaAnimation(sprite, 1);
        }
        
        private IEnumerator AnimateAlpha(GameObject target)
        {
            var sprite = target.GetComponent<SpriteRenderer>();
            
            yield return AlphaAnimation(sprite, 0);
        }

        private IEnumerator MoveAnimation(GameObject target)
        {
            var moveTime = 0f;
            while (moveTime < _moveTime)
            {
                moveTime += Time.deltaTime;
                var progress = moveTime / _moveTime;
                target.transform.position = Vector3.Lerp(target.transform.position, _destination.transform.position, progress);

                yield return null;
            }
        }

        private IEnumerator AlphaAnimation(SpriteRenderer sprite, float requiredAlpha)
        {
            var time = 0f;
            var spriteAlpha = sprite.color.a;
            while (time < _alphaTime)
            {
                time += Time.deltaTime;
                var progress = time / _alphaTime;
                var temporaryAlpha = Mathf.Lerp(spriteAlpha, requiredAlpha, progress);
                var color = sprite.color;
                color.a = temporaryAlpha;
                sprite.color = color;

                yield return null;
            }
        }
    }
}