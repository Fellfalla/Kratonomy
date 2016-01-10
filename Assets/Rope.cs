using UnityEngine;
using System.Collections;
using System.Runtime.Remoting.Messaging;

[RequireComponent(typeof(Rigidbody2D))]
public class Rope : MonoBehaviour
{

    //public GameObject Target;
    public float Elasizity = 0.1f;
    public float ropeForce = 100f;

    private float sqrtRopeLength;
    private Rigidbody2D _rigidbody;
    public Rigidbody2D TargetRigidbody;


    // Use this for initialization
    void Start()
    {
        sqrtRopeLength = (transform.position - TargetRigidbody.transform.position).sqrMagnitude;
        _rigidbody = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        var distanceVector = (transform.position - TargetRigidbody.transform.position);
        var distance = distanceVector.sqrMagnitude;
        if (distance > sqrtRopeLength)
        {
            var stretch = distance - sqrtRopeLength;
            if (stretch > Elasizity)
            {
                stretch = Elasizity;
            }
            var force = stretch*ropeForce;
            var forceVector = Vector2.Scale(distanceVector.normalized, new Vector2(-force, -force));
            _rigidbody.AddForce(forceVector);

            if (TargetRigidbody != null)
            {
                TargetRigidbody.AddForce(forceVector);
            }
            
        }
    }
}
