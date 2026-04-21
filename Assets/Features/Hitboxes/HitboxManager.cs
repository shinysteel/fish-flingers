using FishFlingers.Entities;
using FishFlingers.Pools;
using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.Hitboxes
{
    public interface IHitboxManagerListener
    { }

    public class HitboxManager : GameSystem<IHitboxManagerListener>
    {
        private PoolManager _poolManager;

        private HitboxManagerConfig _config;

        public override void Initialise(GameManagerConfig config)
        {
            _poolManager = GameManager.Instance.Get<PoolManager>();

            _config = config.HitboxManagerConfig;

            base.Initialise(config);
        }

        public void CreateHitbox(HitboxParams parameters)
        {
            Hitbox hitbox = _poolManager.GetPoolable<Hitbox>(new());
            hitbox.Initialise(parameters);
        }
    }
}