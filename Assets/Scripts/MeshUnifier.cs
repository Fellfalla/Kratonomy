using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Framework.Extensions;

[RequireComponent(typeof(MeshFilter))]
public class MeshUnifier : MonoBehaviour
{

    public float ScaleOnCollision = 1.1f;

    private Mesh mesh;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

    }


    // Use this for initialization
    void OnCollisionEnter(Collision col)
    {
        var otherMesh = col.collider.GetComponent<MeshFilter>().mesh;
        var point = col.contacts.First().point;

        
        var ownVertices = GetClosestVertices(point, mesh, 3);
        var otherVertices = GetClosestVertices(point, otherMesh, 3);



        //col.gameObject.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        //var mesh = col.gameObject.GetComponent<MeshFilter>().mesh;
        //var vertices = mesh.vertices;
        //mesh.MarkDynamic();
        //mesh.SetVertices(mesh.vertices.Select<Vector3,Vector3>((vertice) => {
        //    return Vector3.Scale(vertice, new Vector3(ScaleOnCollision, ScaleOnCollision, ScaleOnCollision));
        //}).ToList());
        //for (int i = 0; i< vertices.Length; i++)
        //{
        //    //vertices[i] = Vector3.Scale(vertices[i],new Vector3(1.5f,1.5f,1.5f));
        //}
    }
    public static IEnumerable<int> GetClosestVertices(Vector3 point, Mesh mesh, int count)
    {
        var distances = new Dictionary<int, float>();
        float minDistance = float.MaxValue;

        for (int vertex = 0; vertex < mesh.vertexCount; vertex++)
        {
            minDistance = point.Distance(mesh.vertices[vertex]);
            distances.Add(vertex, minDistance);
        }

        return distances.OrderBy(pair => pair.Key).Select(pair => pair.Key);

    }

}
