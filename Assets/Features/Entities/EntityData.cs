using FishFlingers.Localisation;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "EntityData", menuName = "Data/Entities/EntityData")]
    public class EntityData : ScriptableObject
    {
        [SerializeField] protected EntityId _id;
        [SerializeField] private LocalisationTerm _nameTerm;
        [SerializeField] private LocalisationTerm _descriptionTerm;
        [SerializeField] private Sprite _sprite;
        [SerializeField] protected int _health = 1;
        [SerializeField] private EntityAlliance _alliance;
        [SerializeField] protected EntityDefeatSettings _entityDefeatSettings;
        [SerializeField] protected EntityPhysicsSettings _entityPhysicsSettings;

        public EntityId Id => _id;
        public LocalisationTerm NameTerm => _nameTerm;
        public LocalisationTerm DescriptionTerm => _descriptionTerm;
        public Sprite Sprite => _sprite;
        public int Health => _health;
        public EntityAlliance Alliance => _alliance;
        public EntityDefeatSettings EntityDefeatSettings => _entityDefeatSettings;
        public EntityPhysicsSettings EntityPhysicsSettings => _entityPhysicsSettings;
    }
}