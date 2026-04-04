using UnityEngine;

namespace FishFlingers.Entities
{
    public class CharacterModel : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        public Animator Animator => _animator;
    }
}