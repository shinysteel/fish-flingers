using FishFlingers.Networking;
using ShinyOwl.Common;
using ShinyOwl.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public abstract class Panel : ScreenUI
    {
        [SerializeField] protected Button _closeButton;

        public override void Load(Canvas canvas)
        {
            base.Load(canvas);

            _closeButton.onClick.AddListener(ClosePressed);
        }

        protected void ClosePressed()
        {
            // Not every panel will need a reference to UIManager, so this is a one off
            // If more references came up, then I would consider defining all manager refs here
            Hide(() => GameManager.Instance.Get<UIManager>().DestroyScreenUI(this, UILayer.Panels));
        }

        public void SimulateClosePressed()
        {
            Utils.UI.SimulatePressed(_closeButton);
        }
    }
}