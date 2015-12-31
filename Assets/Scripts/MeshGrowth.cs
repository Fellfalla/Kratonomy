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
    public float GrowSize = 1f;

    /// <summary>
    /// Adds a new Face every * seconds
    /// </summary>
    public float GrowRate = 1f;

    private MeshFilter _meshFilter;

    // Use this for initialization
    void Awake()
    {

        if ((_meshFilter = gameObject.GetComponent<MeshFilter>()) == null)
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        if (_meshFilter.mesh == null || _meshFilter.mesh.vertexCount == 0)
        {
            _meshFilter.mesh = CreateInitialMesh(Width, Height);
        }

        if (gameObject.GetComponent<MeshRenderer>() == null)
        {
            gameObject.AddComponent<MeshRenderer>();
        }

    }

    // Use this for initialization
    void Start()
    {

        InvokeRepeating("Grow", GrowRate, GrowRate);
    }

    private static Mesh CreateInitialMesh(float width, float height)
    {
        var mesh = new Mesh();
        mesh.MarkDynamic();

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-width, -height, 0),
            new Vector3(-width, height, 0),
            new Vector3(width, height, 0),
            new Vector3(width, -height, 0),
        };

        int[] triangles = new int[6];

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 3;

        triangles[3] = 1;
        triangles[4] = 2;
        triangles[5] = 3;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        return mesh;
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
        Vertice vertice = new Vertice(_meshFilter.mesh, choosenVertex);

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
            var initialVector = Vector3.up;
            initialVector.Scale(new Vector3(GrowSize, GrowSize, GrowSize));
            Vector3 targetPoint = Quaternion.Euler(0, 0, Random.Range(space.Begin + minAngle, space.Begin + maxAngle)) * initialVector +
                            _meshFilter.mesh.vertices[choosenVertex];

            int first = vertice.Index;
           
            int third;
            if (space.OpeningVertice != null)
            {
                third = space.OpeningVertice.Index;
            }
            else if (space.ClosingVertice != null)
            {
                third = space.ClosingVertice.Index;
            }
            else
            {
                Debug.Log("No Vertice for Growth found");
                return;
            }

            var range = Convert.ToSingle(GrowSize/2);
            var second = ExistingVertexWithinRange(vertice.Mesh, targetPoint, range);

            if (second == null) // look no existing Vertex is near
            {
                second = AddNewVertex(space, targetPoint, vertice);
            }

            CreateTriangle(vertice.Mesh, first, second.Value, third);

        }
        else
        {
            if (space.OpeningVertice == null || space.ClosingVertice == null)
            {
                return;
            }

            CreateTriangle(vertice.Mesh, vertice.Index, space.OpeningVertice.Index, space.ClosingVertice.Index);

        }
        FinishGrow();
    }

    private int? ExistingVertexWithinRange(Mesh mesh, Vector3 position, float sqareRange)
    {
        var nearVertices = new Dictionary<int, float>();

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            var dist = (mesh.vertices[i] - position).sqrMagnitude;
            if (sqareRange > dist)
            {
                var range = Convert.ToSingle(Math.Sqrt(sqareRange));
                Debug.DrawLine(position - new Vector3(range, 0, 0), position + new Vector3(range, 0, 0), Color.red, 0.8f);
                Debug.DrawLine(position - new Vector3(0, range, 0), position + new Vector3(0, range, 0), Color.red, 0.8f);
                nearVertices.Add(i,dist);
            }
            
        }
        if (nearVertices.Any())
        {
            return nearVertices.OrderBy(pair => pair.Value).Select(pair => pair.Key).First();
        }
        else
        {
            return null;
        }
    }


    private int AddNewVertex(RadialSpace space, Vector3 newVertex, Vertice vertice)
    {
        // Add new vertice in free space
        var newVertexLength = 1;
        newVertex *= newVertexLength;
        _meshFilter.mesh.vertices = _meshFilter.mesh.vertices.Concat(new[] {newVertex}).ToArray();

        return _meshFilter.mesh.vertices.Length - 1;

    }

    private void CreateTriangle(Mesh mesh, int first, int second, int third)
    {
        // Calculate the right direction 
        var crossProd = Vector3.Cross(mesh.vertices[second] - mesh.vertices[first],
            mesh.vertices[third] - mesh.vertices[first]);
        if (Vector3.Dot(crossProd, Vector3.forward) < 0) // both look in contrary direction and order is ok
        {
            mesh.triangles =
                _meshFilter.mesh.triangles.Concat(new[]
                {first, second, third}).ToArray();
        }
        else
        {
            mesh.triangles =
                _meshFilter.mesh.triangles.Concat(new[]
                {first, third, second}).ToArray();
        }
    }


    private void FinishGrow()
    {
        _meshFilter.mesh.RecalculateNormals();
        _meshFilter.mesh.Optimize();
    }

    private List<int> GetVerticeIndicesWithEmptyNeighbour()
    {
        List<int> indicesOfAvailableVertices = new List<int>();
        for (int verticeIndex = 0; verticeIndex < _meshFilter.mesh.vertices.Length; verticeIndex++)
        {
            // Schau ob das vertice in nur 6 Triangles vorkommt
            var index = verticeIndex;
            if (_meshFilter.mesh.triangles.Count(i => i == index) < 6)
            {
                indicesOfAvailableVertices.Add(verticeIndex);
            }
        }
        return indicesOfAvailableVertices;
    }

}
