using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.Levels
{
    public interface ILevelManagerListener
    { }

    public class LevelManager : GameSystem<ILevelManagerListener>
    {
        private LevelManagerConfig _config;

        public override void Initialise(GameManagerConfig gameManagerConfig)
        {
            _config = gameManagerConfig.LevelManagerConfig;

            base.Initialise(gameManagerConfig);
        }
    }
}