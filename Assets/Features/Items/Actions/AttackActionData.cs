using FishFlingers.States;
using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.Items
{
    [CreateAssetMenu(fileName = "AttackActionData", menuName = "Data/Items/Actions/AttackActionData")]
    public class AttackActionData : ItemActionData
    {
        public override void Execute(GameplayContext context)
        {
            context.LocalPlayer.AttackLogic.Attack();   
        }
    }
}