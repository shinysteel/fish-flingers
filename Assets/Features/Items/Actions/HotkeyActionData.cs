using FishFlingers.States;
using UnityEngine;

namespace FishFlingers.Items
{
    [CreateAssetMenu(fileName = "HotkeyActionData", menuName = "Data/Items/Actions/HotkeyActionData")]
    public class HotkeyActionData : ItemActionData
    {
        public override void Execute(GameplayContext context)
        {
            context.LocalPlayer.InteractLogic.Interact(_hotkey);
        }
    }
}