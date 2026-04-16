using UnityEngine;

namespace FishFlingers.Entities
{
    public class RaftPlayerAttackLogic
    {
        private RaftPlayer _player;

        public RaftPlayerAttackLogic(RaftPlayer player)
        {
            _player = player;
        }

        public void Attack()
        {
            _ = _player.AnimateLogic.Attack();
        }
    }
}