using FishFlingers.Entities;
using FishFlingers.Inventories;
using UnityEngine;

namespace FishFlingers.Entities
{
    public class ClamChest : Structure<ClamChestData>, IInteractable
    {
        private Inventory _inventory;

        public Vector3 Position => transform.position;
        
        public void Interact()
        {
            
        }
    }
}