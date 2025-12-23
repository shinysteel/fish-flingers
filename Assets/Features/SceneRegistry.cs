using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Tried doing a config but scenes can't serialise in the inspector.
/// Would be nice to find a workaround. This enum solution isn't great,
/// since we can't use space bars for scene names. Ideal solution is a config
/// with SceneMappings (enum -> serialised scene)
/// </summary>

// Needs to be 1:1 with scene names
public enum EScene
{
    Startup,
    Game   ,
}

public static class SceneRegistry
{
    private static Dictionary<EScene, string> _registry;

    public static string GetSceneName(EScene scene)
    {
        return _registry[scene];
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void OnSubsystemRegistration()
    {
        _registry = new();

        foreach (EScene e in Enum.GetValues(typeof(EScene)))
        {
            _registry.Add(e, e.ToString());
        }
    }
}
