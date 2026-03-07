using PurrNet;
using UnityEngine;
using FishFlingers.Environments;
using ShinyOwl.Common;
using FishFlingers.Items;
using FishFlingers.Inventories;

namespace FishFlingers.Entities
{
    public class DroppedItem : NetEntity, IInteractable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private SyncVar<string> _instanceId = new SyncVar<string>(ownerAuth: true);
        private SyncVar<ItemId> _itemId = new SyncVar<ItemId>(ownerAuth: true);
        private SyncVar<int> _count = new SyncVar<int>(ownerAuth: true);

        public DroppedItemData Data => (DroppedItemData)_entityData;

        public Vector3 Position => transform.position;

        protected override void OnSpawned()
        {
            base.OnSpawned();
            
            HandleItemIdChanged(_itemId.value);

            _itemId.onChanged += HandleItemIdChanged;
        }

        protected override void OnDespawned()
        {
            base.OnDespawned();
            
            _itemId.onChanged -= HandleItemIdChanged;
        }

        private void HandleItemIdChanged(ItemId itemId)
        {
            _spriteRenderer.sprite = itemId != ItemId.None ? _itemManager.GetItemData(itemId).Sprite : null;
        }

        public void SetItem(string instanceId, ItemId itemId, int count)
        {
            _instanceId.value = instanceId;
            _itemId.value = itemId;
            _count.value = count;
        }

        public void Interact()
        {
            ChangeParams parameters = new ChangeParams()
            {
                InstanceId = _instanceId,
                ItemId = _itemId,
                Count = _count
            };

            if (_context.LocalPlayer.Inventory.TryAddItem(parameters, false, out _, out _, out _))
            {
                _networkManager.Despawn(this);
            }
        }
    }
}