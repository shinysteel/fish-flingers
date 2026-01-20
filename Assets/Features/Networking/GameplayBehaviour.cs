using FishFlingers.States;
using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.Networking
{
    public abstract class GameplayBehaviour : NetBehaviour
    {
        protected GameplayContext _context;

        public virtual void Initialise(GameplayContext context)
        {
            _context = context;
        }
    }
}