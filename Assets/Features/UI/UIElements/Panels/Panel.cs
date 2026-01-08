using FishFlingers.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public abstract class Panel : UIElementAnimated
    {
        [SerializeField] private Button _closeButton;

        public override void Load()
        {
            _closeButton.onClick.AddListener(ClosePressed);
        }

        private void ClosePressed()
        {
            // Not every panel will need a reference to UIManager, so this is a one off
            GameManager.Instance.Get<UIManager>().PopLayer(UILayer.Panels);
        }
    }
}