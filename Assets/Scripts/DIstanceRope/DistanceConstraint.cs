using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DistanceConstraint : MonoBehaviour
{
    public Rigidbody target;
    private Rigidbody body;
    
    public float distance;
    public bool useStartDistance = true;

    private void Awake() {
        body = GetComponent<Rigidbody>();
    }

    private void Start() {
        if (useStartDistance && target != null) distance = Vector3.Distance(body.position, target.position);
    }

    private void FixedUpdate() {
        if (target != null) ApplyConstraint(body, target, distance);
    }

    public static void ApplyConstraint(Rigidbody b1, Rigidbody b2, float distance) {
        Vector3 vec = b2.position - b1.position;
        float difference = distance - vec.magnitude;
        Vector3 movement = vec.normalized * difference * 0.5f;
        
        // b1.position += movement * InverseMass(b1);
        // b2.position -= movement * InverseMass(b2);
        
        b1.velocity = b1.velocity + movement * Time.fixedDeltaTime;
        b2.velocity = b2.velocity - movement * Time.fixedDeltaTime;
    }

    private static float InverseMass(Rigidbody body){
        return (body.isKinematic ? 0.0f : 1.0f);
    }
}
