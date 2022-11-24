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
        float invmass1 = InverseMass(b1);
        float invmass2 = InverseMass(b2);
        float length = vec.magnitude;
        float difference = (length - distance) / (length * (invmass1 + invmass2));
        Vector3 move1 = vec * difference * invmass1;
        Vector3 move2 = vec * difference * invmass2;
        
        if (!b1.isKinematic && difference > 0.0f) {
            b1.velocity += move1;
        }
        if (!b2.isKinematic && difference > 0.0f) {
            b2.velocity -= move2;
        }
    }

    private static float InverseMass(Rigidbody body){
        return (body.mass == 0.0 ? 0.00000001f : 1.0f);
    }
}
