using FishFlingers.Entities;
using FishFlingers.Pools;
using FishFlingers.States;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class BuildingKitPanel : Panel
    {
        [SerializeField] private ScrollRect _blueprintsScrollRect;

        private EntityManager _entityManager;
        private PoolManager _poolManager;

        private BlueprintEntry[] _blueprintEntries;

        public override void Load(Canvas canvas)
        {
            base.Load(canvas);

            _entityManager = GameManager.Instance.Get<EntityManager>();
            _poolManager = GameManager.Instance.Get<PoolManager>();
        }

        public void Setup(GameplayContext context)
        {
            IEnumerable<IBuildable> buildables = context.LocalPlayer.TargetLogic.Target.Tile == null
                ? _entityManager.GetEntities<RaftTile>().Select(tile => tile.Data)
                : _entityManager.GetEntities<Structure>().Select(structure => structure.StructureData);

            _blueprintEntries = new BlueprintEntry[buildables.Count()];

            // Populate the blueprint entries
            int i = 0;
            foreach (IBuildable buildable in buildables)
            {
                BlueprintEntry entry = _poolManager.Get<BlueprintEntry>(new SpawnParams() { Parent = _blueprintsScrollRect.content });
                entry.Setup(context, buildable);

                _blueprintEntries[i] = entry;
                i++;
            }
        }

        public override void Unload()
        {
            base.Unload();

            foreach (BlueprintEntry entry in _blueprintEntries)
            {
                _poolManager.Return(entry);
            }
        }
    }
}