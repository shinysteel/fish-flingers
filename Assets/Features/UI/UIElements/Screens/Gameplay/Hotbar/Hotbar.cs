using ShinyOwl.Common;
using ShinyOwl.Common.Utils;
using UnityEngine;

namespace FishFlingers.UI
{
    public class Hotbar : MonoBehaviour
    {
        [SerializeField] private HotbarSlot _slotPrefab;

        private HotbarSlot[] _slots;

        private int _selectedIndex;

        private const int DefaultSlots = 3;

        private void Start()
        {
            _slots = new HotbarSlot[DefaultSlots];

            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i] = Instantiate(_slotPrefab, transform);
            }

            RefreshSlots();
        }

        private void Update()
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

        private void ChangeSelectedSlot(int delta)
        {
            if (delta == 0)
            {
                return;
            }

            _selectedIndex = Utils.Math.EuclideanModulo(_selectedIndex + delta, _slots.Length);

            RefreshSlots();
        }

        private void RefreshSlots()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i].SetSelected(i == _selectedIndex);
            }
        }
    }
}