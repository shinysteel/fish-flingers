using FishFlingers.Inventories;
using FishFlingers.States;
using ShinyOwl.Common;
using ShinyOwl.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hotbar
{
    private List<InventoryItem> _slots = new List<InventoryItem>(DefaultCapacity);

    public IReadOnlyList<InventoryItem> Slots => _slots;

    private const int DefaultCapacity = 3;

    private GameplayContext _context;

    private int _selectedIndex;
    public int SelectedIndex => _selectedIndex;

    public InventoryItem SelectedItem => _slots[_selectedIndex];

    // Invoked when a slot is changed
    public event Action<int, InventoryItem> OnSlotChanged;

    // Invoked when a slot is selected
    public event Action<int, InventoryItem> OnSelectedChanged;

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
        bool assigned = false;

        // Find and update a potential slot linked to the item
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i]?.ItemInstance.InstanceId != instanceId)
            {
                continue;
            }

            SetSlot(i, newInventoryItem);
            assigned = true;
            break;
        }

        // If it's not assigned and a slot is available, assign it
        if (oldInventoryItem == null && newInventoryItem != null && !assigned && TryGetNextUnassignedSlot(out int index))
        {
            SetSlot(index, newInventoryItem);
        }
    }

    public void SetSlot(int index, InventoryItem item)
    {
        // Guard against invalid requests
        if (index < 0 || index >= _slots.Count)
        {
            return;
        }

        // You can't equip the same item in more than one slot, so we need to swap the existing assignment when assigning a value that isn't null
        if (item != null)
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                if (i == index)
                {
                    continue;
                }

                if (_slots[i]?.ItemInstance.InstanceId == item.ItemInstance.InstanceId)
                {
                    _slots[i] = null;
                    OnSlotChanged?.Invoke(i, null);
                    break;
                }
            }
        }

        _slots[index] = item;
        OnSlotChanged?.Invoke(index, item);

        if (_selectedIndex == index)
        {
            NotifySelectedChanged();
        }
    }

    public void ChangeSelectedIndex(int delta)
    {
        if (delta == 0)
        {
            return;
        }

        int index = Utils.Math.EuclideanModulo(_selectedIndex + delta, _slots.Count);
        SetSelectedIndex(index);
    }

    public void SetSelectedIndex(int index)
    {
        if (_selectedIndex == index)
        {
            return;
        }

        _selectedIndex = index;
        NotifySelectedChanged();
    }

    private void NotifySelectedChanged()
    {
        OnSelectedChanged?.Invoke(_selectedIndex, _slots[_selectedIndex]);
    }

    public bool IsItemAssigned(InventoryItem item, out int index)
    {
        index = -1;

        if (item == null)
        {
            return false;
        }

        index = _slots.FindIndex(slot => slot?.ItemInstance.InstanceId == item.ItemInstance.InstanceId);
        return index >= 0;
    }

    private bool TryGetNextUnassignedSlot(out int index)
    {
        index = -1;
        
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i] != null)
            {
                continue;
            }

            index = i;
            break;
        }

        return index >= 0;
    }
}
