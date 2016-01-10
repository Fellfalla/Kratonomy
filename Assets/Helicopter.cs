using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.VR;

[RequireComponent(typeof(Rigidbody2D))]
public class Helicopter : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    public int Force = 1;
    public float EnginePower = 1;

    public float MaxAcceleration = 5;

    public float PGain = 1;
    public float DGain = 1;
    public float Sensibility = 20 ;
    private float lastError;
    private Vector2 oldPos;
    private Vector2 savedValue;

    // Use this for initialization
    void Start ()
    {
        Cursor.SetCursor(Texture2D.whiteTexture, transform.position, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.None;
        
	    _rigidbody = GetComponent<Rigidbody2D>();
        oldPos = GetMouseMovement();
    }

    private Vector2 GetMouseMovement()
    {
        return new Vector2(Input.GetAxis("Mouse X") * Sensibility, Input.GetAxis("Mouse Y") * Sensibility);
    }

    // Update is called once per frame
    void FixedUpdate ()
	{

	    //float newX = Input.mousePosition.x;
	    //float newY = Input.mousePosition.y;
	    //Vector2 Force = GetForce();
        var Force = GetForceDirectFromMouse();
        _rigidbody.velocity += Vector2.Scale(Force,
            new Vector2(Time.fixedDeltaTime/_rigidbody.mass, Time.fixedDeltaTime/_rigidbody.mass));

	}


    private Vector2 GetForceDirectFromMouse()
    {
        Vector2 newPos = GetMouseMovement();
        var force = (newPos - oldPos);
        
        oldPos = newPos;
        return Vector2.Scale(force, new Vector2(EnginePower, EnginePower));
        
    }

    /// <summary>
    /// Source: http://answers.unity3d.com/questions/197225/pid-controller-simulation.html
    /// </summary>
    /// <returns></returns>
    private Vector2 GetPidControlledForce()
    {
        var toTargetVector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        var error = toTargetVector.magnitude; // generate the error signal
        var diff = (error - lastError) / Time.deltaTime; // calculate differential error
        lastError = error;
        // calculate the acceleration:
        var accel = error * PGain + diff * DGain;
        // limit it to the max acceleration
        accel = Mathf.Clamp(accel, -MaxAcceleration, MaxAcceleration);

        var force = accel*_rigidbody.mass;

        return Vector2.Scale(toTargetVector.normalized, new Vector2(force, force));
    }

    private Vector2 LimitEngineForce(Vector2 force)
    {
        if (force.sqrMagnitude > EnginePower)
        {
            return Vector2.Scale(force.normalized, new Vector2(EnginePower, EnginePower));
        }
        return force;
    }

}
