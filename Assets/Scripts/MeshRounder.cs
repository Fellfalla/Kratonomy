using UnityEngine;
using System.Collections;

public class MeshRounder : MonoBehaviour
{

    public MeshFilter MF;
    public int Level = 4;
    public float amount = 1.0f;
    public float jitter = .0f;

    private float jitterOld = float.NaN;
    private float amountOld = float.NaN;
    private int LevelOld = int.MaxValue;

    private Mesh initialMesh;

    void Awake()
    {
        initialMesh = Mesh.Instantiate(MF.mesh);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Level != LevelOld || amount != amountOld || jitter != jitterOld)
        {
            jitterOld = jitter;
            amountOld = amount;
            LevelOld = Level;

            Mesh mesh = Mesh.Instantiate(initialMesh);
            MeshHelper.RoundAndSubdivide(mesh, Level, amount, jitter);   // divides a single quad into 6x6 quads
            MF.mesh = mesh;
            mesh.RecalculateNormals();
            mesh.Optimize();
        }
    }
}
