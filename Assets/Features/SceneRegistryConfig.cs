using System;
using UnityEngine;

[Serializable]
public class SceneMapping
{
    [SerializeField] private EScene _enum;
    [SerializeField] private string _name;

    public EScene Enum => _enum;
    public string Name => _name;
}

[CreateAssetMenu(fileName = "SceneRegistryConfig", menuName = "Configs/SceneRegistryConfig")]
public class SceneRegistryConfig : ScriptableObject
{
    [SerializeField] private SceneMapping[] _sceneMappings;

    public SceneMapping[] SceneMappings => _sceneMappings;
}
