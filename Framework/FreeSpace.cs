using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Framework.Extensions;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Diese Klasse gibt informationen darüber, welche bereiche um ein vertice nicht teil des meshes sind
    /// </summary>
    public class FreeSpace2D
    {
        public static float Tolerance = 0.01f;

        public FreeSpace2D()
        {
            _degreesOfFreeSpace = new List<Tuple<float, float>>()
            {
                Tuple.New(0f,360f),
            };

            _degreesOfNotFreeSpace = new List<Tuple<float, float>>();
        }

        /// <summary>
        /// Each free space goes from the first vector in direction of the second vector
        /// </summary>
        private List<Tuple<float, float>> _degreesOfFreeSpace;        
        
        /// <summary>
        /// Each free space goes from the first vector in direction of the second vector
        /// </summary>
        private List<Tuple<float, float>> _degreesOfNotFreeSpace;

        public void AddBlockedSpace(float begin, float end)
        {
            begin = FormatDegrees(begin);
            end = FormatDegrees(end);

            if(!AreParametersValid(begin, end))
            {
                return; // if parameters are no valid -> do nothing
            }

            AddRegion(begin, end, _degreesOfNotFreeSpace);
            RemoveRegion(begin, end, _degreesOfFreeSpace);

        }

        public void RemoveBlockedSpace(float begin, float end)
        {
            begin = FormatDegrees(begin);
            end = FormatDegrees(end);

            if (!AreParametersValid(begin, end))
            {
                return; // if parameters are no valid -> do nothing
            }

            AddRegion(begin, end, _degreesOfFreeSpace);
            RemoveRegion(begin, end, _degreesOfNotFreeSpace);
        }

        private static void AddRegion(float begin, float end, List<Tuple<float,float>> regionCollection )
        {
            regionCollection.Add(Tuple.New(begin, end));
        }

        private void RemoveRegion(float begin, float end, List<Tuple<float, float>> regionCollection)
        {


            // Search affected Regions
            foreach (var region in regionCollection)
            {
                if (begin.IsAlmostEqual(region.First, Tolerance) && end.IsAlmostEqual(region.Second, Tolerance)) // Anfang und ende stimmen
                {
                    regionCollection.Remove(region);
                }

                else if (begin.IsAlmostEqual(region.First, Tolerance)) // Startpunkte stimmen überein
                {
                    // Verkleinern falls das ende innerhalb der region liegt
                    if (IsWithinRegion(end, region))
                    {
                        region.First = end;
                    }
                    else // komplett löschen, falls das ende ausserhalb der region oder auf dem Rand liegt
                    {
                        regionCollection.Remove(region);
                    }
                }

                else if (end.IsAlmostEqual(region.Second, Tolerance)) // Endpunkte stimmen überein
                {
                    // Verkleinern falls der anfang innerhalb der region liegt
                    if (IsWithinRegion(begin, region))
                    {
                        region.Second = begin;
                    }
                    else // komplett löschen, falls der Anfang ausserhalb der region oder auf dem Rand liegt
                    {
                        regionCollection.Remove(region);
                    }
                }

                else if (IsWithinRegion(begin, region) && IsWithinRegion(end, region)) // Startpunkt und Endpunkt liegen innerhalb der region
                {
                    // Erzeuge 2 neue kleinere regionen
                    region.Second = begin;
                    var newRegion = Tuple.New(end, region.Second);
                    regionCollection.Add(newRegion);
                }
                else if (IsWithinRegion(begin, region)) // Startpunkt liegt innerhalb der region
                {
                    region.Second = begin;
                }
                else if (IsWithinRegion(end, region)) // Endpunkt liegt innerhalb der region
                {

                    region.Second = begin;
                }

            }

            regionCollection.Add(Tuple.New(begin, end));
        }


        private bool AreParametersValid(float begin, float end)
        {
            // Are valid if not almost equal
            return !begin.IsAlmostEqual(second: end, tolerance: Tolerance);
        }


        private bool IsWithinRegion(float value, Tuple<float, float> region)
        {
            bool valueIsOnEdge = (value.IsAlmostEqual(region.First, Tolerance) ||
                      value.IsAlmostEqual(region.Second, Tolerance));

            if (region.First > region.Second)
            {
                return !valueIsOnEdge && (value > region.First || value < region.Second);
            }
            else // Der zweite ist größer als der erste
            {
                return !valueIsOnEdge && (value > region.First && value < region.Second);
            }
        }



        private float FormatDegrees(float value)
        {
            if (value > 360)
            {
                value = value % 360;
            }

            else if (value < 0)
            {
                while (value < 0)
                {
                    // e.g. make -50 degree to 310 degree
                    value += 360;
                }
            }

            return value;
        }
    }
}
