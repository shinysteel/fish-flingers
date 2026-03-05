using FishFlingers.Items;
using FishFlingers.States;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "StructureData", menuName = "Data/Entities/Structures/StructureData")]
    public abstract class StructureData : EntityData, IBuildable
    {
        [SerializeField] private Recipe _recipe;

        public EntityData EntityData => this;
        public Recipe Recipe => _recipe;

        public void Build(GameplayContext context, RaftPlayerTarget target)
        {
            
        }
    }
}