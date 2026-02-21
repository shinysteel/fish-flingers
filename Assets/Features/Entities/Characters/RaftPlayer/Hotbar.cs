using FishFlingers.Inventories;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using FishFlingers.States;
using System;
using ShinyOwl.Common;

public class Hotbar
{
    private List<InventoryItem> _slots = new List<InventoryItem>(DefaultCapacity);

    public IReadOnlyList<InventoryItem> Slots => _slots;

    private const int DefaultCapacity = 3;

    private GameplayContext _context;

    public event Action<int, InventoryItem> OnSlotChanged;

    public Hotbar(GameplayContext context)
    {
        _context = context;
        _context.LocalPlayer.Inventory.OnInventoryItemChanged += HandleInventoryItemChanged;

        for (int i = 0; i < DefaultCapacity; i++)
        {
            // Can't use .SetSlot, since it does index guards
            _slots.Add(null);
        }
    }

    ~Hotbar()
    {
        if (_context?.LocalPlayer?.Inventory != null)
        {
            _context.LocalPlayer.Inventory.OnInventoryItemChanged -= HandleInventoryItemChanged;
        }
    }

    private void HandleInventoryItemChanged(string instanceId, InventoryItem oldInventoryItem, InventoryItem newInventoryItem)
    {
        // We only care when an item is removed
        if (newInventoryItem != null)
        {
            return;
        }

        // Clear any potential slot linked to that item
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i]?.ItemInstance?.InstanceId == oldInventoryItem.ItemInstance.InstanceId)
            {
                SetSlot(i, null);
                break;
            }
        }
    }

    public void SetSlot(int index, InventoryItem setItem)
    {
        // Guard against invalid requests
        if (index < 0 || index >= _slots.Count)
        {
            return;
        }

        // You can't equip the same item in more than one slot, so we check for duplicates when assigning a value that isn't null
        if (setItem != null)
        {
            foreach (InventoryItem existingItem in _slots)
            {
                if (existingItem?.ItemInstance?.InstanceId == setItem.ItemInstance.InstanceId)
                {
                    return;
                }
            }
        }

        _slots[index] = setItem;

        OnSlotChanged?.Invoke(index, setItem);
    }
}
