using FishFlingers.Inventories;
using FishFlingers.States;
using PrimeTween;
using ShinyOwl.Common.Utils;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class ItemActionsView : MonoBehaviour
    {
        // Visualises an action that can be executed
        [Serializable]
        private class ActionView
        {
            [SerializeField] private Button _button;
            [SerializeField] private RectTransform _rectTransform;
            [SerializeField] private GameObject _container;

            public Button Button => _button;

            private Tween _showTween;

            private const float ShowDuration = 0.1f;

            public void Setup()
            {
                Utils.UI.StretchToParent(_rectTransform);
            }

            public void Show(bool show)
            {
                _container.gameObject.SetActive(show);

                if (show)
                {
                    _showTween = Tween.UIAnchoredPosition(_rectTransform, endValue: Vector2.zero, duration: ShowDuration, ease: Ease.OutBack);
                }
                else
                {
                    _showTween.Stop();
                    _rectTransform.anchoredPosition = Vector2.down * _rectTransform.rect.size.y * 2f;
                }
            }
        }

        // If there are more actions in the future we should consider a list, but for now this is reasonable
        [SerializeField] private ActionView _leftClickActionView;
        [SerializeField] private ActionView _rightClickActionView;

        private UIManager _uiManager;

        private GameplayContext _context;

        private InventoryItem _selectedItem;

        private void Awake()
        {
            _uiManager = GameManager.Instance.Get<UIManager>();
        }

        public void Setup(GameplayContext context)
        {
            _context = context;

            HandleHotbarSelectedChanged(_context.LocalPlayer.Hotbar.SelectedIndex, _context.LocalPlayer.Hotbar.GetSelected());
            _context.LocalPlayer.Hotbar.OnSelectedChanged += HandleHotbarSelectedChanged;

            _leftClickActionView.Setup();
            _rightClickActionView.Setup();

            _leftClickActionView.Button.onClick.AddListener(LeftPressed);
            _rightClickActionView.Button.onClick.AddListener(RightPressed);
        }

        private void OnDestroy()
        {
            if (_context.LocalPlayer?.Hotbar != null)
            {
                _context.LocalPlayer.Hotbar.OnSelectedChanged -= HandleHotbarSelectedChanged;
            }
        }

        private void HandleHotbarSelectedChanged(int index, InventoryItem item)
        {
            // _selectedItem denotes what actions can be executed
            _selectedItem = item;

            _leftClickActionView.Show(item?.ItemInstance.Data.LeftClickAction != null);
            _rightClickActionView.Show(item?.ItemInstance.Data.RightClickAction != null);
        }

        private void Update()
        {
            SimulatePressedUpdate();
        }

        private void SimulatePressedUpdate()
        {
            // Note that the repeated check here stops the simulated presses. There's a reason we repeat it, since
            // you can still press the buttons via the mouse. We aren't stopping that, and rather just not simulating
            // the presses when it's not reasonable
            if (_uiManager.IsLayerInUse(UILayer.Panels))
            {
                return;
            }

            if (_leftClickActionView.Button.gameObject.activeSelf && _context.LocalPlayer.InputLogic.LeftClick)
            {
                Utils.UI.SimulatePressed(_leftClickActionView.Button);
            }

            if (_rightClickActionView.Button.gameObject.activeSelf && _context.LocalPlayer.InputLogic.RightClick)
            {
                Utils.UI.SimulatePressed(_rightClickActionView.Button);
            }
        }

        private void LeftPressed()
        {
            if (_uiManager.IsLayerInUse(UILayer.Panels))
            {
                return;
            }

            _selectedItem.ItemInstance.Data.LeftClickAction.Execute(_context);   
        }

        private void RightPressed()
        {
            if (_uiManager.IsLayerInUse(UILayer.Panels))
            {
                return;
            }

            _selectedItem.ItemInstance.Data.RightClickAction.Execute(_context);
        }
    }
}