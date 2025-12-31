using PurrNet.Prediction;
using UnityEngine;

namespace FishFlingers.Entities
{
    public class FlyingFish : PredictedIdentity<FlyingFish.State>
    {
        public struct State : IPredictedData<State>
        {
            public void Dispose() { }
        }

        protected override void SimulationStart()
        {
            // randomly choose a tile to target
            // choose a direction and iterate 
        }
    }
}