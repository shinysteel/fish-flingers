using FishFlingers.Items;
using FishFlingers.Pools;
using FishFlingers.States;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class ItemActionView : MonoBehaviour, ITypedPoolable
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _actionImage;
        [SerializeField] private TextMeshProUGUI _hotkeyText;
        [SerializeField] private Image _hotkeyImage;

        [SerializeField] private Sprite _leftClickSprite;
        [SerializeField] private Sprite _rightClickSprite;

        private GameplayContext _context;
        private ItemActionData _data;

        private void Awake()
        {
            _button.onClick.AddListener(Pressed);
        }

        public void Setup(GameplayContext context, ItemActionData data)
        {
            _context = context;
            _data = data;

            if (_data.Hotkey == ActionHotkey.None)
            {
                return;
            }

            ActionHotkeyUtils.Apply(_data.Hotkey, _hotkeyText, _hotkeyImage, _leftClickSprite, _rightClickSprite);

            _actionImage.sprite = _data.Sprite;
        }

        private void Pressed()
        {
            _context.LocalPlayer.InteractLogic.Interact(_data.Hotkey);
        }

        public void OnReturnedToPool()
        {
            _context = null;
            _data = null;
        }

        public void OnTakenFromPool()
        { }
    }
}