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
        [SerializeField] private Image _hotkeyImage;
        [SerializeField] private TextMeshProUGUI _hotkeyText;

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

            if (_data.InteractHotkey == InteractHotkey.None)
            {
                return;
            }

            _hotkeyImage.gameObject.SetActive(false);
            _hotkeyText.gameObject.SetActive(false);

            if (_data.InteractHotkey == InteractHotkey.FKey)
            {
                _hotkeyText.text = "F";
                _hotkeyText.gameObject.SetActive(true);
            }
            else if (_data.InteractHotkey == InteractHotkey.LeftClick)
            {
                _hotkeyImage.sprite = _leftClickSprite;
                _hotkeyImage.gameObject.SetActive(true);
            }
            else if (_data.InteractHotkey == InteractHotkey.RightClick)
            {
                _hotkeyImage.sprite = _rightClickSprite;
                _hotkeyImage.gameObject.SetActive(true);
            }

            _actionImage.sprite = _data.ActionSprite;
        }

        private void Pressed()
        {
            _context.LocalPlayer.InteractLogic.Interact(_data.InteractHotkey);
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