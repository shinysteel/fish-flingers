using FishFlingers.Cameras;
using FishFlingers.Entities;
using FishFlingers.Inventories;
using FishFlingers.UI;
using PurrNet;
using ShinyOwl.Common;
using ShinyOwl.Common.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using FishFlingers.Items;

public class RaftPlayerGrabbedItemLogic
{
    private RaftPlayer _player;

    private SyncVar<NetInventoryItem> _netGrabbedInventoryItem;

    private InventoryItem _grabbedInventoryItem;
    public InventoryItem GrabbedInventoryItem => _grabbedInventoryItem;

    private InventoryItemView _grabbedItemView;

    // When an item is 'grabbed', it's alpha is modified until the grab is resolved
    private const float GrabAlpha = 0.5f;

    public event Action<InventoryItem> OnChanged;

    public RaftPlayerGrabbedItemLogic(RaftPlayer player, SyncVar<NetInventoryItem> netGrabbedInventoryItem)
    {
        _player = player;

        _netGrabbedInventoryItem = netGrabbedInventoryItem;
        _netGrabbedInventoryItem.onChanged += HandleNetGrabbedInventoryItemChanged;
    }

    ~RaftPlayerGrabbedItemLogic()
    {
        if (_netGrabbedInventoryItem != null)
        {
            _netGrabbedInventoryItem.onChanged -= HandleNetGrabbedInventoryItemChanged;
        }
    }

    /// <summary>
    /// Mark an item as 'grabbed', and visualise it on the cursor
    /// </summary>
    public void Grab(InventoryItemView itemView, InventorySlotView slotView)
    {
        // The item needs to be a clone so that rotating it doesn't affect the original
        string instanceId = itemView.InventoryItem.ItemInstance.InstanceId;
        NetInventoryItem item = itemView.InventoryWidget.Inventory.NetInventoryItems[instanceId].DeepClone();
         
        // The slot we grabbed at becomes the pivot
        item.SetPivot(InventoryItemUtils.RecalculatePivot(item.Cell, slotView.Cell, item.Pivot, item.Rotations));

        _netGrabbedInventoryItem.value = item;

        // Listen for changes while we hold it
        _grabbedItemView = itemView;
        _grabbedItemView.SetAlpha(GrabAlpha);
        _grabbedItemView.InventoryWidget.Inventory.OnInventoryItemChanged += HandleInventoryItemChanged;
    }

    /// <summary>
    /// Retrieve relevant views to target under the cursor
    /// </summary>
    public void Assign(HotbarWidgetSlot slot)
    {
        _player.Hotbar.SetSlot(slot.Index, _grabbedInventoryItem);

        Release();
    }

    /// <summary>
    /// Place the grabbed item at an inventory slot
    /// </summary>
    public void Place(InventorySlotView slotView)
    {
        InventoryPlaceParams placeParams = new InventoryPlaceParams()
        {
            Cell = slotView.Cell,
            Pivot = _netGrabbedInventoryItem.value.Pivot,
            RotationParams = new InventoryRotationParams() { Rotations = _netGrabbedInventoryItem.value.Rotations },
            InstanceId = _grabbedItemView.InventoryItem.ItemInstance.InstanceId,
            ItemId = _grabbedItemView.InventoryItem.ItemInstance.Data.ItemId,
            Count = _grabbedItemView.InventoryItem.ItemInstance.Count
        };

        if (slotView.InventoryWidget.Inventory.TryPlaceItem(placeParams, true, out int overflow, out _, out NetInventoryItemsChange change))
        {
            if (overflow > 0)
            {
                _grabbedItemView.InventoryWidget.Inventory.NetInventoryItems[_netGrabbedInventoryItem.value.ItemInstance.InstanceId].ItemInstance.SetCount(overflow);
                _grabbedItemView.InventoryWidget.Inventory.NetInventoryItems.SetDirty(_netGrabbedInventoryItem.value.ItemInstance.InstanceId);
                return;
            }
            
            // No overflow and a valid change implies the item has no count left
            if (change.IsValid)
            {
                _grabbedItemView.InventoryWidget.Inventory.RemoveItem(_netGrabbedInventoryItem.value.ItemInstance.InstanceId);
            }

            Release();
        }
    }

    /// <summary>
    /// Drop the grabbed item out of the inventory
    /// </summary>
    public void Drop()
    {
        _player.DropItemLogic.SpawnDroppedItem(_grabbedInventoryItem.ItemInstance, true);
        _grabbedItemView.InventoryWidget.Inventory.RemoveItem(_grabbedInventoryItem.ItemInstance.InstanceId);
        Release();
    }

    /// <summary>
    /// Call this after a grab action is resolved to do necessary cleanup
    /// </summary>
    private void Release()
    {
        _grabbedItemView.InventoryWidget.Inventory.OnInventoryItemChanged -= HandleInventoryItemChanged;

        _grabbedItemView.ResetAlpha();
        _grabbedItemView = null;

        _netGrabbedInventoryItem.value = null;
    }

    /// <summary>
    /// Broadcasts changes to the net grabbed item in a nicer format
    /// </summary>
    private void HandleNetGrabbedInventoryItemChanged(NetInventoryItem item)
    {
        _grabbedInventoryItem = item != null ? InventoryItem.Create(item) : null;

        OnChanged?.Invoke(_grabbedInventoryItem);
    }

    /// <summary>
    /// If the source of the item we are holding has changes, we need to match them
    /// </summary>
    private void HandleInventoryItemChanged(string instanceId, InventoryItem oldInventoryItem, InventoryItem newInventoryItem)
    {
        if (_netGrabbedInventoryItem.value == null)
        {
            return;
        }

        if (_netGrabbedInventoryItem.value.ItemInstance.InstanceId != instanceId)
        {
            return;
        }

        // This callback can happen before we call SetNetGrabbedInventoryItem(null) ourselves, so it's safe to ignore in this scenario
        if (newInventoryItem == null)
        {
            return;
        }

        // Sync up with any changes that aren't to the pivot or rotations
        NetInventoryItem netInventoryItem = new NetInventoryItem(newInventoryItem.Cell, _netGrabbedInventoryItem.value.Pivot, _netGrabbedInventoryItem.value.Rotations, NetItemInstance.Create(newInventoryItem.ItemInstance));

        _netGrabbedInventoryItem.value = netInventoryItem;
    }
}
