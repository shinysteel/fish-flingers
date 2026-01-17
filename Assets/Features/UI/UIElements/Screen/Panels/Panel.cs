using FishFlingers.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public abstract class Panel : ScreenUI
    {
        [SerializeField] private Button _closeButton;

        public override void Load(Canvas canvas)
        {
            base.Load(canvas);

            _closeButton.onClick.AddListener(ClosePressed);
        }

        private void ClosePressed()
        {
            // Not every panel will need a reference to UIManager, so this is a one off
            // If more references came up, than I would consider defining all manager refs here
            GameManager.Instance.Get<UIManager>().PopLayer(UILayer.Panels);
        }
    }
}