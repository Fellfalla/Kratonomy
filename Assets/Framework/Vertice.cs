using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using Framework.Extensions;
using UnityEngine;

namespace Assets.Framework
{
    public class Vertice
    {
        public Vertice(Mesh mesh, int verticeIndex)
        {
            Index = verticeIndex;
            Vector = mesh.vertices[verticeIndex];
            Neighbours = new List<int>();
            Triangles = new List<int[]>();
            Mesh = mesh;

            RefreshVerticeInformation();
        }

        /// <summary>
        /// Refreshes the triangles in which the vertice is used and the <see cref="Neighbours"/>
        /// </summary>
        private void RefreshVerticeInformation()
        {
            Neighbours = new List<int>();
            Triangles = new List<int[]>();

            // Analyse Triangles and Neighbours
            for (int triangle = 0; triangle + 2 < Mesh.triangles.Length; triangle += 3)
            {
                // Get all using triangles
                var currentTriangle = Mesh.triangles.SubArray(triangle, 3);
                if (currentTriangle.Contains(Index))
                {
                    Triangles.Add(currentTriangle);

                    // Get all existing neighbours
                    foreach (var index in currentTriangle.Where(index => index != Index))
                    {
                        if (!Neighbours.Contains(index))
                        {
                            Neighbours.Add(index);
                        }
                    }
                }
            }

            //FreeSpace = FreeSpace2D.GetFreeSpace(this);
        }

        public Vector3 Vector { get; set; }

        public int Index { get; set; }

        public Mesh Mesh { get; private set; }

        public List<int> Neighbours { get; set; }

        public List<int[]> Triangles { get; set; } 

        //public FreeSpace2D FreeSpace { get; set; }

        public override string ToString()
        {
            return Index.ToString();
        }
    }
}
