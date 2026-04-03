using FishFlingers.Entities;
using FishFlingers.Instantiating;
using FishFlingers.Items;
using FishFlingers.Saving;
using FishFlingers.States;
using Newtonsoft.Json;
using ShinyOwl.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EntityId = FishFlingers.Entities.EntityId;

namespace FishFlingers.Environments
{
    public class GameplayEnvironmentSave
    {
        [JsonProperty] public RaftSave Raft { get; private set; } = new();
        [JsonProperty] public List<DroppedItemSave> DroppedItems { get; private set; } = new();

        public void LoadTo(GameplayEnvironment environment)
        {
            EntityManager entityManager = GameManager.Instance.Get<EntityManager>();

            Raft.LoadTo(environment.Context.Raft);

            foreach (DroppedItemSave save in DroppedItems)
            {
                DroppedItem droppedItem = (DroppedItem)entityManager.Spawn(EntityId.DroppedItem, new SpawnParams() { Position = save.Position });
                droppedItem.Set(NetItemInstance.Create(save), save.Type);
            }
        }

        public void SaveFrom(GameplayEnvironment environment)
        {
            EntityManager entityManager = GameManager.Instance.Get<EntityManager>();

            Raft.SaveFrom(environment.Context.Raft);

            DroppedItems.Clear();

            foreach (IEntity entity in entityManager.Entities)
            {
                if (entity is not DroppedItem droppedItem)
                {
                    continue;
                }

                DroppedItems.Add(new DroppedItemSave(droppedItem));
            }
        }

        public void ApplyDefaults()
        {
            Raft.ApplyDefaults();
        }
    }
    
    public class GameplayEnvironment : MonoBehaviour, ISaveable
    {
        private InstantiateManager _instantiateManager;
        private SaveManager _saveManager;

        private GameplayContext _context;
        public GameplayContext Context => _context;

        private void Awake()
        {
            _instantiateManager = GameManager.Instance.Get<InstantiateManager>();
            _saveManager = GameManager.Instance.Get<SaveManager>();
        }

        public void Initialise(GameplayContext context)
        {
            _context = context;

            _instantiateManager.RaiseComponentInstantiated(this);
        }

        private void OnDestroy()
        {
            _instantiateManager?.RaiseComponentDestroyed(this);
        }

        async Task ISaveable.LoadAsync()
        {
            _saveManager.GameSave.Environment.LoadTo(this);
        }

        void ISaveable.Save()
        {
            _saveManager.GameSave.Environment.SaveFrom(this); 
        }
    }
}