using System;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "EntityManagerConfig", menuName = "Configs/Managers/EntityManagerConfig")]
    public class EntityManagerConfig : ScriptableObject
    {
        [SerializeField] private IEntityScanner _iEntityScanner;

        public IEntityScanner IEntityScanner => _iEntityScanner;
    }
}