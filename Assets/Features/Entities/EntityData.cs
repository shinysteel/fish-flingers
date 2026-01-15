using FishFlingers.Localisation;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "EntityData", menuName = "Data/Entities/EntityData")]
    public class EntityData : ScriptableObject
    {
        [SerializeField] private LocalisationTerm _nameTerm;
        [SerializeField] private LocalisationTerm _descriptionTerm;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private int _health = 1;

        public LocalisationTerm NameTerm => _nameTerm;
        public LocalisationTerm DescriptionTerm => _descriptionTerm;
        public Sprite Sprite => _sprite;
        public int Health => _health;
    }
}