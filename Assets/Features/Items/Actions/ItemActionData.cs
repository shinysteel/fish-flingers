using FishFlingers.States;
using UnityEngine;

namespace FishFlingers.Items
{
    public abstract class ItemActionData : ScriptableObject
    {
        [SerializeField] private Sprite _sprite;

        public Sprite Sprite => _sprite;

        public abstract void Execute(GameplayContext context);
    }
}