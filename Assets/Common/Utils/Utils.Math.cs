using System;
using UnityEngine;

namespace ShinyOwl.Common.Utils
{
    public static partial class Utils
    {
        public static class Math
        {
            public static int EuclideanModulo(int dividend, int modulus)
            {
                int remainder = dividend % modulus;

                if (remainder < 0)
                {
                    remainder += modulus;
                }

                return remainder;
            }

            public static Vector2 RotateCell(Vector2 cell, int rotations, bool clockwise)
            {
                rotations = EuclideanModulo(clockwise ? rotations : -rotations, 4);

                return (rotations % 4) switch
                {
                    0 => cell,
                    1 => new Vector2(cell.y, -cell.x),
                    2 => new Vector2(-cell.x, -cell.y),
                    3 => new Vector2(-cell.y, cell.x),

                    _ => cell
                };
            }

            public static Vector2Int RotateCell(Vector2Int cell, int rotations, bool clockwise)
            {
                return Vector2Int.RoundToInt(RotateCell((Vector2)cell, rotations, clockwise));
            }

            public static Vector3 RoundVector3(Vector3 vector3, int precision)
            {
                return new Vector3(
                    (float)System.Math.Round(vector3.x, precision), 
                    (float)System.Math.Round(vector3.y, precision), 
                    (float)System.Math.Round(vector3.z, precision));
            }

            public static Quaternion RoundQuaternion(Quaternion quaternion, int precision)
            {
                return new Quaternion(
                    (float)System.Math.Round(quaternion.x, precision),
                    (float)System.Math.Round(quaternion.y, precision),
                    (float)System.Math.Round(quaternion.z, precision),
                    (float)System.Math.Round(quaternion.w, precision));
            }

            public static int HashLongToInt(long value)
            {
                return (int)(value ^ (value >> 32));
            }
        }
    }
}