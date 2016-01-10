using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Framework.Extensions;
using UnityEngine;
using Random = System.Random;

public class MeshRounder : MonoBehaviour
{

    public MeshFilter MF;

    [Range(0, 10)]
    public int SamplingLevel = 1;

    [Range(0, 5)]
    public int PresamplingLevel = 0;

    [Range(-5, 5)]
    public float AmountOfRounding = 1.0f;

    [Range(0, 1f)]
    public float Noise = .0f;

    [Range(0, 4)]
    public int Smoothness = 0;

    [Range(0, 8)]
    public int SamplingIterations = 0;

    public int Seed = 516234251;

    private float _meshScale = 1;

    // Variables for saving old status
    //private float jitterOld = float.NaN;
    //private float amountOld = float.NaN;
    //private int LevelOld = int.MaxValue;
    private Mesh _initialMesh;

    private Dictionary<FieldInfo, object> OldStatus = new Dictionary<FieldInfo, object>(); 

    void Awake()
    {
        _initialMesh = Mesh.Instantiate(MF.mesh);

        foreach (var fieldInfo in GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
        {
            OldStatus.Add(fieldInfo, null);
        }
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (OldStatus.Any((pair) => !pair.Key.GetValue(this).Equals(pair.Value)))
        {
            RefreshStatus();

            Mesh mesh = Mesh.Instantiate(_initialMesh);
            RoundAndSubdivide(mesh, AmountOfRounding, Noise);   // divides a single quad into 6x6 quads

            MF.mesh = mesh;
            mesh.RecalculateNormals();
            mesh.Optimize();
        }
    }

    private void RefreshStatus()
    {
        foreach (var fieldInfo in OldStatus.Keys.ToArray())
        {
            OldStatus[fieldInfo] = fieldInfo.GetValue(this);
        }
    }

    #region Rounding Subdivision
    public void RoundAndSubdivide(Mesh mesh, float amount, float noise)
    {
    

        mesh = ClearDoubledVertices(mesh);
        // Predivide
        MeshHelper.Subdivide(mesh, PresamplingLevel);

        for (int i = 0; i < SamplingIterations; i++)
        {
            var center = new Vector3(0, 0, 0);
            var averageRadius = 0f;

            if (Smoothness > 0)
            {
                mesh = SmoothFilter.CreateSmoothMesh(mesh, Smoothness);
            }

            RefreshMeshScale(mesh);
            mesh = NoiseMesh(mesh, noise * _meshScale , i);

            // Postdivide
            MeshHelper.Subdivide(mesh, SamplingLevel);

            foreach (var vector3 in mesh.vertices)
            {
                center += vector3;
                averageRadius += vector3.magnitude;
            }

            averageRadius /= mesh.vertexCount;
            //float time = 10f;

            //Debug.DrawLine(transform.position + new Vector3(-averageRadius, 0,0), transform.position + new Vector3(averageRadius, 0, 0), Color.red,time);
            //Debug.DrawLine(transform.position + new Vector3(0,-averageRadius,0), transform.position + new Vector3(0,averageRadius, 0), Color.red, time);
            //Debug.DrawLine(transform.position + new Vector3(0, 0 ,- averageRadius), transform.position + new Vector3(0, 0, averageRadius), Color.red, time);

            mesh.vertices = MeshHelper.RoundVertices(mesh.vertices, amount, averageRadius, center);
        }
    }


    private void RefreshMeshScale(Mesh mesh)
    {
        int precision = 10;
        float scale = 0;

        for (int i = 0; i < precision; i++)
        {
            var first = (mesh.vertices[mesh.triangles[i]] - mesh.vertices[mesh.triangles[i+1]]).magnitude;
            var second = (mesh.vertices[mesh.triangles[i]] - mesh.vertices[mesh.triangles[i+2]]).magnitude;
            var third = (mesh.vertices[mesh.triangles[i+1]] - mesh.vertices[mesh.triangles[i+2]]).magnitude;

            // Get average triangle size
            scale += (first + second + third)/3f;
        }

        _meshScale = scale/precision;
    }

    private Mesh ClearDoubledVertices(Mesh mesh)
    {
        var vertices = mesh.vertices.ToList();
        var triangles = mesh.triangles;

        // Alle vertices überprüfen
        for (int i = 0; i < vertices.Count; i++)
        {
            // Für alle nachfolgenden vertices
            for (int j = i+1; j < vertices.Count;)
            {
                // Schau ob der Vertex einem nachfoilgenden Vertex gleich ist
                if (vertices[i].Equals(vertices[j]))
                {
                    // Doppelten vertex löschen und alle verweise in triangles ändern
                    triangles = MeshHelper.ReplaceIndices(triangles, j, i);
                    vertices.RemoveAt(j); // -> index j zeigt nun bereits auf das nächste element

                    // Nachfolgende Vertex indizes anpassen
                    for (int k = j; k < vertices.Count; k++)
                    {
                        // Alle rücken um 1 nach vorne
                        triangles = MeshHelper.ReplaceIndices(triangles, k+1, k);
                    }
                }
                else
                {
                    // inkrement index
                    j++;
                }
            }
        }
        
        mesh.triangles = triangles;
        mesh.vertices = vertices.ToArray();
        return mesh;
    }


    private Mesh NoiseMesh(Mesh mesh, float noise, int seedManipulator = 0)
    {
        var random = new Random(Seed + seedManipulator);
        var vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = vertices[i] + new Vector3(random.GetRandomFloat(-noise, noise), random.GetRandomFloat(-noise, noise), random.GetRandomFloat(-noise, noise));
        }
        mesh.vertices = vertices;
        return mesh;
    }

    #endregion  Rounding Subdivision
}
