using FishFlingers.Items;
using FishFlingers.Localisation;
using UnityEngine;

namespace FishFlingers
{
    public abstract class DefinitionData : ScriptableObject
    {
        [SerializeField] private LocalisationTerm _nameTerm;
        [SerializeField] private LocalisationTerm _descriptionTerm;
        [SerializeField] private Sprite _sprite;

        public LocalisationTerm NameTerm => _nameTerm;
        public LocalisationTerm DescriptionTerm => _descriptionTerm;
        public Sprite Sprite => _sprite;
    }
}