using FishFlingers.Pools;
using UnityEngine;

namespace FishFlingers.Environments
{
    public class Prop : MonoBehaviour, IPoolable
    {
        [SerializeField] private PropId _id;
        public PropId Id => _id;

        public void OnReturnedToPool()
        { }

        public void OnTakenFromPool()
        { }
    }
}