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
        float difference = (distance - vec.magnitude) / vec.magnitude;
        
        b1.velocity = b1.velocity - vec * difference * 0.5f;
        b2.velocity = b2.velocity + vec * difference * 0.5f;
    }

    private static float InverseMass(Rigidbody body){
        return (body.mass == 0.0 ? float.PositiveInfinity : 1.0f);
    }
}
