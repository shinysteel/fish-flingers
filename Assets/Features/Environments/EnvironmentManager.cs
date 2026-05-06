using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.Environments
{
    public interface IEnvironmentManagerListener
    { }

    public class EnvironmentManager : GameSystem<IEnvironmentManagerListener>
    {
        private EnvironmentManagerConfig _config;

        private Dictionary<PropId, Prop> _idPrefabMap = new();

        public override void Initialise(GameManagerConfig config)
        {
            _config = config.EnvironmentManagerConfig;

            foreach (Prop prop in _config.PropScanner.GetAssets())
            {
                _idPrefabMap.Add(prop.Id, prop);
            }

            base.Initialise(config);
        }

        public Prop GetPropPrefab(PropId id)
        {
            return _idPrefabMap[id];
        }
    }
}