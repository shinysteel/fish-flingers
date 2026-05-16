using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers
{
    public static class ActionHotkeyUtils
    {
        public static void Apply(ActionHotkey hotkey, TextMeshProUGUI text, Image image, Sprite leftClickSprite, Sprite rightClickSprite)
        {
            text.gameObject.SetActive(false);
            image.gameObject.SetActive(false);

            if (hotkey == ActionHotkey.FKey)
            {
                text.text = "F";
                text.gameObject.SetActive(true);
            }
            else if (hotkey == ActionHotkey.LeftClick)
            {
                image.sprite = leftClickSprite;
                image.gameObject.SetActive(true);
            }
            else if (hotkey == ActionHotkey.RightClick)
            {
                image.sprite = rightClickSprite;
                image.gameObject.SetActive(true);
            }
        }
    }

    public enum ActionHotkey
    {
        None,
        FKey,
        LeftClick,
        RightClick
    }
}