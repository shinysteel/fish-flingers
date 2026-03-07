using FishFlingers.Items;
using FishFlingers.States;
using UnityEngine;
using ShinyOwl.Common;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "StructureData", menuName = "Data/Entities/Structures/StructureData")]
    public abstract class StructureData : EntityData, IBuildable
    {
        [SerializeField] private Recipe _recipe;

        public EntityData EntityData => this;
        public Recipe Recipe => _recipe;

        public bool TryBuild(GameplayContext context, RaftPlayerTarget target)
        {
            if (target.Tile == null || target.Tile.Structure != null)
            {
                return false;
            }

            context.Raft.SetStructureIdRpc(target.Cell, _id);

            return true;
        }
    }
}