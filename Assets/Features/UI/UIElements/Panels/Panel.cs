using FishFlingers.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public abstract class Panel : UIElement
    {
        [SerializeField] private Button _closeButton;

        protected UIManager _uiManager;
        protected NetworkManager _networkManager;

        public override void Load()
        {
            _uiManager = GameManager.Instance.Get<UIManager>();
            _networkManager = GameManager.Instance.Get<NetworkManager>();

            _closeButton.onClick.AddListener(ClosePressed);
        }

        private void ClosePressed()
        {
            _uiManager.DestroyUIElement(this, UILayer.Panels);
        }
    }
}