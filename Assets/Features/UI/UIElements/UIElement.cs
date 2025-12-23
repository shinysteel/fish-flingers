using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.UI
{
    public abstract class UIElement : MonoBehaviour
    {
        public virtual void Load()
        { }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Unload()
        { }
    }
}