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
        GetTargetViews(out InventoryItemView targetItemView, out HotbarWidgetSlot targetHotbarSlot, out InventorySlotView targetInventorySlot);

        if (_heldInventoryItem == null)
        {
            // If the item and slot is linked, grab it
            if (targetItemView != null && targetInventorySlot != null && targetItemView.View.InventoryItem.ItemInstance.InstanceId == targetInventorySlot.InventoryItem?.ItemInstance.InstanceId)
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
    private void GetTargetViews(out InventoryItemView targetItemView, out HotbarWidgetSlot targetHotbarSlot, out InventorySlotView targetInventorySlot)
    {
        targetItemView = null;
        targetHotbarSlot = null;
        targetInventorySlot = null;

        _pointerEventData.Reset();
        _pointerEventData.position = Input.mousePosition;

        _raycastResults.Clear();

        _uiManager.ScreenGraphicRaycaster.Raycast(_pointerEventData, _raycastResults);

        foreach (RaycastResult result in _raycastResults)
        {
            if (targetItemView == null)
            {
                result.gameObject.TryGetComponent(out targetItemView);
            }

            if (targetHotbarSlot == null)
            {
                result.gameObject.TryGetComponent(out targetHotbarSlot);
            }

            if (targetInventorySlot == null)
            {
                result.gameObject.TryGetComponent(out targetInventorySlot);
            }

            if (targetItemView != null && targetHotbarSlot != null && targetInventorySlot != null)
            {
                return;
            }
        }
    }

    /// <summary>
    /// Mark an item as 'grabbed', and visualise it on the cursor
    /// </summary>
    private void Grab(InventoryItemView itemView, InventorySlotView slotView)
    {
        // The item needs to be a clone so that rotating it doesn't affect the original
        string instanceId = itemView.View.InventoryItem.ItemInstance.InstanceId;
        NetInventoryItem item = itemView.InventoryWidget.Inventory.NetInventoryItems[instanceId].DeepClone();
         
        Vector2Int origin = item.Cell - Utils.Math.RotateCell(item.Pivot, item.Rotations, true);
        Vector2Int offset = slotView.Cell - origin;
        Vector2Int pivot = Utils.Math.RotateCell(offset, item.Rotations, false);
        item.SetPivot(pivot);

        SetNetHeldInventoryItem(item);

        // Listen for changes while we hold it
        _grabbedItemView = itemView;
        _grabbedItemView.View.SetAlpha(GrabAlpha);
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
            InstanceId = _grabbedItemView.View.InventoryItem.ItemInstance.InstanceId,
            ItemId = _grabbedItemView.View.InventoryItem.ItemInstance.Data.ItemId,
            Amount = _grabbedItemView.View.InventoryItem.ItemInstance.Count
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

        _grabbedItemView.View.ResetAlpha();
        _grabbedItemView = null;

        SetNetHeldInventoryItem(null);
    }

    private void SetNetHeldInventoryItem(NetInventoryItem item)
    {
        if (_netHeldInventoryItem.value == item)
        {
            return;
        }

        _netHeldInventoryItem.value = item;
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

        if (newInventoryItem == null)
        {
            return;
        }
        
        _networkManager.ChangeSyncVar(_netHeldInventoryItem, () => _netHeldInventoryItem.value.SetCount(newInventoryItem.ItemInstance.Count));
    }
}
