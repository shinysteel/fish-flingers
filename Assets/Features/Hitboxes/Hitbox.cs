using FishFlingers.Entities;
using FishFlingers.Pools;
using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.Hitboxes
{
    public class HitboxParams
    {
        private Vector3 _position;
        private float _radius;
        private int _damage;
        private float _knockbackStrength;
        private float _duration;
        private EntityAlliance _alliance;

        public Vector3 Position => _position;
        public float Radius => _radius;
        public int Damage => _damage;
        public float KnockbackStrength => _knockbackStrength;
        public float Duration => _duration;
        public EntityAlliance Alliance => _alliance;

        public HitboxParams(Vector3 position, float radius, int damage, float knockbackStrength, float duration, EntityAlliance alliance)
        {
            _position = position;
            _radius = radius;
            _damage = damage;
            _knockbackStrength = knockbackStrength;
            _duration = duration;
            _alliance = alliance;
        }
    }

    public class Hitbox : MonoBehaviour, IPoolable
    {
        [SerializeField] private SphereCollider _sphereCollider;

        private PoolManager _poolManager;

        private HitboxParams _parameters;

        private float _timer;

        private void Awake()
        {
            _poolManager = GameManager.Instance.Get<PoolManager>();
        }

        public void Initialise(HitboxParams parameters)
        {
            _parameters = parameters;

            transform.position = parameters.Position;
            _sphereCollider.radius = parameters.Radius;
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer < _parameters.Duration)
            {
                return;
            }

            _poolManager.ReturnPoolable(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IEntity entity))
            {
                return;
            }

            if (_parameters.Alliance == entity.EntityData.Alliance && _parameters.Alliance != EntityAlliance.Neutral)
            {
                return;
            }

            entity.HealthModule.ChangeHealth(-_parameters.Damage);

            Vector3 knockbackDirection = (entity.Transform.position - transform.position).normalized;
            entity.Rigidbody.AddForce(knockbackDirection * _parameters.KnockbackStrength, ForceMode.Impulse);

            _poolManager.ReturnPoolable(this);
        }

        public void OnTakenFromPool()
        {
            _parameters = null;
            _timer = 0f;
        }

        public void OnReturnedToPool()
        { }
    }
}