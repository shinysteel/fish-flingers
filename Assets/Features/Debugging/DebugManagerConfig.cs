using UnityEngine;

[CreateAssetMenu(fileName = "DebugManagerConfig", menuName = "Configs/Managers/DebugManagerConfig")]
public class DebugManagerConfig : ScriptableObject
{
    [SerializeField] private float _fastForwardTimeScale = 10f;

    public float FastForwardTimeScale => _fastForwardTimeScale;
}