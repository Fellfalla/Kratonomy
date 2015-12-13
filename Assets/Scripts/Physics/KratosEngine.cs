using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class KratosEngine : MonoBehaviour
{
    public float GravitationalConstant = 1f;
    private float sqrMinDistance = 0.1f;

    private List<Rigidbody> _masses = new List<Rigidbody>(); 

	// Use this for initialization
	void Start () {
	
	}

    void Awake()
    {
        _masses.AddRange(FindObjectsOfType<Rigidbody>());
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

	}

    void FixedUpdate()
    {
        HandleGravityForce();
    }

    void HandleGravityForce()
    {
        GravityForce2DList(_masses);
    }


    void GravityForce2D(Rigidbody first, Rigidbody second)
    {
        var distanceVector = second.transform.position - first.transform.position; //vector in direction of second
        var sqrdistance = distanceVector.sqrMagnitude;
        if (sqrdistance < sqrMinDistance)
        { // damit F nicht gegen unendlich geht
            sqrdistance = sqrMinDistance;
        }

        var force = 0.5f * distanceVector.normalized * GravitationalConstant * first.mass * second.mass / sqrdistance;
        first.AddForce(force);
        second.AddForce(-force);


    }

    void GravityForce2DList(IEnumerable<Rigidbody> masses)
    {
        var handledMasses = new List<Rigidbody>();

        foreach (var mass in masses)
        {
            handledMasses.Add(mass); // Alle nachfolgenden Massen interagieren nachfolgend mit dieser masse

            var reactors = masses.Except(handledMasses); // Berechne Kräfte zu verbleibenden Massen

            foreach (var reactor in reactors)
            {
                GravityForce2D(mass, reactor);
            }

        }
    }
}
