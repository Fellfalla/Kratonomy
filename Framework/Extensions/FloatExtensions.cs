using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Extensions
{
    public static class FloatExtensions
    {
        /// <summary>
        /// The default tolerance
        /// </summary>
        public static float Tolerance = 0.01f;

        public static bool IsAlmostEqual(this float first, float second)
        {
            return first.IsAlmostEqual(second, Tolerance);
        }
        public static bool IsAlmostEqual(this float first, float second, float tolerance)
        {
            return Math.Abs(first - second) < tolerance;
        }
    }
}
