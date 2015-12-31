using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class MeshRounder : MonoBehaviour
{

    public MeshFilter MF;

    [Range(0, 100)]
    public int Level = 4;

    [Range(-5, 5)]
    public float AmountOfRounding = 1.0f;

    [Range(-5, 5)]
    public float Noise = .0f;

    public bool RoundSubdivide4 = true;

    public bool RoundSubdivide9 = true;

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
            RoundAndSubdivide(mesh, Level, AmountOfRounding, Noise);   // divides a single quad into 6x6 quads
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
    public void RoundAndSubdivide(Mesh mesh, int level, float amount, float jitter)
    {
        var center = new Vector3(0, 0, 0);
        var averageRadius = 0f;

        MeshHelper.Subdivide(mesh, level);

        foreach (var vector3 in mesh.vertices)
        {
            center += vector3;
            averageRadius += vector3.magnitude;
        }

        averageRadius /= mesh.vertexCount;
        float time = 10f;

        Debug.DrawLine(transform.position + new Vector3(-averageRadius, 0,0), transform.position + new Vector3(averageRadius, 0, 0), Color.red,time);
        Debug.DrawLine(transform.position + new Vector3(0,-averageRadius,0), transform.position + new Vector3(0,averageRadius, 0), Color.red, time);
        Debug.DrawLine(transform.position + new Vector3(0, 0 ,- averageRadius), transform.position + new Vector3(0, 0, averageRadius), Color.red, time);

        
        mesh.vertices = MeshHelper.RoundVertices(mesh.vertices, amount, jitter, averageRadius, center);
    }
    #endregion  Rounding Subdivision
}
