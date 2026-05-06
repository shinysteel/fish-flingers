using UnityEngine;

namespace FishFlingers.Entities
{
    public class CharacterEffectsModule : EntityEffectsModule
    {
        public Character Character => (Character)_entity;

        public CharacterEffectsModule(Character character) : base(character)
        { }

        public override void AnimateHurt()
        {
            Character.CharacterModel.AnimateHurt();
        }
    }
}