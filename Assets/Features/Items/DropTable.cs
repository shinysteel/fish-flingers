using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.Items
{
    [CreateAssetMenu(fileName = "ItemDropTable", menuName = "Tables/ItemDropTable")]
    public class DropTable : ScriptableObject
    {
        [SerializeField] private WeightedEntry<ItemId>[] _entries;

        public WeightedEntry<ItemId>[] Entries => _entries;
    }
}