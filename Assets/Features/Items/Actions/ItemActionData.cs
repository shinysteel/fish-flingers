using FishFlingers.States;
using UnityEngine;

namespace FishFlingers.Items
{
    public abstract class ItemActionData : ScriptableObject
    {
        public abstract void Execute(GameplayContext context);
    }
}