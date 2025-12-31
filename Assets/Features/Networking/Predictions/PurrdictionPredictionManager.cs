using UnityEngine;
using FishFlingers.Networking.Predictions;

/// <summary>
/// Wrapper for Purrdiction's prediction manager, so that we can override some callbacks
/// </summary>
public class PurrdictionPredictionManager : PurrNet.Prediction.PredictionManager
{
    private PredictionManager _predictionManager;

    protected override void OnInitializeModules()
    {
        _predictionManager = GameManager.Instance.Get<PredictionManager>();
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();

        _predictionManager.RegisterPurrdictionPredictionManager(this);
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();

        _predictionManager?.UnregisterPurrdictionPredictionManager(this);
    }
}
