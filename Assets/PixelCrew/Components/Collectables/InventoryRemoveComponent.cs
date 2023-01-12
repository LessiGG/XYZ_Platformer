using PixelCrew.Model;
using PixelCrew.Model.Data;
using UnityEngine;

namespace PixelCrew.Components.Collectables
{
    public class InventoryRemoveComponent : MonoBehaviour
    {
        [SerializeField] private InventoryItemData[] _itemsToRemove;
        private GameSession _session;

        private void Start()
        {
            _session = GameSession.Instance;
        }

        public void Remove()
        {
            foreach (var item in _itemsToRemove)
                _session.Data.Inventory.Remove(item.Id, item.Value);
        }
    }
}