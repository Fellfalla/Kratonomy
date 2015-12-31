using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.Framework;
using Assets.Framework.Extensions;
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

        private static Vector3 normalVector = Vector3.forward;

        public FreeSpace2D()
        {
            var initialRegion = new RadialSpace(0f, 360f);
            initialRegion.NextRegion = initialRegion;
            initialRegion.PreviousRegion = initialRegion;
            AddRegion(initialRegion);
        }

        /// <summary>
        /// Gibt Tuples mit nachbarn zurück und Informationen darüber, ob diese Nachbarn besetzt sind.
        /// </summary>
        public static FreeSpace2D GetFreeSpace(Vertice vertice)
        {
            var result = new FreeSpace2D();
            result.CenterVertice = vertice;

            // Remove free spaces 
            foreach (var triangle in vertice.Triangles)
            {
                var neighbourIndices = triangle.Where(index => index != vertice.Index).ToArray();

                // Get first direction
                var direction1 = vertice.Mesh.vertices[neighbourIndices[0]] - vertice.Mesh.vertices[vertice.Index];

                // Get second direction
                var direction2 = vertice.Mesh.vertices[neighbourIndices[1]] - vertice.Mesh.vertices[vertice.Index];

                // Get angle between triangle points
                var angle = direction1.PlanarAngle(direction2, normalVector);
                float beginAngle;
                Vertice openingVertice;
                Vertice closingVertice;
                if (angle < 180)
                {
                    beginAngle = Convert.ToSingle(Vector3.up.PlanarAngle(direction1, normalVector));
                    openingVertice = new Vertice(vertice.Mesh, neighbourIndices[0]);
                    closingVertice = new Vertice(vertice.Mesh, neighbourIndices[1]);
                }
                else // The triangle mesh is closed in other direction
                {

                    angle = direction2.PlanarAngle(direction1, normalVector);
                    beginAngle = Convert.ToSingle(Vector3.up.PlanarAngle(direction2, normalVector));
                    openingVertice = new Vertice(vertice.Mesh, neighbourIndices[1]);
                    closingVertice = new Vertice(vertice.Mesh, neighbourIndices[0]);
                    //
                }
                var endAngle = beginAngle + Convert.ToSingle(angle);

                var newRegion = new RadialSpace(beginAngle, endAngle);
                newRegion.CenterVertice = vertice;
                newRegion.ClosingVertice = closingVertice;
                newRegion.OpeningVertice = openingVertice;

                // Remove Space
                result.AddRegion(newRegion);
            }

            return result;
        }

        /// <summary>
        /// Each free space goes from the first vector in direction of the second vector
        /// </summary>
        public readonly List<RadialSpace> Regions = new List<RadialSpace>();        

        public Vertice CenterVertice { get; set; }

        public MeshFilter MeshFilter { get; set; }

        public IEnumerable<RadialSpace> GetRegionsOfFreeSpace()
        {
            var result = Regions.Where(region => !region.IsBlocked);
            return result;
        }

        public IEnumerable<RadialSpace> GetRegionsOfBlockedSpace()
        {
            return Regions.Where(region => !region.IsBlocked);
        }

        public void AddRegion(RadialSpace newRegion)
        {

            // Search affected Regions
            foreach (var region in Regions.ToArray())
            {
                if (!newRegion.IntersectsWith(region)) { continue;}

                if (newRegion.Equals(region)) // Anfang und ende stimmen
                {
                    RemoveRegion(region);
                }

                else if (region.BeginsWith(newRegion)) // Startpunkte stimmen überein
                {
                    // Verkleinern falls das ende innerhalb der region liegt
                    if (region.ContainsDirection(newRegion.End))
                    {
                        region.PreviousRegion = newRegion;

                    }
                    else // komplett löschen, falls das ende ausserhalb der region oder auf dem Rand liegt
                    {
                        RemoveRegion(region); // todo: vertice informationen anpassen
                        continue;
                    }
                }

                else if (region.EndsWith(newRegion)) // Endpunkte stimmen überein
                {
                    // Verkleinern falls der anfang innerhalb der region liegt
                    if (region.ContainsDirection(newRegion.Begin))
                    {
                        region.NextRegion = newRegion;
                    }
                    else // komplett löschen, falls der Anfang ausserhalb der region oder auf dem Rand liegt
                    {
                        RemoveRegion(region);
                    }
                }

                else if (newRegion.IsCompleteWithin(region)) // Startpunkt und Endpunkt liegen innerhalb der region
                {
                    // Erzeuge 2 neue kleinere regionen
                    // Die neue Region ist die Region hinter newRegion
                    var additionalRegion = new RadialSpace(newRegion, region.NextRegion);

                    region.NextRegion = newRegion;

                    Regions.Add(additionalRegion);
                }

                else if (region.ContainsDirection(newRegion.Begin)) // Startpunkt liegt innerhalb der region und ende liegt ausserhalb
                {
                    region.NextRegion = newRegion;
                }

                else if (region.ContainsDirection(newRegion.End)) // Endpunkt liegt innerhalb der region und anfang liegt ausserhalb
                {

                    region.PreviousRegion = newRegion;
                }

            }

            Regions.Add(newRegion);

            CleanRegions();
        }

        private void RemoveRegion(RadialSpace region)
        {
            region.PreviousRegion.NextRegion = region.NextRegion;

            region.NextRegion.PreviousRegion = region.PreviousRegion;

            Regions.Remove(region);
        }

        private void CleanRegions()
        {
            // Bereinigt redundante regionen

            foreach (var region in Regions.ToArray())
            {
                if (region != region.NextRegion && !region.IsBlocked && !region.NextRegion.IsBlocked)
                {
                    // Vereinige 2 Regionen, falls beide nicht blockiert sind
                    var mergedRegion = RadialSpace.MergeRadialSpaces(region, region.NextRegion);
                    RemoveRegion(region);
                    RemoveRegion(region.NextRegion);
                    AddRegion(mergedRegion);
                }
            }

            //throw new NotImplementedException();
        }
    }
}
