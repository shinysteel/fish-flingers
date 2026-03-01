using FishFlingers.Inventories;
using FishFlingers.States;
using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.Entities
{
    public class TargetLogic
    {
        private GameplayContext _context;

        private Vector2Int _targetCell;
        public Vector2Int TargetCell => _targetCell;

        private Target _target;

        private const float Range = 1f;

        private static readonly Vector3 StructureVisualScale = new Vector3(0.66f, 0.25f, 0.66f);
        private static readonly Vector3 TileVisualScale = new Vector3(1f, 0.25f, 1f);

        public TargetLogic(GameplayContext context, Target targetPrefab)
        {
            _context = context;

            _target = Object.Instantiate(targetPrefab);

            HandleHotbarSelectedItemChanged(_context.LocalPlayer.Hotbar.SelectedIndex, _context.LocalPlayer.Hotbar.GetSelected());
            _context.LocalPlayer.Hotbar.OnSelectedChanged += HandleHotbarSelectedItemChanged;
        }

        ~TargetLogic()
        {
            if (_context.LocalPlayer?.Hotbar != null)
            {
                _context.LocalPlayer.Hotbar.OnSelectedChanged -= HandleHotbarSelectedItemChanged;
            }
        }

        private void HandleHotbarSelectedItemChanged(int index, InventoryItem item)
        {
            _target.gameObject.SetActive(item?.ItemInstance.Data.DisplayTarget ?? false);
        }

        public void Tick()
        {
            if (!_target.gameObject.activeSelf)
            {
                return;
            }

            DetermineTargetTick();
            TransformVisualTick();
        }

        private void DetermineTargetTick()
        {
            Vector3 forward = _context.LocalPlayer.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 position = _context.LocalPlayer.transform.position + forward * Range;

            // Target the cell x units away from us in the direction we are facing
            _targetCell = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        }

        private void TransformVisualTick()
        {
            Vector3 scale;
            Vector3 position = _context.Raft.CellToWorldPosition(_targetCell);

            if (_context.Raft.Tiles.TryGetValue(_targetCell, out RaftTile tile))
            {
                scale = StructureVisualScale;
                position.y = tile.GetSurfaceY() + scale.y * 0.5f;
            }
            else
            {
                scale = TileVisualScale;
                position.y = 0f;
            }

            _target.SetVisualScale(scale);
            _target.transform.position = position;
        }
    }
}