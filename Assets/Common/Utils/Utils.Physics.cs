using UnityEngine;

namespace ShinyOwl.Common.Utils
{
    public static partial class Utils
    {
        public static class Physics
        {
            public static Vector3 GetProjectilePosition(Vector3 startPosition, Vector3 endPosition, float gravity, float launchAngle, float normalisedTime)
            {
                Vector3 direction = endPosition - startPosition;
                Vector3 directionXZ = new Vector3(direction.x, 0f, direction.z);
                float distance = directionXZ.magnitude;
                float radians = launchAngle * Mathf.Deg2Rad;
                float cos = Mathf.Cos(radians);
                float sin = Mathf.Sin(radians);
                float height = direction.y;
                float speed = (gravity * distance * distance) / (2f * cos * cos * (distance * Mathf.Tan(radians) - height));
                speed = Mathf.Sqrt(speed);
                Vector3 velocity = directionXZ.normalized * speed * cos + Vector3.up * speed * sin;
                float time = distance / (speed * cos);
                float t = time * normalisedTime;
                return startPosition + velocity * t + Vector3.down * (0.5f * gravity * t * t);
            }
        }
    }
}