using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
public class MeshUnifier : MonoBehaviour {

	// Use this for initialization
    void OnCollisionEnter(Collision col)
    {
        var otherMesh = col.collider.GetComponent<MeshFilter>().mesh;

        var combine = new[]
        {
            new CombineInstance()
            {
                mesh = otherMesh,
            }
        };


        //col.gameObject.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        var mesh = col.gameObject.GetComponent<MeshFilter>().mesh;
        var vertices = mesh.vertices;
        mesh.MarkDynamic();
        mesh.SetVertices(mesh.vertices.Select<Vector3,Vector3>((vertice) => {
            return Vector3.Scale(vertice, new Vector3(1.1f, 1.1f, 1.1f));
        }).ToList());
        //for (int i = 0; i< vertices.Length; i++)
        //{
        //    //vertices[i] = Vector3.Scale(vertices[i],new Vector3(1.5f,1.5f,1.5f));
        //}
        
    }
}
