using Newtonsoft.Json;
using UnityEngine;

namespace ShinyOwl.Common.Utils
{
    public static partial class Utils
    {
        public static class Serialisation
        { }
    }

    public class SerialisableVector2Int
    {
        [JsonProperty] public int X { get; private set; }
        [JsonProperty] public int Y { get; private set; }

        public SerialisableVector2Int() : this(Vector2Int.zero)
        { }

        public SerialisableVector2Int(Vector2Int vector2Int)
        {
            X = vector2Int.x;
            Y = vector2Int.y;
        }

        public Vector2Int ToVector2Int()
        {
            return new Vector2Int(X, Y);
        }
    }

    public class SerialisableVector3
    {
        [JsonProperty] public float X { get; private set; }
        [JsonProperty] public float Y { get; private set; }
        [JsonProperty] public float Z { get; private set; }

        public SerialisableVector3() : this(Vector3.zero)
        { }

        public SerialisableVector3(Vector3 vector3)
        {
            X = vector3.x;
            Y = vector3.y;
            Z = vector3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }

    public class SerialisableQuaternion
    {
        [JsonProperty] public float X { get; private set; }
        [JsonProperty] public float Y { get; private set; }
        [JsonProperty] public float Z { get; private set; }
        [JsonProperty] public float W { get; private set; }

        public SerialisableQuaternion() : this(Quaternion.identity)
        { }

        public SerialisableQuaternion(Quaternion quaternion)
        {
            X = quaternion.x;
            Y = quaternion.y;
            Z = quaternion.z;
            W = quaternion.w;
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(X, Y, Z, W);
        }
    }
}