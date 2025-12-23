using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.Cameras
{
    [CreateAssetMenu(fileName = "CameraManagerConfig", menuName = "Configs/Managers/CameraManagerConfig")]
    public class CameraManagerConfig : ScriptableObject
    {
        [SerializeField] private Camera _gameCameraPrefab;

        public Camera GameCameraPrefab => _gameCameraPrefab;
    }
}