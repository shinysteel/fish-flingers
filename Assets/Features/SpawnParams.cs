using FishFlingers.Scenes;
using UnityEngine;

public class SpawnParams
{
    private SceneManager _sceneManager;

    public Vector3 Position { get; set; } = Vector3.zero;
    public Quaternion Rotation { get; set; } = Quaternion.identity;
    public Transform Parent { get; set; } = null;
    public SpawnScene SpawnScene { get; set; } = SpawnScene.ActiveScene();

    public SpawnParams()
    {
        _sceneManager = GameManager.Instance.Get<SceneManager>();
    }

    public T Spawn<T>(T prefab) where T : Component
    {
        T component = Object.Instantiate(prefab, Position, Rotation, Parent);

        if (Parent == null)
        {
            _sceneManager.MoveGameObjectToScene(component.gameObject, SpawnScene.Get());
        }

        return component;
    }
}
