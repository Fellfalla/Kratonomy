using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.TestStuff
{
    public class deformable : MonoBehaviour
    {

        public Mesh mesh;
        public Vector3[] verts;
        public Mesh oldMesh;

        void Start()
        {
            if (mesh == null)
            {
                oldMesh = mesh = this.GetComponent<MeshFilter>().mesh;
            }
            verts = mesh.vertices;
            this.GetComponent<MeshFilter>().mesh = mesh;
        }
        public void UpdateMesh()
        {
            mesh.vertices = verts;
            this.GetComponent<MeshFilter>().mesh.vertices = mesh.vertices;
        }

        public void UpdateMesh(Vector3[] points)
        {
            mesh.vertices = points;
            this.GetComponent<MeshFilter>().mesh.vertices = mesh.vertices;
            this.GetComponent<MeshCollider>().sharedMesh = null;
            this.GetComponent<MeshCollider>().sharedMesh = mesh;
        }
        void OnApplicationQuit()
        {
            this.GetComponent<MeshFilter>().mesh.vertices = oldMesh.vertices;
            this.GetComponent<MeshCollider>().sharedMesh = null;
            this.GetComponent<MeshCollider>().sharedMesh = oldMesh;
        }
    }
}
