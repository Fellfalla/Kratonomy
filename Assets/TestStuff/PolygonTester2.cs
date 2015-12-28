using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.TestStuff
{
    public class PolygonTester2 : MonoBehaviour
    {
        void Update()
        {
            MeshFilter meshfilter = GetComponent<MeshFilter>();

            // Use the triangulator to get indices for creating triangles
            Triangulator tr = new Triangulator(meshfilter.mesh.vertices);
            int[] indices = tr.Triangulate();

            // Create the Vector3 vertices
            Vector3[] vertices = new Vector3[meshfilter.mesh.vertexCount];

            // Create the mesh
            Mesh msh = new Mesh();
            msh.vertices = vertices;
            msh.triangles = indices;
            msh.RecalculateNormals();
            msh.RecalculateBounds();

            // Set up game object with mesh;
            meshfilter.mesh = msh;
        }
    }
}
