using UnityEngine;
using System.Threading.Tasks;
using PrimeTween;
using ShinyOwl.Common;
using FishFlingers.Hitboxes;

namespace FishFlingers.Entities
{
    public enum RaftPlayerAttackState
    {
        None,
        Windup,
        Impact
    }

    public class RaftPlayerAttackLogic
    {
        private HitboxManager _hitboxManager;

        private RaftPlayer _player;

        private RaftPlayerAttackState _attackState;
        public RaftPlayerAttackState AttackState => _attackState;
        
        public RaftPlayerAttackLogic(RaftPlayer player)
        {
            _hitboxManager = GameManager.Instance.Get<HitboxManager>();

            _player = player;
        }

        public async Task AttackAsync()
        {
            if (_attackState > RaftPlayerAttackState.None)
            {
                return;
            }

            _attackState = RaftPlayerAttackState.Windup;

            AnimateEvents events = new AnimateEvents()
            {
                new AnimateEvent(0.5f, () =>
                {
                    _player.Rigidbody.AddForce(_player.transform.forward, ForceMode.Impulse);
                    _attackState = RaftPlayerAttackState.Impact;

                    _hitboxManager.CreateHitbox(new HitboxParams(_player.transform.position, 2.5f, 1, 2.5f, 0.5f, EntityAlliance.Ally));
                }),
            };

            await _player.AnimateLogic.AttackAsync(events);

            _attackState = RaftPlayerAttackState.None;
        }
    }
}