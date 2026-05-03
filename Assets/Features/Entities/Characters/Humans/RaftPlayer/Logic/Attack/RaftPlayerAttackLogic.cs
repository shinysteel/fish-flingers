using UnityEngine;
using System.Threading.Tasks;
using PrimeTween;
using ShinyOwl.Common;
using FishFlingers.Hitboxes;
using FishFlingers.Pools;

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
        private PoolManager _poolManager;

        private RaftPlayer _player;

        private RaftPlayerAttackSettings _settings;

        private RaftPlayerAttackState _attackState;
        public RaftPlayerAttackState AttackState => _attackState;
        
        public RaftPlayerAttackLogic(RaftPlayer player)
        {
            _poolManager = GameManager.Instance.Get<PoolManager>();

            _player = player;

            _settings = _player.DefinitionData.AttackSettings;

            if (_player.isOwner)
            {
                _player.AnimateLogic.AttackStateAnimationEvents.Add(new StateAnimationEvent(0f, () => _attackState = RaftPlayerAttackState.Windup));
                _player.AnimateLogic.AttackStateAnimationEvents.Add(new StateAnimationEvent(0.5f, Lunge));
                _player.AnimateLogic.AttackStateAnimationEvents.Add(new StateAnimationEvent(1f, () => _attackState = RaftPlayerAttackState.None));
            }
        }

        public void Attack()
        {
            if (_attackState > RaftPlayerAttackState.None)
            {
                return;
            }

            _player.AnimateLogic.Attack();
        }

        private void Lunge()
        {
            _attackState = RaftPlayerAttackState.Impact;
            _player.Rigidbody.AddForce(_player.transform.forward * _settings.LungeStrength, ForceMode.Impulse);
            Hitbox hitbox = _poolManager.GetPoolable<Hitbox>(new SpawnParams() { Position = _player.transform.position, Rotation = _player.transform.rotation });
            hitbox.Initialise(_settings.HitboxData);
        }
    }
}