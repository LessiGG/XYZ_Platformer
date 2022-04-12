﻿using PixelCrew.Model;
using PixelCrew.Model.Data;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Components.Interactions
{
    public class RequireItemsComponent : MonoBehaviour
    {
        [SerializeField] private InventoryItemData[] _required;
        [SerializeField] private bool _removeAfterUse;

        [SerializeField] private UnityEvent _onSuccess;
        [SerializeField] private UnityEvent _onFail;

        public void Check()
        {
            var session = FindObjectOfType<GameSession>();
            var areAllRequirementsDone = true;

            foreach (var item in _required)
            {
                var numItems = session.Data.Inventory.GetCount(item.Id);

                if (numItems < item.Value)
                    areAllRequirementsDone = false;
            }

            if (areAllRequirementsDone)
            {
                if (_removeAfterUse)
                {
                    foreach (var item in _required)
                        session.Data.Inventory.Remove(item.Id, item.Value);
                }
                _onSuccess?.Invoke();
            }
            
            else
                _onFail?.Invoke();
        }
    }
}