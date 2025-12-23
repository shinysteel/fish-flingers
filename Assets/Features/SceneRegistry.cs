using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Couldn't figure out how to serialise scenes in inspector. This is as good
/// as it gets for now, but its not great having to manually enter and maintain
/// the scene as a string
/// </summary>

// Try to keep these 1:1 with scene names for clarity
public enum EScene
{
    Startup             ,
    Game                ,
    EnvironmentMainMenu ,
    EnvironmentGameplay ,
}

public static class SceneRegistry
{
    private static SceneRegistryConfig _config;

    private static Dictionary<EScene, string> _registry;

    private const string ConfigPath = "Configs/SceneRegistryConfig";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void OnSubsystemRegistration()
    {
        _config = Resources.Load<SceneRegistryConfig>(ConfigPath);

        _registry = new();

        foreach (SceneMapping mapping in _config.SceneMappings)
        {
            _registry.Add(mapping.Enum, mapping.Name);
        }
    }

    public static string GetSceneName(EScene scene)
    {
        return _registry[scene];
    }
}
