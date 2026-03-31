using FishFlingers.Entities;
using FishFlingers.Inventories;
using FishFlingers.UI;
using ShinyOwl.Common.Structures;
using UnityEngine;

namespace FishFlingers.Entities
{
    public class ClamChest : Structure<ClamChestData>, IInteractable
    {
        [SerializeField] private Inventory _inventory;
        [SerializeField] private BoolGrid _inventoryLayout;

        private PanelInstance<ClamChestPanel> _clamChestPanelInstance;

        public Vector3 Position => transform.position;
        
        private void Start()
        {
            _inventory.SetLayout(_inventoryLayout);

            _clamChestPanelInstance = new PanelInstance<ClamChestPanel>(_uiManager.Config.ClamChestPanel);
        }

        public void Interact()
        {
            _clamChestPanelInstance.Toggle((ClamChestPanel panel) => panel.Setup(_context, _inventory));
        }
    }
}