using FishFlingers.Cameras;
using FishFlingers.Entities;
using FishFlingers.Items;
using UnityEngine;

namespace FishFlingers.Entities
{ 
    public class DropItemLogic
    {
        private EntityManager _entityManager;
        private CameraManager _cameraManager;

        private RaftPlayer _player;

        private const float Pitch = -45f;
        private const float Strength = 3f;

        public DropItemLogic(RaftPlayer player)
        {
            _entityManager = GameManager.Instance.Get<EntityManager>();
            _cameraManager = GameManager.Instance.Get<CameraManager>();

            _player = player;
        }

        public void Drop(ItemInstance instance, bool towardsMouse)
        {
            DroppedItem item = (DroppedItem)_entityManager.Spawn(EEntity.DroppedItem, new SpawnParams() { Position = _player.transform.position });
            item.SetItem(instance.InstanceId, instance.Data.ItemId, instance.Count);

            Vector3 direction = _player.transform.forward;
            direction.y = 0f;
            direction.Normalize();

            if (towardsMouse)
            {
                Ray ray = _cameraManager.MainCamera.ScreenPointToRay(_player.InputLogic.Mouse);
                Plane plane = new Plane(Vector3.up, _player.transform.position);

                if (plane.Raycast(ray, out float distance))
                {
                    direction = (ray.GetPoint(distance) - _player.transform.position).normalized;
                }
            }

            // Launch the item in the direction
            direction = Quaternion.AngleAxis(Pitch, Vector3.Cross(Vector3.up, direction)) * direction;
            item.Rigidbody.AddForce(direction * Strength, ForceMode.Impulse);
        }
    }
}