using FishFlingers.Pools;
using UnityEngine;
using FishFlingers.Environments;

namespace FishFlingers.Entities
{
    public class RaftPlayerDefeatModule : CharacterDefeatModule
    {
        public RaftPlayer Player => (RaftPlayer)_entity;

        public RaftPlayerDefeatModule(RaftPlayer player) : base(player)
        { 
            
        }

        public override void Defeat()
        {
        }

        protected override void Despawn()
        {
        }
    }
}