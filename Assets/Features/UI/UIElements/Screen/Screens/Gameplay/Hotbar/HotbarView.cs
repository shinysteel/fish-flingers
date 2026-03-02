using FishFlingers.Inventories;
using FishFlingers.States;
using ShinyOwl.Common;
using ShinyOwl.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class HotbarView : MonoBehaviour
    {
        [SerializeField] private Transform _backgroundTransform;
        [SerializeField] private Image _backgroundImage;

        [SerializeField] private HotbarSlot _slotPrefab;

        private GameplayContext _context;

        private Material _backgroundMaterial;

        private HotbarSlot[] _slots;

        // Background shader requires an index that doesn't wrap, so we use a local index here and just use the
        // delta when updating the hotbar's selected index
        private int _selectedIndex;
        private float _selectedIndexBlend;

        private const string SlotsCountName = "_SlotsCount";
        private const string HighlightIndexName = "_HighlightIndex";

        private const float ScrollSpeed = 100f;

        public void Setup(GameplayContext context)
        {
            _context = context;

            Hotbar hotbar = _context.LocalPlayer.Hotbar;

            _slots = new HotbarSlot[hotbar.Slots.Count];

            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i] = Instantiate(_slotPrefab, transform);
            }

            _backgroundMaterial = _backgroundImage.material;
            _backgroundMaterial.SetInt(SlotsCountName, hotbar.Slots.Count);
            
            // Since the background is getting inverse masked, it needs to be last
            _backgroundTransform.SetAsLastSibling();

            for (int i = 0; i < hotbar.Slots.Count; i++)
            {
                HandleSlotChanged(i, hotbar.Slots[i]);
            }

            hotbar.OnSlotChanged += HandleSlotChanged;

            _selectedIndex = hotbar.SelectedIndex;
            _selectedIndexBlend = _selectedIndex;

            HandleSelectedChanged(hotbar.SelectedIndex, hotbar.Slots[hotbar.SelectedIndex]);
            hotbar.OnSelectedChanged += HandleSelectedChanged;
        }

        ~HotbarView()
        {
            if (_context.LocalPlayer?.Hotbar != null)
            {
                _context.LocalPlayer.Hotbar.OnSlotChanged -= HandleSlotChanged;
                _context.LocalPlayer.Hotbar.OnSelectedChanged -= HandleSelectedChanged;
            }
        }

        private void HandleSlotChanged(int index, InventoryItem item)
        {
            _slots[index].SetInventoryItem(item);
        }

        private void Update()
        {
            if (_context.LocalPlayer.CanAct)
            {
                ScrollUpdate();
                HotkeyUpdate();
            }

            BackgroundUpdate();
        }

        private void ScrollUpdate()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll > 0f)
            {
                ChangeSelectedSlot(-1);
            }
            else if (scroll < 0f)
            {
                ChangeSelectedSlot(1);
            }
        }

        private void HotkeyUpdate()
        {
            if (!_context.LocalPlayer.InputLogic.TryGetNumber(out int number))
            {
                return;
            }

            int delta = number - 1 - Utils.Math.EuclideanModulo(_selectedIndex, _slots.Length);
            ChangeSelectedSlot(delta);
        }

        private void BackgroundUpdate()
        {
            _selectedIndexBlend = Mathf.Lerp(_selectedIndexBlend, _selectedIndex, ScrollSpeed * Time.deltaTime);

            // The background uses a shader to highlight the selected slot
            _backgroundMaterial.SetFloat(HighlightIndexName, _selectedIndexBlend);
        }

        private void ChangeSelectedSlot(int delta)
        {
            if (delta == 0)
            {
                return;
            }

            _selectedIndex += delta;

            _context.LocalPlayer.Hotbar.SetSelected(Utils.Math.EuclideanModulo(_selectedIndex, _slots.Length));
        }

        private void HandleSelectedChanged(int index, InventoryItem item)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i].SetSelected(i == index);
            }
        }
    }
}