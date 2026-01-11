
using FishFlingers.Inventories;
using FishFlingers.Networking;
using ShinyOwl.Common;
using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.Items
{
    public interface IItemManagerListener
    { }

    public class ItemManager : GameSystem<IItemManagerListener>
    {
        private NetworkManager _networkManager;

        private ItemManagerConfig _config;

        private Dictionary<ItemId, ItemData> _idDataMap = new();

        private int _itemInstanceIdCounter;

        public override void Initialise(GameManagerConfig config)
        {
            _networkManager = GameManager.Instance.Get<NetworkManager>();

            _config = config.ItemManagerConfig;

            // Eventually, we'd want to have something do a project scan in editor
            // to maintain a collection of all ItemData
            _idDataMap.Add(ItemId.Driftwood, _config.DriftwoodData);
            _idDataMap.Add(ItemId.Paddle, _config.PaddleData);

            base.Initialise(config);
        }

        public ItemData GetItemData(ItemId id)
        {
            return _idDataMap[id];
        }

        // This solution is alright for making sure all items are unique across the session, but it will
        // break if a client stops their application and than rejoins, having their id counter reset to 0
        public string GetNextItemInstanceId()
        {
            return $"{_networkManager.LocalPlayerId}_{_itemInstanceIdCounter++}";
        }
    }
}