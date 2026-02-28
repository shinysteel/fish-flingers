using FishFlingers.Entities;
using FishFlingers.Inventories;
using FishFlingers.UI;
using PurrNet;
using ShinyOwl.Common;
using ShinyOwl.Common.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Pool;
using NetworkManager = FishFlingers.Networking.NetworkManager;

public class HeldItemLogic
{
    private UIManager _uiManager;
    private NetworkManager _networkManager;

    private RaftPlayer _player;

    private SyncVar<NetInventoryItem> _netHeldInventoryItem;

    private PointerEventData _pointerEventData;
    private List<RaycastResult> _raycastResults = new();

    private InventoryItem _heldInventoryItem;
    public InventoryItem HeldInventoryItem => _heldInventoryItem;

    private InventoryItemView _grabbedItemView;
    
    private const float GrabAlpha = 0.5f;

    public event Action<InventoryItem> OnChanged;

    public HeldItemLogic(RaftPlayer player, SyncVar<NetInventoryItem> netHeldInventoryItem)
    {
        _uiManager = GameManager.Instance.Get<UIManager>();
        _networkManager = GameManager.Instance.Get<NetworkManager>();

        _player = player;

        _netHeldInventoryItem = netHeldInventoryItem;
        _netHeldInventoryItem.onChanged += HandleNetHeldInventoryItemChanged;

        _pointerEventData = new PointerEventData(EventSystem.current);
    }

    public void Dispose()
    {
        _netHeldInventoryItem.onChanged -= HandleNetHeldInventoryItemChanged;
    }

    public void Tick()
    {
        if (_player.InputLogic.Rotate)
        {
            Rotate();
        }

        if (_player.InputLogic.Click)
        {
            Click();
        }
    }

    private void Rotate()
    {
        if (_heldInventoryItem == null)
        {
            return;
        }

        _networkManager.ChangeSyncVar(_netHeldInventoryItem, () => _netHeldInventoryItem.value.ChangeRotations(1));
    }

    private void Click()
    {
        GetTargetViews(out InventoryItemView targetItemView, out InventorySlotView targetInventorySlot, out HotbarWidgetSlot targetHotbarSlot);

        if (_heldInventoryItem == null)
        {
            // If the item and slot is linked, grab it
            if (targetItemView != null && targetInventorySlot != null && targetItemView.InventoryItem.ItemInstance.InstanceId == targetInventorySlot.InventoryItem?.ItemInstance.InstanceId)
            {
                Grab(targetItemView, targetInventorySlot);
            }
        }
        else if (targetHotbarSlot != null)
        {
            Assign(targetHotbarSlot);
        }
        else if (targetInventorySlot != null)
        {
            Place(targetInventorySlot);
        }
        else
        {
            Drop();
        }
    }

    /// <summary>
    /// Retrieve relevant views to target under the cursor
    /// </summary>
    private void GetTargetViews(out InventoryItemView targetItemView, out InventorySlotView targetInventorySlot, out HotbarWidgetSlot targetHotbarSlot)
    {
        targetItemView = null;
        targetInventorySlot = null;
        targetHotbarSlot = null;

        List<InventoryItemView> targetItemViews = ListPool<InventoryItemView>.Get();

        _pointerEventData.Reset();
        _pointerEventData.position = Input.mousePosition;

        _raycastResults.Clear();

        _uiManager.ScreenGraphicRaycaster.Raycast(_pointerEventData, _raycastResults);

        // Retrieve the first inventory slot and hotbar slot we detect. We can expect multiple items in a single raycast,
        // so we use a list to track those
        foreach (RaycastResult result in _raycastResults)
        {
            if (result.gameObject.TryGetComponent(out targetItemView))
            {
                targetItemViews.Add(targetItemView);
            }

            if (targetInventorySlot == null)
            {
                result.gameObject.TryGetComponent(out targetInventorySlot);
            }

            if (targetHotbarSlot == null)
            {
                result.gameObject.TryGetComponent(out targetHotbarSlot);
            }
        }

        // Choose the preferred targetItemView 
        try
        {
            if (targetItemViews.Count == 0)
            {
                return;
            }

            targetItemView = targetItemViews[0];

            if (targetInventorySlot?.InventoryItem == null)
            {
                return;
            }
            
            // Given items can overlap cells they aren't actually on, we'd prefer to target items that are actually on the slot
            foreach (InventoryItemView itemView in targetItemViews)
            {
                if (itemView.InventoryItem.ItemInstance.InstanceId == targetInventorySlot.InventoryItem.ItemInstance.InstanceId)
                {
                    targetItemView = itemView;
                    return;
                }
            }
        }
        finally
        {
            ListPool<InventoryItemView>.Release(targetItemViews);
        }
    }

    /// <summary>
    /// Mark an item as 'grabbed', and visualise it on the cursor
    /// </summary>
    private void Grab(InventoryItemView itemView, InventorySlotView slotView)
    {
        // The item needs to be a clone so that rotating it doesn't affect the original
        string instanceId = itemView.InventoryItem.ItemInstance.InstanceId;
        NetInventoryItem item = itemView.InventoryWidget.Inventory.NetInventoryItems[instanceId].DeepClone();
         
        Vector2Int origin = item.Cell - Utils.Math.RotateCell(item.Pivot, item.Rotations, true);
        Vector2Int offset = slotView.Cell - origin;
        Vector2Int pivot = Utils.Math.RotateCell(offset, item.Rotations, false);
        item.SetPivot(pivot);

        _netHeldInventoryItem.value = item;

        // Listen for changes while we hold it
        _grabbedItemView = itemView;
        _grabbedItemView.SetAlpha(GrabAlpha);
        _grabbedItemView.InventoryWidget.Inventory.OnInventoryItemChanged += HandleInventoryItemChanged;
    }

    /// <summary>
    /// Retrieve relevant views to target under the cursor
    /// </summary>
    private void Assign(HotbarWidgetSlot slot)
    {
        _player.Hotbar.SetSlot(slot.Index, _heldInventoryItem);

        Release();
    }

    /// <summary>
    /// Place the held item at an inventory slot
    /// </summary>
    private void Place(InventorySlotView slotView)
    {
        PlaceParams placeParams = new PlaceParams()
        {
            Cell = slotView.Cell,
            Pivot = _netHeldInventoryItem.value.Pivot,
            RotationParams = new RotationParams() { Rotations = _netHeldInventoryItem.value.Rotations },
            InstanceId = _grabbedItemView.InventoryItem.ItemInstance.InstanceId,
            ItemId = _grabbedItemView.InventoryItem.ItemInstance.Data.ItemId,
            Amount = _grabbedItemView.InventoryItem.ItemInstance.Count
        };

        if (slotView.InventoryWidget.Inventory.TryPlaceItems(placeParams, true, out int overflow))
        {
            if (overflow > 0)
            {
                _grabbedItemView.InventoryWidget.Inventory.NetInventoryItems[_netHeldInventoryItem.value.InstanceId].SetCount(overflow);
                _grabbedItemView.InventoryWidget.Inventory.NetInventoryItems.SetDirty(_netHeldInventoryItem.value.InstanceId);
            }
            else
            {
                Release();
            }
        }
    }

    /// <summary>
    /// Drop the held item out of the inventory
    /// </summary>
    private void Drop()
    {
        _grabbedItemView.InventoryWidget.Inventory.RemoveItem(_heldInventoryItem.ItemInstance.InstanceId);
        Release();
    }

    /// <summary>
    /// Call this after a hold action is resolved
    /// </summary>
    private void Release()
    {
        _grabbedItemView.InventoryWidget.Inventory.OnInventoryItemChanged -= HandleInventoryItemChanged;

        _grabbedItemView.ResetAlpha();
        _grabbedItemView = null;

        _netHeldInventoryItem.value = null;
    }

    /// <summary>
    /// Broadcasts changes to the net held item in a nicer format
    /// </summary>
    private void HandleNetHeldInventoryItemChanged(NetInventoryItem item)
    {
        _heldInventoryItem = item != null ? new InventoryItem(item) : null;

        OnChanged?.Invoke(_heldInventoryItem);
    }

    /// <summary>
    /// If the source of the item we are holding has changes, we need to match them
    /// </summary>
    private void HandleInventoryItemChanged(string instanceId, InventoryItem oldInventoryItem, InventoryItem newInventoryItem)
    {
        if (_netHeldInventoryItem.value == null)
        {
            return;
        }

        if (_netHeldInventoryItem.value.InstanceId != instanceId)
        {
            return;
        }

        // This callback can happen before we call SetNetHeldInventoryItem(null) ourselves, so it's safe to ignore in this scenario
        if (newInventoryItem == null)
        {
            return;
        }

        // Sync up with any changes that aren't to the pivot or rotations
        NetInventoryItem netInventoryItem = new NetInventoryItem(newInventoryItem.Cell, _netHeldInventoryItem.value.Pivot, _netHeldInventoryItem.value.Rotations, newInventoryItem.ItemInstance.InstanceId, newInventoryItem.ItemInstance.Data.ItemId, newInventoryItem.ItemInstance.Count);
        _netHeldInventoryItem.value = netInventoryItem;
    }
}
