using FishFlingers.Inventories;
using FishFlingers.Pools;
using FishFlingers.States;
using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.UI
{
    public class UnitItemView : ItemView, ITypedPoolable
    {
        public override void Refresh()
        {
            // UnitItemView's verison of RefreshRect
            _rectTransform.sizeDelta = _slotSize;

            RefreshItemImage();
            RefreshCountText();
            RefreshAssignmentImage();

            // Omitted RefreshDetailsRects
        }

        public void OnReturnedToPool()
        {
            ResetAlpha();
        }

        public void OnTakenFromPool()
        { }
    }
}