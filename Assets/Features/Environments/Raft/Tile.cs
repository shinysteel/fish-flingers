using FishFlingers.Pools;
using UnityEngine;

namespace FishFlingers.Environments
{
    public class Tile : MonoBehaviour, IPoolable
    {
        private Vector2Int _cell;
        private int _health;

        public const int DefaultHealth = 3;

        public void Set(Vector2Int cell, int health)
        {
            _cell = cell;
            _health = health;

            // For now, let's just update the position here
            transform.localPosition = new Vector3(cell.x, 0f, cell.y);
        }

        public void OnReturnedToPool()
        { }

        public void OnTakenFromPool()
        { }
    }
}