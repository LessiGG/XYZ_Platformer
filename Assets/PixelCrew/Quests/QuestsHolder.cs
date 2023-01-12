using System.Collections.Generic;
using UnityEngine;

namespace PixelCrew.Quests
{
    public class QuestsHolder : MonoBehaviour
    {
        [SerializeField] private List<Quest> _quests;
        [SerializeField] private Quest _defaultQuest;
        [SerializeField] private bool _showDefaultQuest;

        private Quest _activeQuest;

        private void Start()
        {
            _activeQuest = _quests[0];
        }

        public void Check()
        {
            switch (_showDefaultQuest)
            {
                case true:
                {
                    if (_activeQuest._isCompleted)
                        _activeQuest = _activeQuest.Id + 1 >= _quests.Count ? _defaultQuest : _quests[_activeQuest.Id + 1];
                    break;
                }
                case false:
                {
                    if (_activeQuest._isCompleted)
                    {
                        if (_activeQuest.Id + 1 >= _quests.Count)
                            return;
                        _activeQuest = _quests[_activeQuest.Id + 1];
                    }

                    break;
                }
            }

            _activeQuest.Check();
        }
    }
}