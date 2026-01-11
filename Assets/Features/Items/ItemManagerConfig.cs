using FishFlingers.Inventories;
using UnityEngine;

namespace FishFlingers.Items
{
    [CreateAssetMenu(fileName = "ItemManagerConfig", menuName = "Configs/Managers/ItemManagerConfig")]
    public class ItemManagerConfig : ScriptableObject
    {
        [SerializeField] private ItemData _driftwoodData;
        [SerializeField] private ItemData _paddleData;

        public ItemData DriftwoodData => _driftwoodData;
        public ItemData PaddleData => _paddleData;
    }
}