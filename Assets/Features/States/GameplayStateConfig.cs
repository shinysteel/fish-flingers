using FishFlingers.Environments;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayStateConfig", menuName = "Configs/Managers/State/GameplayStateConfig")]
public class GameplayStateConfig : ScriptableObject
{
    [SerializeField] private Raft _raftPrefab;
    [SerializeField] private WaveSpawner _waveSpawnerPrefab;

    public Raft RaftPrefab => _raftPrefab;
    public WaveSpawner WaveSpawnerPrefab => _waveSpawnerPrefab;
}
