using FishFlingers.Pools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class InteractPromptUI : WorldUI
    {
        [SerializeField] private TextMeshProUGUI _hotkeyText;
        [SerializeField] private Image _hotkeyImage;

        [SerializeField] private Sprite _leftClickSprite;
        [SerializeField] private Sprite _rightClickSprite;

        protected PoolManager _poolManager;

        private void Awake()
        {
            _poolManager = GameManager.Instance.Get<PoolManager>();
        }

        public void SetupInteract(ActionHotkey hotkey)
        {
            ActionHotkeyUtils.Apply(hotkey, _hotkeyText, _hotkeyImage, _leftClickSprite, _rightClickSprite);
        }
    }
}