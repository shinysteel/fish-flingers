using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.Entities
{
    public class CharacterPhysicsLogic
    {
        private Character _character;

        private CharacterPhysicsSettings _settings;

        protected bool _isGrounded;
        public bool IsGrounded => _isGrounded;

        private bool _isFloating;
        public bool IsFloating => _isFloating;

        private RaycastHit[] _hitsNonAlloc = new RaycastHit[2];

        public CharacterPhysicsLogic(Character character)
        {
            _character = character;

            _settings = _character.CharacterData.CharacterPhysicsSettings;
        }

        public virtual void Tick()
        { }

        public virtual void FixedTick()
        {
            _isGrounded = DetectContact(_settings.ContactDetection.GroundedMask);
            _isFloating = DetectContact(_settings.ContactDetection.FloatingMask);
        }

        private bool DetectContact(LayerMask mask)
        {
            Vector3 origin = _character.CharacterCollider.bounds.center;
            origin.y = _character.CharacterCollider.bounds.min.y;
            origin += Vector3.up * _settings.ContactDetection.CastRadius;

            int hits = Physics.SphereCastNonAlloc(origin, _settings.ContactDetection.CastRadius, Vector3.down, _hitsNonAlloc, _settings.ContactDetection.CastDistance, mask);

            bool success = false;

            for (int i = 0; i < hits; i++)
            {
                // Since we include the player layer to jump on other player's heads, we need to ignore our own collider here
                if (_hitsNonAlloc[i].collider.gameObject != _character.gameObject)
                {
                    success = true;
                    break;
                }
            }

            return success;
        }
    }
}