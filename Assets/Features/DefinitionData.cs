using FishFlingers.Items;
using FishFlingers.Localisation;
using FishFlingers.Rarities;
using UnityEngine;

namespace FishFlingers
{
    public abstract class DefinitionData : ScriptableObject
    {
        [SerializeField] private LocalisationTerm _nameTerm;
        [SerializeField] private LocalisationTerm _descriptionTerm;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Rarity _rarity;

        public LocalisationTerm NameTerm => _nameTerm;
        public LocalisationTerm DescriptionTerm => _descriptionTerm;
        public Sprite Sprite => _sprite;
        public Rarity Rarity => _rarity;
    }
}