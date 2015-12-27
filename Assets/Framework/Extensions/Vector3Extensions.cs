using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Framework.Extensions
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// http://stackoverflow.com/questions/14066933/direct-way-of-computing-clockwise-angle-between-2-vectors
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double PlanarAngle(this Vector3 from, Vector3 to, Vector3 normalVector3)
        {
            if (normalVector3 == null)
            {
                throw new ArgumentNullException("normalVector3");
            }

            var dot = Vector3.Dot(from, to);
            var det = from.Det(to, normalVector3.normalized);

            //lenSq1 = x1 * x1 + y1 * y1 + z1 * z1
            //lenSq2 = x2 * x2 + y2 * y2 + z2 * z2

            var degrees = ToDegree(Math.Atan2(det, dot));

            if (degrees < 0)
            {
                degrees += 360;
            }

            return degrees;
        }

        private static double ToDegree(double value)
        {
            return (180*value)/Math.PI;
        }

        public static double Det(this Vector3 first, Vector3 second, Vector3 third)
        {

            // det = x1 * y2 * zn + x2 * yn * z1 + xn * y1 * z2 - z1 * y2 * xn - z2 * yn * x1 - zn * y1 * x2

            return first.x*second.y*third.z +
                   first.y*second.z*third.x +
                   first.z*second.x*third.y -
                   first.z*second.y*third.x -
                   first.x*second.z*third.y -
                   first.y*second.x*third.z;

        }

        public static float Distance(this Vector3 first, Vector3 second)
        {
            return (second - first).magnitude;
        }

    }
}
