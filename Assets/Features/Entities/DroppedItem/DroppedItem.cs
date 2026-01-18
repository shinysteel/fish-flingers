using PurrNet;
using UnityEngine;
using FishFlingers.Environments;
using ShinyOwl.Common;
using FishFlingers.Items;

namespace FishFlingers.Entities
{
    public class DroppedItem : NetEntity, IInteractable
    {
        private ItemId _itemId;
        private int _count;

        public DroppedItemData Data => (DroppedItemData)_entityData;

        public Vector3 Position => transform.position;

        public void SetItem(ItemId itemId, int count)
        {
            _itemId = itemId;
            _count = count;
        }

        public void Interact()
        {
            _context.LocalPlayer.Inventory.TryAddItems(_itemId, _count, out _);
            _networkManager.Despawn(this);
        }
    }
}