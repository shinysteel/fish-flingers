using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

namespace FishFlingers.UI
{
    /// <summary>
    /// Handles raycasts for views relevant to an InventoryWidget
    /// </summary>
    public class InventoryRaycaster
    {
        private UIManager _uiManager;

        private PointerEventData _pointerEventData;
        private List<RaycastResult> _raycastResults = new();

        public InventoryRaycaster()
        {
            _uiManager = GameManager.Instance.Get<UIManager>();

            _pointerEventData = new PointerEventData(EventSystem.current);
        }

        /// <summary>
        /// Retrieve relevant views to target under the cursor
        /// </summary>
        public void GetTargetViews(out InventoryItemView targetItemView, out InventorySlotView targetInventorySlot, out HotbarWidgetSlot targetHotbarSlot, out Panel targetPanel)
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
    }
}