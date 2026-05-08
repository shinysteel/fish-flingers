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

        // Don't inherit Tick logic from CharacterDefeatModule
        public override void Tick()
        { }
        
        public override void Defeat()
        {
            _poolManager.GetProp(PropId.Barrel, new SpawnParams() { Parent = Player.transform });

            _isDefeated = true;

            RaiseDefeated();
        }

        protected override void Despawn()
        {
        }
    }
}