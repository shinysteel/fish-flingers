using FishFlingers.Entities;
using FishFlingers.Pools;
using ShinyOwl.Common;
using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace FishFlingers.Hitboxes
{
    [CreateAssetMenu(fileName = "HitboxData", menuName = "Data/Hitboxes/HitboxData")]
    public class HitboxData : ScriptableObject
    {
        [SerializeField] private int _damage = 1;
        [SerializeField] private float _knockbackStrength = 1f;
        [SerializeField] private EntityAlliance _alliance = EntityAlliance.Ally;
        [SerializeField] private HitboxStep[] _steps = new HitboxStep[0];

        public int Damage => _damage;
        public float KnockbackStrength => _knockbackStrength;
        public EntityAlliance Alliance => _alliance;
        public HitboxStep[] Steps => _steps;

        // We don't cache this since it's nice to have it update in realtime while editing
        public float Duration => _steps.Max(step => step.StartTime + step.Duration);
    }

    [Serializable]
    public class HitboxStep
    {
        [SerializeField] private HitboxShape _shape = HitboxShape.Box;
        [SerializeField] private Vector3 _offset = Vector3.zero;
        [SerializeField] private Vector3 _size = Vector3.one;
        [SerializeField] private float _radius = 1f;
        [SerializeField] private float _startTime = 0f;
        [SerializeField] private float _duration = 0.5f;

        public HitboxShape Shape => _shape;
        public Vector3 Offset => _offset;
        public Vector3 Size => _size;
        public float Radius => _radius;
        public float StartTime => _startTime;
        public float Duration => _duration;
        
        public bool InTimeWindow(float time)
        {
            return time >= _startTime && time < _startTime + _duration;
        }

        public Vector3 GetPosition(Transform hitboxTransform)
        {
            return hitboxTransform.transform.position + hitboxTransform.TransformDirection(_offset);
        }
    }

    public enum HitboxShape
    {
        Box,
        Sphere
    }

    public class Hitbox : MonoBehaviour, IPoolable
    {
        private PoolManager _poolManager;
        private HitboxManager _hitboxManager;

        private HitboxData _data;

        private float _timer;

        private Collider[] _collidersNonAlloc = new Collider[MaxOverlaps];
        private const int MaxOverlaps = 10;

        private List<IEntity> _hitEntities = new();

        private void Awake()
        {
            _poolManager = GameManager.Instance.Get<PoolManager>();
            _hitboxManager = GameManager.Instance.Get<HitboxManager>();
        }

        public void Initialise(Vector3 position, Quaternion rotation, HitboxData data)
        {
            transform.position = position;
            transform.rotation = rotation;

            _data = data;
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            StepUpdate();

            if (_timer >= _data.Duration)
            {
                _poolManager.ReturnPoolable(this);
            }
        }

        private void StepUpdate()
        {
            foreach (HitboxStep step in _data.Steps)
            {
                if (!step.InTimeWindow(_timer))
                {
                    continue;
                }

                int overlaps = step.Shape switch
                {
                    HitboxShape.Box => Physics.OverlapBoxNonAlloc(step.GetPosition(transform), step.Size * 0.5f, _collidersNonAlloc),
                    HitboxShape.Sphere => Physics.OverlapSphereNonAlloc(step.GetPosition(transform), step.Radius, _collidersNonAlloc),
                    _ => 0
                };

                for (int i = 0; i < overlaps; i++)
                {
                    if (!_collidersNonAlloc[i].TryGetComponent(out IEntity entity))
                    {
                        continue;
                    }

                    if (_hitEntities.Contains(entity))
                    {
                        continue;
                    }

                    if (_data.Alliance == entity.EntityData.Alliance && _data.Alliance != EntityAlliance.Neutral)
                    {
                        continue;
                    }

                    HitEntity(entity);
                }
            }
        }

        private void HitEntity(IEntity entity)
        {
            entity.HealthModule.ChangeHealth(-_data.Damage);

            Vector3 knockbackDirection = (entity.Transform.position - transform.position).normalized;
            entity.Rigidbody.AddForce(knockbackDirection * _data.KnockbackStrength, ForceMode.Impulse);

            _hitEntities.Add(entity);
        }

        public void OnReturnedToPool()
        {
            _data = null;

            _timer = 0f;

            _hitEntities.Clear();
        }

        public void OnTakenFromPool()
        { }

        private void OnDrawGizmos()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            
            if (!_hitboxManager.Config.DrawGizmos)
            {
                return;
            }

            Gizmos.color = _data.Alliance switch
            {
                EntityAlliance.Ally => Color.green,
                EntityAlliance.Enemy => Color.red,
                EntityAlliance.Neutral => Color.gray,
                _ => Color.gray
            };

            foreach (HitboxStep step in _data.Steps)
            {
                if (!step.InTimeWindow(_timer))
                {
                    continue;
                }
                
                if (step.Shape == HitboxShape.Box)
                {
                    Gizmos.DrawCube(step.GetPosition(transform), step.Size);
                }
                else if (step.Shape == HitboxShape.Sphere)
                {
                    Gizmos.DrawSphere(step.GetPosition(transform), step.Radius);
                }
            }
        }
    }
}