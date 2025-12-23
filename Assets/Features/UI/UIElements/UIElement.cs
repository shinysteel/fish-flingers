using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.UI
{
    public abstract class UIElement : MonoBehaviour
    {
        protected bool _isVisible;

        public virtual void Load()
        { }

        public virtual void Show()
        {
            _isVisible = true;
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            _isVisible = false;
            gameObject.SetActive(false);
        }

        public virtual void Unload()
        { }
    }
}