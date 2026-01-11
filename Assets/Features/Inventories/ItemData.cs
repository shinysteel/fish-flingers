using ShinyOwl.Common.Structures;
using UnityEngine;
using FishFlingers.Items;

namespace FishFlingers.Inventories
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Data/ItemData")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private ItemId _itemId;
        [SerializeField] private int _maxStack;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private BoolGrid _shape;

        // To differentiate from InstanceId, we use ItemId
        public ItemId ItemId => _itemId;

        public int MaxStack => _maxStack;
        public Sprite Sprite => _sprite;
        public BoolGrid Shape => _shape;
    }
}