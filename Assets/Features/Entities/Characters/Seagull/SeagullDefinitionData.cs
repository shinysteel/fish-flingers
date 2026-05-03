using System;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "SeagullDefinitionData", menuName = "Data/Entities/Characters/SeagullDefinitionData")]
    public class SeagullDefinitionData : CharacterDefinitionData
    {
        [SerializeField] private SeagullFlySettings _flySettings;

        public SeagullFlySettings FlySettings => _flySettings;
    }


    [Serializable]
    public class SeagullFlySettings
    {
        [SerializeField] private float _speed = 2.5f;
        [SerializeField] private float _acceleration = 2.5f;

        public float Speed => _speed;
        public float Acceleration => _acceleration;
    }
}