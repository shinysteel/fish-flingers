using FishFlingers.States;
using UnityEngine;

namespace FishFlingers.Items
{
    public abstract class ItemActionData : ScriptableObject
    {
        [SerializeField] protected InteractHotkey _interactHotkey;
        [SerializeField] private Sprite _actionSprite;

        public InteractHotkey InteractHotkey => _interactHotkey;
        public Sprite ActionSprite => _actionSprite;

        public abstract void Execute(GameplayContext context);
    }
}