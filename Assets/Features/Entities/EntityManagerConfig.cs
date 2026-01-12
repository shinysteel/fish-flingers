using System;
using UnityEngine;

namespace FishFlingers.Entities
{
    [Serializable]
    public class EntityMapping
    {
        [SerializeField] private EEntity _entity;
        [SerializeField] private GameObject _prefab;

        public EEntity Entity => _entity;
        public GameObject Prefab => _prefab;
    }

    [CreateAssetMenu(fileName = "EntityManagerConfig", menuName = "Configs/Managers/EntityManagerConfig")]
    public class EntityManagerConfig : ScriptableObject
    {
        [SerializeField] private EntityMapping[] _entityMappings;

        public EntityMapping[] EntityMappings => _entityMappings;
    }
}