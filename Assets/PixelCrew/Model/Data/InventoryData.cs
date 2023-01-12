using System;
using System.Collections.Generic;
using System.Linq;
using PixelCrew.Model.Definitions;
using PixelCrew.Model.Definitions.Repositories;
using PixelCrew.Model.Definitions.Repositories.Items;
using UnityEngine;

namespace PixelCrew.Model.Data
{
    [Serializable]
    public class InventoryData
    {
        [SerializeField] private List<InventoryItemData> _inventory = new List<InventoryItemData>();

        public delegate void OnInventoryChanged(string id, int value);

        public OnInventoryChanged OnChanged;

        public List<InventoryItemData> Inventory => _inventory;
        public int InventorySize => _inventory.Count;
        public bool IsFull => _inventory.Count >= DefsFacade.I.Player.InventorySize;

        public InventoryItemData[] GetAllItems(params ItemTag[] tags)
        {
            var returnValue = new List<InventoryItemData>();
            
            foreach (var item in _inventory)
            {
                var itemDef = DefsFacade.I.Items.Get(item.Id);
                var areAllRequirementsMet = tags.All(x => itemDef.HasTag(x));

                if (areAllRequirementsMet)
                    returnValue.Add(item); 
            }
            
            return returnValue.ToArray();
        }

        public void Add(string id, int value)
        {
            if (value <= 0) return;

            var itemDef = DefsFacade.I.Items.Get(id);
            if (itemDef.IsVoid) return;

            if (itemDef.HasTag(ItemTag.Stackable))
            {
                AddToStack(id, value);
            }
            else
            {
                AddNonStack(id, value);
            }
            
            OnChanged?.Invoke(id, Count(id));
        }

        private void AddToStack(string id, int value)
        {
            var item = GetItem(id);
            if (item == null)
            {
                if (IsFull) return;
                    
                item = new InventoryItemData(id);
                _inventory.Add(item);
            }
            item.Value += value;
        }

        private void AddNonStack(string id, int value)
        {
            var itemsLasts = DefsFacade.I.Player.InventorySize - _inventory.Count;
            value = Mathf.Min(itemsLasts, value);
            
            for (var i = 0; i < value; i++)
            {
                var item = new InventoryItemData(id) {Value = 1};
                _inventory.Add(item);
            }
        }

        public void Remove(string id, int value)
        {
            var itemDef = DefsFacade.I.Items.Get(id);
            if (itemDef.IsVoid) return;

            if (itemDef.HasTag(ItemTag.Stackable))
            {
                RemoveFromStack(id, value);
            }
            else
            {
                RemoveNonStack(id, value);
            }

            OnChanged?.Invoke(id, Count(id));
        }

        private void RemoveFromStack(string id, int value)
        {
            var item = GetItem(id);
            if (item == null) return;
            
            item.Value -= value;

            if (item.Value <= 0)
                _inventory.Remove(item);
        }

        private void RemoveNonStack(string id, int value)
        {
            for (var i = 0; i < value; i++)
            {
                var item = GetItem(id);
                if (item == null) return;
                    
                _inventory.Remove(item);
            }
        }
        
        public bool IsContainingStackableItem(InventoryItemData item)
        {
            return DefsFacade.I.Items.Get(item.Id).HasTag(ItemTag.Stackable) && _inventory.Any(itemData => itemData.Id == item.Id);
        }

        private InventoryItemData GetItem(string id)
        {
            return _inventory.FirstOrDefault(itemData => itemData.Id == id);
        }

        public int Count(string id)
        {
            return _inventory.Where(item => item.Id == id).Sum(item => item.Value);
        }

        public bool IsEnough(params ItemWithCount[] items)
        {
            var joined = new Dictionary<string, int>();
            
            foreach (var item in items)
            {
                if (joined.ContainsKey(item.ItemId))
                    joined[item.ItemId] += item.Count;
                else
                    joined.Add(item.ItemId, item.Count);
            }
            
            foreach (var kvp in joined)
            {
                var count = Count(kvp.Key);
                if (count < kvp.Value) return false;
            }

            return true;
        }
    }

    [Serializable]
    public class InventoryItemData
    {
        [InventoryId] public string Id;
        public int Value;

        public InventoryItemData(string id)
        {
            Id = id;
        }
    }
}