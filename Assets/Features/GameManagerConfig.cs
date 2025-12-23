using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishFlingers.Networking;
using FishFlingers.Cameras;
using FishFlingers.Levels;
using FishFlingers.States;
using FishFlingers.UI;

[CreateAssetMenu(fileName = "GameManagerConfig", menuName = "Configs/Managers/GameManagerConfig")]
public class GameManagerConfig : ScriptableObject
{
    [SerializeField] private SteamManagerConfig _steamManagerConfig;
    [SerializeField] private NetworkManagerConfig _networkManagerConfig;
    [SerializeField] private CameraManagerConfig _cameraManagerConfig;
    [SerializeField] private LevelManagerConfig _levelManagerConfig;
    [SerializeField] private StateManagerConfig _stateManagerConfig;
    [SerializeField] private UIManagerConfig _uiManagerConfig;

    public SteamManagerConfig SteamManagerConfig => _steamManagerConfig;
    public NetworkManagerConfig NetworkManagerConfig => _networkManagerConfig;
    public CameraManagerConfig CameraManagerConfig => _cameraManagerConfig;
    public LevelManagerConfig LevelManagerConfig => _levelManagerConfig;
    public StateManagerConfig StateManagerConfig => _stateManagerConfig;
    public UIManagerConfig UIManagerConfig => _uiManagerConfig;
}