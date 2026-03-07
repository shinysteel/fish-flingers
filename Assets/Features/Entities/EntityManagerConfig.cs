using System;
using UnityEngine;

namespace FishFlingers.Entities
{
    [Serializable]
    public class EntityMapping
    {
        [SerializeField] private EntityId _id;
        [SerializeField] private GameObject _prefab;

        public EntityId Id => _id;
        public GameObject Prefab => _prefab;
    }

    [CreateAssetMenu(fileName = "EntityManagerConfig", menuName = "Configs/Managers/EntityManagerConfig")]
    public class EntityManagerConfig : ScriptableObject
    {
        [SerializeField] private EntityMapping[] _entityMappings;

        public EntityMapping[] EntityMappings => _entityMappings;
    }
}