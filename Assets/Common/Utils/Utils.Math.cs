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
        }
    }
}