using FishFlingers.Items;
using FishFlingers.Localisation;
using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "EntityDefinitionData", menuName = "Data/Entities/EntityDefinitionData")]
    public class EntityDefinitionData : DefinitionData
    {
        [SerializeField] protected EntityId _id;
        [SerializeField] protected int _health = 1;
        [SerializeField] private EntityAlliance _alliance;
        [SerializeField] private DropTable[] _dropTables;

        public EntityId Id => _id;
        public int Health => _health;
        public EntityAlliance Alliance => _alliance;
        public DropTable[] DropTables => _dropTables;
    }
}