using System;
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

        public virtual void Show(Action onComplete)
        {
            _isVisible = true;
            gameObject.SetActive(true);
            onComplete?.Invoke();
        }

        public virtual void Hide(Action onComplete)
        {
            _isVisible = false;
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }

        public virtual void Unload()
        { }
    }
}