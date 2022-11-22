using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletRope : MonoBehaviour
{
    [Header("Rope")]
    [SerializeField]
    private Transform attachPoint;
    private Rigidbody attachBody;
    
    Vector3[] pos;
    Vector3[] prevPos;
    float[] mass;
    
    public int pointsNb = 20;

    [Header("Constraints")]
    public float constraintDistance = 0.1f;
    public int constraintIterations = 10;

    private LineRenderer line;

    private void Awake() {
        line = GetComponent<LineRenderer>();
        if (attachPoint != null) 
            attachBody = attachPoint.GetComponent<Rigidbody>();
    }

    private void Start() {
        CreatePoints();
    }

    private void FixedUpdate() {
        if (pointsNb > 1) {
            ApplyVerlet();
            ApplyConstraints();

            pos[0] = transform.position;
            if (attachPoint) {
                pos[pointsNb - 1] = attachPoint.position;
            }
        }

        if (line)
            line.SetPositions(pos);
    }

    private void CreatePoints() {
        pos = new Vector3[pointsNb];
        prevPos = new Vector3[pointsNb];
        mass = new float[pointsNb];

        Vector3 targetPos = attachPoint ? attachPoint.position : transform.position + Physics.gravity.normalized * (constraintDistance * pointsNb);
        for (int i = 0; i < pointsNb; i ++) {
            pos[i] = Vector3.Lerp(transform.position, targetPos, (float)i / (pointsNb - 1));
            prevPos[i] = pos[i];
            mass[i] = 1.0f;

            Debug.Log(pos[i] + " " + prevPos[i]);
        }

        mass[0] = 0.0f;
        if (attachPoint) mass[pointsNb - 1] = 0.0f;
        if (line) line.positionCount = pointsNb;
    }

    private void ApplyVerlet() {
        Vector3 temp;
        for (int i = 0; i < pointsNb; i++) {
            temp = pos[i];
            pos[i] += pos[i] - prevPos[i];
            pos[i] += mass[i] * Physics.gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
            prevPos[i] = temp;
        }
    }

    private void ApplyConstraints() {
        Vector3 delta;
        float length;
        float diff;
        // float invmass1;
        // float invmass2;
        for (int iteration = 0; iteration < constraintIterations; iteration++) {
            for (int i = 1; i < pointsNb; i++) {
                delta = pos[i] - pos[i - 1];
                length = delta.magnitude;

                diff = (length - constraintDistance) / length;
                pos[i] -= delta * diff * 0.5f;
                pos[i - 1] += delta * diff * 0.5f;
                
                // invmass1 = InverseMass(mass[i - 1]);
                // invmass2 = InverseMass(mass[i]);
                // diff = (length - constraintDistance) / (length * (invmass1 + invmass2));
                // pos[i - 1] -= delta * diff * invmass1;
                // pos[i] += delta * diff * invmass2;
            }
        }
    }

    private static float InverseMass(float mass) {
        return mass == 0.0f ? float.PositiveInfinity : 1.0f / mass;
    }
}
