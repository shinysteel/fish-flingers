using PurrNet;
using System;
using UnityEngine;

namespace FishFlingers.Entities
{
    public class CharacterModel : MonoBehaviour
    {
        [SerializeField] private Transform _itemLocator;
        [SerializeField] private Animator _animator;
        [SerializeField] private NetworkAnimator _networkAnimator;

        public Transform ItemLocator => _itemLocator;
       
        public void SetBool(string name, bool value)
        {
            _animator.SetBool(name, value);
        }

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

        public AnimatorStateInfo GetCurrentAnimatorStateInfo(int index)
        {
            return _animator.GetCurrentAnimatorStateInfo(index);
        }

        public bool IsInTransition(int index)
        {
            return _animator.IsInTransition(index);
        }
    }
}