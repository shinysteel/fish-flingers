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

        private SyncVar<NetItemInstance> _netItemInstance = new SyncVar<NetItemInstance>(ownerAuth: true);

        public DroppedItemData Data => (DroppedItemData)_entityData;

        public Vector3 Position => transform.position;

        protected override void OnSpawned()
        {
            base.OnSpawned();

            HandleNetItemInstanceChanged(_netItemInstance);

            _netItemInstance.onChanged += HandleNetItemInstanceChanged;
        }

        protected override void OnDespawned()
        {
            base.OnDespawned();

            _netItemInstance.onChanged -= HandleNetItemInstanceChanged;
        }

        private void HandleNetItemInstanceChanged(NetItemInstance netItemInstance)
        {
            _spriteRenderer.sprite = netItemInstance.ItemId != ItemId.None ? _itemManager.GetItemData(netItemInstance.ItemId).Sprite : null;
        }

        public void SetNetItemInstance(NetItemInstance netItemInstance)
        {
            _netItemInstance.value = netItemInstance;
        }

        public void Interact()
        {
            if (_context.LocalPlayer.Inventory.TryAddItem(InventoryChangeParams.Create(_netItemInstance), false, out _, out _, out _))
            {
                _networkManager.Despawn(this);
            }
        }
    }
}