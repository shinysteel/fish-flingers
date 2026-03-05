using FishFlingers.Entities;
using FishFlingers.Pools;
using FishFlingers.States;
using ShinyOwl.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class BuildingKitPanel : Panel
    {
        [SerializeField] private ScrollRect _blueprintsScrollRect;

        private EntityManager _entityManager;
        private PoolManager _poolManager;

        private GameplayContext _context;

        private List<BlueprintEntry> _blueprintEntries = new();

        public override void Load(Canvas canvas)
        {
            base.Load(canvas);

            _entityManager = GameManager.Instance.Get<EntityManager>();
            _poolManager = GameManager.Instance.Get<PoolManager>();
        }

        public void Setup(GameplayContext context)
        {
            _context = context;

            RefreshEntries();
            _context.LocalPlayer.TargetLogic.OnTargetChanged += HandleRaftPlayerTargetChanged;
        }

        public override void Unload()
        {
            if (_context.LocalPlayer != null)
            {
                _context.LocalPlayer.TargetLogic.OnTargetChanged -= HandleRaftPlayerTargetChanged;
            }

            foreach (BlueprintEntry entry in _blueprintEntries)
            {
                _poolManager.Return(entry);
            }
        }

        private void HandleRaftPlayerTargetChanged(RaftPlayerTarget target)
        {
            RefreshEntries();
        }

        private void RefreshEntries()
        {
            // We populate the entries with either tiles or structures depending on the target
            IEnumerable<IBuildable> buildables = _context.LocalPlayer.TargetLogic.Target.Tile == null
                ? _entityManager.GetEntities<RaftTile>().Select(tile => tile.Data)
                : _entityManager.GetEntities<Structure>().Select(structure => structure.StructureData);

            Utils.Collections.ResizeList(_blueprintEntries, buildables.Count(),
                createElement: () => _poolManager.Get<BlueprintEntry>(new SpawnParams() { Parent = _blueprintsScrollRect.content }),
                removeElement: (BlueprintEntry entry) => _poolManager.Return(entry),
                processElement: (BlueprintEntry entry, int index) => entry.Setup(_context, buildables.ElementAt(index)));
        }
    }
}