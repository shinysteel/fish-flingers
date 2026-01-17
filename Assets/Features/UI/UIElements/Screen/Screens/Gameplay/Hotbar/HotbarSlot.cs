using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class HotbarSlot : MonoBehaviour
    {
        [SerializeField] private Image _image;

        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _unselectedColor;

        public void SetSelected(bool selected)
        {
            _image.color = selected ? _selectedColor : _unselectedColor;
        }
    }
}