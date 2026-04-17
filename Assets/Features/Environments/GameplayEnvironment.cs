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

namespace FishFlingers.Environments
{
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