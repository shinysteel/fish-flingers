using UnityEngine;
using FishFlingers.Inventories;
using FishFlingers.Networking;

namespace FishFlingers.Items
{
    public class ItemInstance
    {
        public string InstanceId { get; private set; }
        public ItemData Data { get; private set; }
        public int Count { get; private set; }

        public ItemInstance(string instanceId, ItemData data, int count)
        {
            InstanceId = instanceId;
            Data = data;
            Count = Mathf.Clamp(count, 0, data.MaxStack);
        }
    }
}