using FishFlingers.Pools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class InteractPromptUI : WorldUI
    {
        [SerializeField] private ActionHotkeyView _actionHotkeyView;

        protected PoolManager _poolManager;

        private void Awake()
        {
            _poolManager = GameManager.Instance.Get<PoolManager>();
        }

        public void SetupInteract(ActionHotkey hotkey)
        {
            _actionHotkeyView.Set(hotkey);
        }
    }
}