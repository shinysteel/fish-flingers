using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class InteractPromptUI : WorldUI
    {
        [SerializeField] private TextMeshProUGUI _hotkeyText;
        [SerializeField] private Image _hotkeyImage;

        public void Setup(string text)
        {
            _hotkeyText.text = text;
        }
    }
}