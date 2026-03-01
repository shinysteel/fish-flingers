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

public class GrabbedItemLogic
{
    private UIManager _uiManager;
    private NetworkManager _networkManager;
    private EntityManager _entityManager;

    private RaftPlayer _player;

    private SyncVar<NetInventoryItem> _netGrabbedInventoryItem;

    private PointerEventData _pointerEventData;
    private List<RaycastResult> _raycastResults = new();

    private InventoryItem _grabbedInventoryItem;
    public InventoryItem GrabbedInventoryItem => _grabbedInventoryItem;

    private InventoryItemView _grabbedItemView;
    
    private const float GrabAlpha = 0.5f;

    public event Action<InventoryItem> OnChanged;

    public GrabbedItemLogic(RaftPlayer player, SyncVar<NetInventoryItem> netGrabbedInventoryItem)
    {
        _uiManager = GameManager.Instance.Get<UIManager>();
        _networkManager = GameManager.Instance.Get<NetworkManager>();
        _entityManager = GameManager.Instance.Get<EntityManager>();

        _player = player;

        _netGrabbedInventoryItem = netGrabbedInventoryItem;
        _netGrabbedInventoryItem.onChanged += HandleNetGrabbedInventoryItemChanged;

        _pointerEventData = new PointerEventData(EventSystem.current);
    }

    public void Dispose()
    {
        _netGrabbedInventoryItem.onChanged -= HandleNetGrabbedInventoryItemChanged;
    }

    public void Tick()
    {
        if (_player.InputLogic.RotateItem)
        {
            Rotate();
        }

        if (_player.InputLogic.LeftClick)
        {
            Click();
        }
    }

    private void Rotate()
    {
        if (_grabbedInventoryItem == null)
        {
            return;
        }

        _networkManager.ChangeSyncVar(_netGrabbedInventoryItem, () => _netGrabbedInventoryItem.value.ChangeRotations(1));
    }

    private void Click()
    {
        GetTargetViews(out InventoryItemView targetItemView, out InventorySlotView targetInventorySlot, out HotbarWidgetSlot targetHotbarSlot, out Panel targetPanel);

        if (_grabbedInventoryItem == null)
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
        else if (targetPanel == null)
        {
            Drop();
        }
    }

    /// <summary>
    /// Retrieve relevant views to target under the cursor
    /// </summary>
    private void GetTargetViews(out InventoryItemView targetItemView, out InventorySlotView targetInventorySlot, out HotbarWidgetSlot targetHotbarSlot, out Panel targetPanel)
    {
        targetItemView = null;
        targetInventorySlot = null;
        targetHotbarSlot = null;
        targetPanel = null;

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

            if (targetPanel == null)
            {
                result.gameObject.TryGetComponent(out targetPanel);
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

        _netGrabbedInventoryItem.value = item;

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
        _player.Hotbar.SetSlot(slot.Index, _grabbedInventoryItem);

        Release();
    }

    /// <summary>
    /// Place the grabbed item at an inventory slot
    /// </summary>
    private void Place(InventorySlotView slotView)
    {
        PlaceParams placeParams = new PlaceParams()
        {
            Cell = slotView.Cell,
            Pivot = _netGrabbedInventoryItem.value.Pivot,
            RotationParams = new RotationParams() { Rotations = _netGrabbedInventoryItem.value.Rotations },
            InstanceId = _grabbedItemView.InventoryItem.ItemInstance.InstanceId,
            ItemId = _grabbedItemView.InventoryItem.ItemInstance.Data.ItemId,
            Amount = _grabbedItemView.InventoryItem.ItemInstance.Count
        };

        if (slotView.InventoryWidget.Inventory.TryPlaceItems(placeParams, true, out int overflow))
        {
            if (overflow > 0)
            {
                _grabbedItemView.InventoryWidget.Inventory.NetInventoryItems[_netGrabbedInventoryItem.value.InstanceId].SetCount(overflow);
                _grabbedItemView.InventoryWidget.Inventory.NetInventoryItems.SetDirty(_netGrabbedInventoryItem.value.InstanceId);
            }
            else
            {
                Release();
            }
        }
    }

    /// <summary>
    /// Drop the grabbed item out of the inventory
    /// </summary>
    private void Drop()
    {
        DroppedItem item = (DroppedItem)_entityManager.Spawn(EEntity.DroppedItem, new SpawnParams() { Position = _player.transform.position });
        item.SetItem(_grabbedInventoryItem.ItemInstance.InstanceId, _grabbedInventoryItem.ItemInstance.Data.ItemId, _grabbedInventoryItem.ItemInstance.Count);
        
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
        _grabbedInventoryItem = item != null ? new InventoryItem(item) : null;

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

        if (_netGrabbedInventoryItem.value.InstanceId != instanceId)
        {
            return;
        }

        // This callback can happen before we call SetNetGrabbedInventoryItem(null) ourselves, so it's safe to ignore in this scenario
        if (newInventoryItem == null)
        {
            return;
        }

        // Sync up with any changes that aren't to the pivot or rotations
        NetInventoryItem netInventoryItem = new NetInventoryItem(newInventoryItem.Cell, _netGrabbedInventoryItem.value.Pivot, _netGrabbedInventoryItem.value.Rotations, newInventoryItem.ItemInstance.InstanceId, newInventoryItem.ItemInstance.Data.ItemId, newInventoryItem.ItemInstance.Count);
        _netGrabbedInventoryItem.value = netInventoryItem;
    }
}
