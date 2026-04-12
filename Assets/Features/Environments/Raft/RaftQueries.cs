using ShinyOwl.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FishFlingers.Entities;
using FishFlingers.Networking;
using System.Collections;
using UnityEngine.Pool;
using System.Buffers;

namespace FishFlingers.Environments
{
    public enum ERaftAxis
    {
        Horizontal,
        Vertical
    }

    public class RaftAxis
    {
        private Raft _raft;
        private ERaftAxis _type;
        private Dictionary<int, RaftLine> _lines = new();

        private int _minAnchor;
        private int _maxAnchor;

        public int MinAnchor => _minAnchor;
        public int MaxAnchor => _maxAnchor;

        public IReadOnlyDictionary<int, RaftLine> Lines => _lines;

        public RaftAxis(Raft raft, ERaftAxis type)
        {
            _raft = raft;
            _type = type;

            _raft.OnTileChanged += HandleTileChanged;
        }

        ~RaftAxis()
        {
            if (_raft != null)
            {
                _raft.OnTileChanged -= HandleTileChanged;
            }
        }

        private void HandleTileChanged(Vector2Int cell, Tile tile)
        {
            UpdateLines(cell, tile);
            UpdateBounds(cell, tile);
        }

        // Maintains positional maps when SetTile is called
        private void UpdateLines(Vector2Int cell, Tile tile)
        {
            int anchor = _type == ERaftAxis.Horizontal ? cell.y : cell.x;
            int index = _type == ERaftAxis.Horizontal ? cell.x : cell.y;

            if (tile != null)
            {
                if (!_lines.ContainsKey(anchor))
                {
                    _lines.Add(anchor, new RaftLine(_raft, _type, anchor));
                }

                _lines[anchor].AddTile(tile);
            }
            else
            {
                _lines[anchor].RemoveTile(tile);
            }
        }

        // Recalculates boundaries when SetTile is called
        private void UpdateBounds(Vector2Int cell, Tile tile)
        {
            int anchor = _type == ERaftAxis.Horizontal ? cell.y : cell.x;

            if (tile != null)
            {
                _minAnchor = Mathf.Min(_minAnchor, anchor);
                _maxAnchor = Mathf.Max(_maxAnchor, anchor);
            }
            else
            {
                if (_minAnchor == anchor && !_lines.ContainsKey(anchor))
                {
                    _minAnchor = _lines.Keys.Min();
                }

                if (_maxAnchor == anchor && !_lines.ContainsKey(anchor))
                {
                    _maxAnchor = _lines.Keys.Max();
                }
            }
        }
    }

    // A line represents a span of cells along a point and axis
    public class RaftLine
    {
        private Raft _raft;
        private ERaftAxis _axis;
        private int _anchor;

        private SortedSet<Tile> _tiles;
        public IReadOnlyCollection<Tile> Tiles => _tiles;

        private RaftEdge _minEdge;
        private RaftEdge _maxEdge;

        public RaftEdge MinEdge => _minEdge;
        public RaftEdge MaxEdge => _maxEdge;

        public RaftLine(Raft raft, ERaftAxis axis, int anchor)
        {
            _raft = raft;
            _axis = axis;
            _anchor = anchor;

            _tiles = new SortedSet<Tile>(Comparer<Tile>.Create((a, b) =>
            {
                if (_axis == ERaftAxis.Horizontal)
                {
                    return b.Cell.x.CompareTo(a.Cell.x);
                }
                else
                {
                    return b.Cell.y.CompareTo(a.Cell.y);
                }
            }));
        }

        public void AddTile(Tile tile)
        {
            _tiles.Add(tile);

            RefreshEdges();
        }

        public void RemoveTile(Tile tile)
        {
            _tiles.Remove(tile);

            RefreshEdges();
        }

        private void RefreshEdges()
        {
            if (_tiles.Count == 0)
            {
                _minEdge = null;
                _maxEdge = null;
                return;
            }

            if (_minEdge == null || !_tiles.Contains(_minEdge.Tile))
            {
                _minEdge = new RaftEdge(_tiles.First(), _axis == ERaftAxis.Horizontal ? Vector2Int.left : Vector2Int.down);
            }

            if (_maxEdge == null || !_tiles.Contains(_maxEdge.Tile))
            {
                _maxEdge = new RaftEdge(_tiles.Last(), _axis == ERaftAxis.Horizontal ? Vector2Int.right : Vector2Int.up);
            }
        }
    }

    public class RaftEdge
    {
        private Tile _tile;
        private Vector2Int _cellDirection;
        private Vector3 _worldDirection;

        public Tile Tile => _tile;
        public Vector2Int CellDirection => _cellDirection;
        public Vector3 WorldDirection => _worldDirection;

        public RaftEdge(Tile tile, Vector2Int cellDirection)
        {
            _tile = tile;
            _cellDirection = cellDirection;

            // Convert 2d to 3d
            if (_cellDirection == Vector2Int.up)
            {
                _worldDirection = Vector3.forward;
            }
            else if (_cellDirection == Vector2Int.right)
            {
                _worldDirection = Vector3.right;
            }
            else if (_cellDirection == Vector2Int.down)
            {
                _worldDirection = Vector3.back;
            }
            else if (_cellDirection == Vector2Int.left)
            {
                _worldDirection = Vector3.left;
            }
            else
            {
                _worldDirection = Vector3.zero;
            }
        }
    }

    public class RaftQueries
    {
        private Raft _raft;

        private Dictionary<ERaftAxis, RaftAxis> _axes = new();

        public IReadOnlyDictionary<ERaftAxis, RaftAxis> Axes => _axes;

        public RaftQueries(Raft raft)
        {
            _raft = raft;

            _axes.Add(ERaftAxis.Horizontal, new RaftAxis(_raft, ERaftAxis.Horizontal));
            _axes.Add(ERaftAxis.Vertical, new RaftAxis(_raft, ERaftAxis.Vertical));
        }

        // Uses Vector2 to allow for floating-point cells
        public Vector3 CellToWorldPosition(Vector2 cell)
        {
            return new Vector3(cell.x, 0f, cell.y);
        }

        public Vector2Int WorldPositionToCell(Vector3 position)
        {
            return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        }

        /// <summary>
        /// Retrieves a random tile
        /// </summary>
        public bool TryGetRandomTile(out Tile tile)
        {
            tile = null;

            if (_raft.Tiles.Count == 0)
            {
                return false;
            }

            tile = _raft.Tiles.Values.ElementAt(Random.Range(0, _raft.Tiles.Count));

            return true;
        }

        public bool TryGetRandomLine(out RaftLine line)
        {
            line = null;

            // Horizontal is always equivalent to vertical
            if (_axes[ERaftAxis.Horizontal].Lines.Count == 0)
            {
                return false;
            }

            RaftAxis axis = _axes.ElementAt(Random.Range(0, _axes.Count)).Value;
            line = axis.Lines.ElementAt(Random.Range(0, axis.Lines.Count)).Value;

            return true;
        }

        /// <summary>
        /// Finds the closest edge to a cell. Ties are resolved randomly
        /// </summary>
        public bool TryGetClosestEdge(Vector2Int cell, out RaftEdge closestEdge)
        {
            closestEdge = null;

            if (!_axes[ERaftAxis.Horizontal].Lines.TryGetValue(cell.y, out RaftLine horizontalLine) 
                || !_axes[ERaftAxis.Vertical].Lines.TryGetValue(cell.x, out RaftLine verticalLine))
            {
                return false;
            }

            if (horizontalLine.Tiles.Count == 0)
            {
                return false;
            }

            RaftEdge[] edges = ArrayPool<RaftEdge>.Shared.Rent(4);

            edges[0] = horizontalLine.MinEdge;
            edges[1] = horizontalLine.MaxEdge;
            edges[2] = verticalLine.MinEdge;
            edges[3] = verticalLine.MaxEdge;

            int minDistance = edges.Min(edge => (cell - edge.Tile.Cell).sqrMagnitude);

            closestEdge = edges
                .Where(edge => (cell - edge.Tile.Cell).sqrMagnitude == minDistance)
                .OrderBy(_ => Random.value)
                .First();

            ArrayPool<RaftEdge>.Shared.Return(edges);

            return true;
        }
    }
}