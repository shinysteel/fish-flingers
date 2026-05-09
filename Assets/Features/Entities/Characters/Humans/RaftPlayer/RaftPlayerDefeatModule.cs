using FishFlingers.Pools;
using UnityEngine;
using FishFlingers.Environments;

namespace FishFlingers.Entities
{
    public class RaftPlayerDefeatModule : CharacterDefeatModule
    {
        private RaftPlayer _player;
        

        public RaftPlayerDefeatModule(RaftPlayer player) : base(player)
        {
            _player = player;
        }

        // Don't inherit Tick logic from CharacterDefeatModule
        public override void Tick()
        { }
        
        public override void Defeat()
        {
            _poolManager.GetProp(PropId.Barrel, new SpawnParams() { Parent = _player.transform });

            _isDefeated = true;

            RaiseDefeated();
        }

        protected override void Despawn()
        {
        }
    }
}