using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class SlotView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;
        [SerializeField] private CellOutline _cellOutline;

        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _tintedColor;

        public RectTransform RectTransform => _rectTransform;
        public CellOutline CellOutline => _cellOutline;

        public enum EColor
        {
            Default,
            Tinted,
        }

        public void SetColor(EColor colorEnum)
        {
            Color color = colorEnum switch
            {
                EColor.Default => _defaultColor,
                EColor.Tinted => _tintedColor,
                _ => _defaultColor
            };

            _image.color = color;
        }

        public void SetTransform(Vector2 position, Vector2 size)
        {
            _rectTransform.anchoredPosition = position;
            _rectTransform.sizeDelta = size;
        }
    }
}