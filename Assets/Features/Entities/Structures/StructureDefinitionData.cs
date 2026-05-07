using FishFlingers.Items;
using FishFlingers.States;
using UnityEngine;
using ShinyOwl.Common;

namespace FishFlingers.Entities
{
    public abstract class StructureDefinitionData : EntityDefinitionData, IBuildable
    {
        [SerializeField] private Recipe _recipe;

        public DefinitionData DefinitionData => this;
        public Recipe Recipe => _recipe;

        public bool TryBuild(GameplayContext context, RaftPlayerTileTarget target)
        {
            if (!target.CanBuildStructure())
            {
                return false;
            }

            context.Raft.AddStructureRpc(target.Cell, _id);

            return true;
        }
    }
}