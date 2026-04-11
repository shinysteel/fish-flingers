using System;
using UnityEngine;

namespace FishFlingers.Entities
{
    public class CharacterModel : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _itemLocator;

        public Animator Animator => _animator;
        public Transform ItemLocator => _itemLocator;
    }
}