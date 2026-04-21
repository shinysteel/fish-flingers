using UnityEngine;
using System.Threading.Tasks;
using PrimeTween;
using ShinyOwl.Common;
using FishFlingers.Hitboxes;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "RaftPlayerAttackSettings", menuName = "Settings/Entities/RaftPlayerAttackSettings")]
    public class RaftPlayerAttackSettings : ScriptableObject
    {
        [SerializeField] private HitboxData _hitboxData;
        [SerializeField] private float _lungeStrength;

        public HitboxData HitboxData => _hitboxData;
        public float LungeStrength => _lungeStrength;
    }

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

        private RaftPlayerAttackSettings _settings;

        private RaftPlayerAttackState _attackState;
        public RaftPlayerAttackState AttackState => _attackState;
        
        public RaftPlayerAttackLogic(RaftPlayer player)
        {
            _hitboxManager = GameManager.Instance.Get<HitboxManager>();

            _player = player;

            _settings = _player.Data.AttackSettings;
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
                    _player.Rigidbody.AddForce(_player.transform.forward * _settings.LungeStrength, ForceMode.Impulse);

                    _attackState = RaftPlayerAttackState.Impact;

                    _hitboxManager.CreateHitbox(_player.transform.position, _player.transform.rotation, _settings.HitboxData);
                }),
            };

            await _player.AnimateLogic.AttackAsync(events);

            _attackState = RaftPlayerAttackState.None;
        }
    }
}