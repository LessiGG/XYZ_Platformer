using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Quests
{
    public class Quest : MonoBehaviour
    {
        [SerializeField] protected int _id;
        [SerializeField] protected List<GameObject> _targets;
        [SerializeField] protected UnityEvent _onSuccess;
        [SerializeField] protected UnityEvent _onFail;
        
        public bool _isCompleted;
        public int Id => _id;

        public virtual void Check()
        {
            if (_targets.Count == 0)
            {
                _onSuccess?.Invoke();
                _isCompleted = true;
                return;
            }
            _onFail.Invoke();
        }
    }
}