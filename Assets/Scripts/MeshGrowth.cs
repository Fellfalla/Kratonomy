using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Framework;
using Framework;
using Framework.Extensions;
using Random = UnityEngine.Random;

/// <summary>
/// Das Mesh muss aus Gleichseitigen Dreiecken bestehen
/// // todo: Es muss ein Radius um das aktuell wachsende Vertice gezogen werden, da es sich sonst mit Faces die nicht über Triangles ersichtlich sind überlagert
/// todo: Radius vom Zentrum des Objekts ausgehen, um einen Hotspot des wachstums fetzulegen
/// </summary>
public class MeshGrowth : MonoBehaviour
{
    public float Width = 1f;
    public float Height = 1f;

    /// <summary>
    /// Adds a new Face every * seconds
    /// </summary>
    public float GrowRate = 1f;

    private MeshFilter meshFilter;


    // Use this for initialization
    void Start()
    {
        
        meshFilter = gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.MarkDynamic();

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-Width, -Height, 0),
            new Vector3(-Width, Height, 0),
            new Vector3(Width, Height, 0),
            new Vector3(Width, -Height, 0),
        };

        int[] triangles = new int[6];

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 3;

        triangles[3] = 1;
        triangles[4] = 2;
        triangles[5] = 3;

        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.triangles = triangles;


        InvokeRepeating("Grow", GrowRate, GrowRate);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Lässt das Mesh wachsen
    /// </summary>
    private void Grow()
    {
        // Wähle ein freies Vertice aus.
        var choosenVertex = GetVerticeIndicesWithEmptyNeighbour().GetRandomElement();
        Vertice vertice = new Vertice(meshFilter.mesh, choosenVertex);

        // Choose any Free Space
        var space = FreeSpace2D.GetFreeSpace(vertice).GetRegionsOfFreeSpace().GetRandomElement();
        if (space == null)
        {
            Debug.LogWarning("Free space is null");
            return;
        }

        float maxAngle = 90;
        float minAngle = 45;
        if (space.GetSize() > maxAngle) // If space is big enough for more tha 1 additional vertice
        {
            // Add new vertice in free space
            Vector3 newVertex = Quaternion.Euler(0, 0, Random.Range(space.Begin + minAngle, space.Begin + maxAngle)) * Vector3.up + meshFilter.mesh.vertices[choosenVertex];
            var newVertexLength = 1;
            newVertex *= newVertexLength;
            meshFilter.mesh.vertices = meshFilter.mesh.vertices.Concat(new[] {newVertex}).ToArray();

            int thirdVertexIndex;

            if (space.OpeningVertice != null)
            {
                thirdVertexIndex = space.OpeningVertice.Index;
            }
            else if (space.ClosingVertice != null)
            {
                thirdVertexIndex = space.ClosingVertice.Index;
            }
            else
            {
                return;
            }
            meshFilter.mesh.triangles =
                meshFilter.mesh.triangles.Concat(new[]
                {vertice.Index, meshFilter.mesh.vertices.Length - 1, thirdVertexIndex}).ToArray();

        }
        else
        {
            if (space.OpeningVertice == null || space.ClosingVertice == null)
            {
                return;
            }

            meshFilter.mesh.triangles =
                meshFilter.mesh.triangles.Concat(new[]
                {vertice.Index, space.OpeningVertice.Index, space.ClosingVertice.Index}).ToArray();
        }

    }

    private List<int> GetVerticeIndicesWithEmptyNeighbour()
    {
        List<int> indicesOfAvailableVertices = new List<int>();
        for (int verticeIndex = 0; verticeIndex < meshFilter.mesh.vertices.Length; verticeIndex++)
        {
            // Schau ob das vertice in nur 6 Triangles vorkommt
            var index = verticeIndex;
            if (meshFilter.mesh.triangles.Count(i => i == index) < 6)
            {
                indicesOfAvailableVertices.Add(verticeIndex);
            }
        }
        return indicesOfAvailableVertices;
    }

}
