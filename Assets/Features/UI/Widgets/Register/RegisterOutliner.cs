using FishFlingers.Entities;
using FishFlingers.Inventories;
using FishFlingers.Pools;
using FishFlingers.States;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace FishFlingers.UI
{
    public class RegisterOutliner<T> : SlotViewOutliner<T> where T : RegisterSlotView, ITypedPoolable
    {
        private RegisterWidget<T> _registerWidget;

        public RegisterOutliner(GameplayContext context, RegisterWidget<T> widget) : base(context)
        {
            _registerWidget = widget;
        }

        public override void Refresh()
        {
            RefreshColors();
        }

        private void RefreshColors()
        {
            InventoryItem grabbedInventoryItem = _context.LocalPlayer.GrabbedInventoryItemLogic.GrabbedInventoryItem;

            foreach (RegisterSlotView slot in _registerWidget.Slots)
            {
                CellOutline.EColor color;

                if (grabbedInventoryItem != null && slot == _targetSlotView)
                {
                    color = CellOutline.EColor.Positive;
                }
                else if (grabbedInventoryItem == null && slot == _targetSlotView && slot.InventoryItem != null)
                {
                    color = CellOutline.EColor.Negative;
                }
                else
                {
                    color = CellOutline.EColor.Default;
                }

                slot.CellOutline.SetColor(color);
            }
        }
    }
}