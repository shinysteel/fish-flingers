using UnityEngine;

namespace FishFlingers.Entities
{
    public class Drowning : Character<DrowningDefinitionData>
    {
        private RaftPlayer _target;

        public void SetTarget(RaftPlayer target)
        {
            _target = target;

            // get the direction to the center
            // flip it
            // randomise by ~25 degrees
            // start there at a distance

            Vector3 direction = target.transform.position;
            direction.y = 0f;
            direction.Normalize();

            direction = Quaternion.AngleAxis(Random.Range(-25f, 25f), Vector3.up) * direction;

            Vector3 position = _target.transform.position;
            position.y = 0f;
            position += direction * 5f;
            
            transform.position = position;
        }

        protected override void Update()
        {
            base.Update();

            if (!isServer)
            {
                return;
            }

            Vector3 direction = (_target.transform.position - transform.position);
            direction.y = 0f;
            direction.Normalize();

            float speed = 0.5f + _lifecycleModule.TimeAlive * 0.5f;

            transform.position += direction * speed * Time.deltaTime;
        }
    }
}