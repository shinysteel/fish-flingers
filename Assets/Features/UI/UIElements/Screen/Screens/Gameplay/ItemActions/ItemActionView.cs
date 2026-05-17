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
        [SerializeField] private ActionHotkeyView _actionHotkeyView;

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

            _actionHotkeyView.Set(_data.Hotkey);

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