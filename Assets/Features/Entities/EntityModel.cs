using FishFlingers.States;
using PurrNet;
using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.Entities
{
    public class EntityModel : MonoBehaviour
    {
        [SerializeField] protected Animator _animator;
        [SerializeField] private NetworkAnimator _networkAnimator;

        public Animator Animator => _animator;

        public void SetTrigger(string name)
        {
            if (_networkAnimator == null)
            {
                _animator.SetTrigger(name);
            }
            else
            {
                _networkAnimator.SetTrigger(name);
            }
        }
    }
}