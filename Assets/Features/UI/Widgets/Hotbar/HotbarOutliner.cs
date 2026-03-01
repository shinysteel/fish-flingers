using FishFlingers.Inventories;
using FishFlingers.States;
using UnityEngine;

namespace FishFlingers.UI
{
    public class HotbarOutliner : SlotViewOutliner<HotbarWidgetSlot>
    {
        private HotbarWidget _hotbarWidget;

        public HotbarOutliner(GameplayContext context, HotbarWidget widget) : base(context)
        {
            _hotbarWidget = widget;
        }

        public override void Refresh()
        {
            RefreshColors();
        }

        private void RefreshColors()
        {
            InventoryItem grabbedItem = _context.LocalPlayer.GrabbedItemLogic.GrabbedInventoryItem;

            foreach (HotbarWidgetSlot slot in _hotbarWidget.Slots)
            {
                CellOutline.EColor color;

                if (grabbedItem != null && slot == _targetSlotView)
                {
                    color = CellOutline.EColor.Positive;
                }
                else if (grabbedItem == null && slot == _targetSlotView && slot.InventoryItem != null)
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