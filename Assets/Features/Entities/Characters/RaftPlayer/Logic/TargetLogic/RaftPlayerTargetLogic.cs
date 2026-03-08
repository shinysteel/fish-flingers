using FishFlingers.Inventories;
using FishFlingers.States;
using PrimeTween;
using PurrNet;
using ShinyOwl.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FishFlingers.Entities
{
    public class RaftPlayerTarget
    {
        private Vector2Int _cell;
        private RaftTile _tile;
        private Structure _structure;

        public Vector2Int Cell => _cell;
        public RaftTile Tile => _tile;
        public Structure Structure => _structure;

        public event Action OnChanged;

        public RaftPlayerTarget()
        {
            _cell = Vector2Int.one * int.MinValue;
        }

        public void SetCell(Vector2Int cell)
        {
            if (_cell == cell)
            {
                return;
            }

            _cell = cell;
            NotifyOnChanged();
        }

        public void SetTile(RaftTile tile)
        {
            if (_tile == tile)
            {
                return;
            }

            _tile = tile;
            NotifyOnChanged();
        }

        public void SetStructure(Structure structure)
        {
            if (_structure == structure)
            {
                return;
            }

            _structure = structure;
            NotifyOnChanged();
        }

        private void NotifyOnChanged()
        {
            OnChanged?.Invoke();
        }

        public bool CanBuild()
        {
            return CanBuildTile() || CanBuildStructure();
        }

        public bool CanBuildTile()
        {
            return _tile == null;
        }

        public bool CanBuildStructure()
        {
            return _tile != null && _structure == null;
        }
    }

    public class RaftPlayerTargetLogic
    {
        private GameplayContext _context;

        private RaftPlayerTargetLogicSettings _settings;

        private RaftPlayerTargetVisual _targetVisual;

        private RaftPlayerTarget _target;
        public RaftPlayerTarget Target => _target;

        private bool _showingTarget;

        private const float Range = 1f;

        public event Action<RaftPlayerTarget> OnTargetChanged;

        // Scales for the target depending on context
        private static readonly Vector3 StructureVisualScale = new Vector3(0.75f, 0.25f, 0.75f);
        private static readonly Vector3 TileVisualScale = new Vector3(1f, 0.25f, 1f);

        private Tween _fadeTween;
        private const float FadeDuration = 0.1f;

        private const float VisualMaxAlpha = 0.4f; // Equivalent to ~102 in color32

        public RaftPlayerTargetLogic(GameplayContext context, RaftPlayerTargetLogicSettings settings)
        {
            _context = context;

            _settings = settings;

            _targetVisual = Object.Instantiate(_settings.TargetVisualPrefab);

            _target = new();
            _target.OnChanged += HandleTargetChanged;

            _context.Raft.OnTileChanged += HandleTileChanged;
            _context.Raft.OnStructureChanged += HandleStructureChanged;

            HandleHotbarSelectedItemChanged(_context.LocalPlayer.Hotbar.SelectedIndex, _context.LocalPlayer.Hotbar.SelectedItem);
            _context.LocalPlayer.Hotbar.OnSelectedChanged += HandleHotbarSelectedItemChanged;
        }

        ~RaftPlayerTargetLogic()
        { 
            if (_context.Raft != null)
            {
                _context.Raft.OnTileChanged -= HandleTileChanged;
            }

            if (_context.LocalPlayer != null)
            {
                _context.LocalPlayer.Hotbar.OnSelectedChanged -= HandleHotbarSelectedItemChanged;
            }
        }

        private void HandleTargetChanged()
        {
            if (_showingTarget)
            {
                RefreshVisualColor();
            }

            // Passes along the event from Target -> Logic -> Listener
            OnTargetChanged?.Invoke(_target);
        }

        private void HandleTileChanged(Vector2Int cell, RaftTile tile)
        {
            if (cell != _target.Cell)
            {
                return;
            }

            _target.SetTile(tile);
        }

        private void HandleStructureChanged(Vector2Int cell, Structure structure)
        {
            if (cell != _target.Cell)
            {
                return;
            }

            _target.SetStructure(structure);
        }

        private void HandleHotbarSelectedItemChanged(int index, InventoryItem item)
        {
            _showingTarget = item?.ItemInstance.Data.ShowsTarget ?? false;

            _fadeTween.Stop();

            // Fade in or out the target based on if we are using it or not
            float startValue = _targetVisual.Material.color.a;
            float endValue = _showingTarget ? VisualMaxAlpha : 0f;
            Action<float> onValueChange = (float alpha) => _targetVisual.SetAlpha(alpha);

            _fadeTween = Tween.Custom(startValue: startValue, endValue: endValue, duration: FadeDuration, onValueChange: onValueChange);

            if (_showingTarget)
            {
                _targetVisual.gameObject.SetActive(true);    
            }
            else
            {
                _fadeTween.OnComplete(() => _targetVisual.gameObject.SetActive(false));
            }
        }

        public void Tick()
        {
            // Targets become locked when you can't act
            if (_context.LocalPlayer.CanAct)
            {
                DetermineTargetTick();
            }

            // _isTargeting represents if the selected item displays a target
            if (_showingTarget)
            {
                TransformVisualTick();
            }
        }

        private void DetermineTargetTick()
        {
            Vector3 forward = _context.LocalPlayer.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            // Target the cell x units away from us in the direction we are facing
            Vector3 position = _context.LocalPlayer.transform.position + forward * Range;

            Vector2Int cell = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));

            // We only care if the cell has changed
            if (_target.Cell == cell)
            {
                return;
            }

            _context.Raft.Tiles.TryGetValue(cell, out RaftTile tile);
            _context.Raft.Structures.TryGetValue(cell, out Structure structure);

            _target.SetCell(cell);
            _target.SetTile(tile);
            _target.SetStructure(structure);
        }

        /// <summary>
        /// Transforms the visual based on whether we are targeting a tile or not
        /// </summary>
        private void TransformVisualTick()
        {
            Vector3 scale;
            Vector3 position = _context.Raft.CellToWorldPosition(_target.Cell);

            if (_target.Tile != null)
            {
                scale = StructureVisualScale;
                position.y = _target.Tile.GetSurfaceY() + scale.y * 0.5f;
            }
            else
            {
                scale = TileVisualScale;
                position.y = 0f;
            }

            _targetVisual.SetVisualScale(scale);
            _targetVisual.transform.position = position;
        }

        private void RefreshVisualColor()
        {
            _targetVisual.SetColor(_target.CanBuild() ? _settings.ValidColor : _settings.InvalidColor);
        }
    }
}