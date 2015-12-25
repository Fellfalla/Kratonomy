using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Framework;
using Framework.Extensions;
using Random = UnityEngine.Random;

/// <summary>
/// Das Mesh muss aus Gleichseitigen Dreiecken bestehen
/// </summary>
public class MeshGrowth : MonoBehaviour
{
    public float Width = 1f;
    public float Height = 1f;

    private MeshFilter meshFilter;


	// Use this for initialization
	void Start ()
	{
        meshFilter = gameObject.AddComponent<MeshFilter>();
	    gameObject.AddComponent<MeshRenderer>();
        meshFilter.mesh = new Mesh();
        
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


        InvokeRepeating("Grow", 1, 1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void Grow()
    {
        var randomIndex1 = Random.Range(0, meshFilter.mesh.triangles.Length - 1);
        int randomIndex2;
        var rest = randomIndex1%3;
        if (rest == 2)
        {
            randomIndex2 = 1;
        }
        else
        {
            randomIndex2 = randomIndex1 + 1;
        }
        var vertex1 = meshFilter.mesh.vertices[meshFilter.mesh.triangles[randomIndex1]];
        var vertex2 = meshFilter.mesh.vertices[meshFilter.mesh.triangles[randomIndex2]];

        Vector3 vertex3;

        var distanze = vertex2 - vertex1;
        vertex3 = Quaternion.Euler(0, 0, 60) * vertex1 + vertex1;
        vertex3 = vertex3.normalized * distanze.magnitude;

        int newIndex = meshFilter.mesh.vertexCount;

        // Add new Vertex
        var newVertices = meshFilter.mesh.vertices.ToList();
        newVertices.Add(vertex3);
        meshFilter.mesh.vertices = newVertices.ToArray();

        // Add new Triangle
        var newTriangles = meshFilter.mesh.triangles.ToList();
        newTriangles.AddRange(new []{ randomIndex1 , randomIndex2 , newIndex });
        meshFilter.mesh.triangles = newTriangles.ToArray();
    }

    /// <summary>
    /// Lässt das Mesh wachsen
    /// </summary>
    private void Grow2()
    {
        // Wähle ein freies Vertice aus.
        GetVerticeIndicesWithEmptyNeighbour();
    }

    private List<int> GetVerticeIndicesWithEmptyNeighbour()
    {
        List<int> indicesOfAvailableVertices = new List<int>();
        for(int verticeIndex = 0; verticeIndex < meshFilter.mesh.vertices.Length; verticeIndex++)
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

    /// <summary>
    /// Gibt Tuples mit nachbarn zurück und Informationen darüber, ob diese Nachbarn besetzt sind.
    /// </summary>
    private FreeSpace2D GetFreeSpace(int verticeIndex)
    {
        var result = new FreeSpace2D();

        var neighbourUsages = new Dictionary<int, int> ();

        var triangles = new List<int[]>();

        
        for (int triangle = 0; triangle + 2 < meshFilter.mesh.triangles.Length; triangle += 2)
        {
            // Get all using triangles
            var currentTriangle = meshFilter.mesh.triangles.SubArray(triangle, 2);
            if (meshFilter.mesh.triangles.SubArray(triangle,2).Contains(verticeIndex))
            {
                triangles.Add(currentTriangle);

                // Get all existing neighbours
                foreach (var index in currentTriangle.Where(index => index != verticeIndex))
                {
                    neighbourUsages[index]++;
                }
            }
        }

        // Find neighbours with free second neighbour
        var freeNeighbours = new List<Tuple<int, Vector3>>();
        foreach (var freeNeighbour in neighbourUsages.Where((pair) => pair.Value == 1))
        {
            freeNeighbours.Add(Tuple.New(freeNeighbour.Key, meshFilter.mesh.vertices[freeNeighbour.Key]));
        }

        // Remove free spaces 
        


        return result;
    }

}
