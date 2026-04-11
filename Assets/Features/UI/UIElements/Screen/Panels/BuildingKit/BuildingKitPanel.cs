using FishFlingers.Entities;
using FishFlingers.Inventories;
using FishFlingers.Pools;
using FishFlingers.States;
using ShinyOwl.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using FishFlingers.Items;

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
            _context.LocalPlayer.TileTargetLogic.OnTargetChanged += HandleRaftPlayerTileTargetChanged;

            _context.LocalPlayer.Hotbar.OnSelectedChanged += HandleHotbarSelectedChanged;
        }

        public override void Unload()
        {
            if (_context.LocalPlayer != null)
            {
                _context.LocalPlayer.TileTargetLogic.OnTargetChanged -= HandleRaftPlayerTileTargetChanged;

                _context.LocalPlayer.Hotbar.OnSelectedChanged -= HandleHotbarSelectedChanged;
            }

            foreach (BlueprintEntry entry in _blueprintEntries)
            {
                _poolManager.ReturnPoolable(entry);
            }
        }

        private void HandleRaftPlayerTileTargetChanged(RaftPlayerTileTarget target)
        {
            if (!target.CanBuild())
            {
                if (_isShowing)
                {
                    ClosePressed();
                }

                return;
            }

            RefreshEntries();
        }

        private void RefreshEntries()
        {
            // We populate the entries with either tiles or structures depending on the target
            IEnumerable<IBuildable> buildables = _context.LocalPlayer.TileTargetLogic.Target.Tile == null
                ? _entityManager.GetEntityPrefabs<Tile>().Select(tile => tile.Data)
                : _entityManager.GetEntityPrefabs<Structure>().Select(structure => structure.StructureData);

            Utils.Collections.ResizeList(_blueprintEntries, buildables.Count(),
                createElement: () => _poolManager.GetPoolable<BlueprintEntry>(new SpawnParams() { Parent = _blueprintsScrollRect.content }),
                removeElement: (BlueprintEntry entry) => _poolManager.ReturnPoolable(entry),
                processElement: (BlueprintEntry entry, int index) => entry.Setup(_context, buildables.ElementAt(index)));
        }
        
        private void HandleHotbarSelectedChanged(HotbarSlot slot)
        {
            // There's a scenario where you aren't holding a hammer anymore while this is open
            if (slot.InventoryItem?.ItemInstance.Data.ItemId != ItemId.Hammer)
            {
                ClosePressed();
            }
        }
    }
}