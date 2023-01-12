using PixelCrew.Model;
using UnityEngine;

namespace PixelCrew.Components.GoBased
{
    public class DestroyObjectComponent : MonoBehaviour
    {
        [SerializeField] private GameObject _objectToDestroy;
        [SerializeField] private RestoreStateComponent _restoreState;

        public void DestroyObject()
        {
            Destroy(_objectToDestroy);
            if (_restoreState != null)
                FindObjectOfType<GameSession>().StoreState(_restoreState.Id);
        }
    }
}