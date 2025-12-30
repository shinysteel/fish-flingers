using FishFlingers.Environments;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayStateConfig", menuName = "Configs/Managers/State/GameplayStateConfig")]
public class GameplayStateConfig : ScriptableObject
{
    [SerializeField] private Raft _raftPrefab;

    public Raft RaftPrefab => _raftPrefab;
}
