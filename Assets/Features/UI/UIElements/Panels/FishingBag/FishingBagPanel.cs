using FishFlingers.Items;
using FishFlingers.Networking;
using System;
using UnityEngine;

namespace FishFlingers.UI
{
    public class FishingBagPanel : Panel
    {
        [SerializeField] private InventoryWidget _inventoryWidget;

        private NetworkManager _networkManager;

        public override void Load()
        {
            base.Load();
            
            // _inventoryWidget.Setup(null);
        }
    }
}