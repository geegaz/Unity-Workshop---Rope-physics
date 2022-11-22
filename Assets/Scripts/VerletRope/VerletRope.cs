using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletRope : MonoBehaviour
{
    [Header("Rope")]
    [SerializeField]
    private Transform attachPoint;
    [SerializeField]
    private Rigidbody attachBody;

    public Dictionary<int, Transform> attachPoints = new Dictionary<int, Transform>();
    
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
    }

    private void Start() {
        CreatePoints();

        attachPoints.Add(0, transform);
        if (attachPoint) attachPoints.Add(pointsNb - 1, attachPoint);
    }

    private void FixedUpdate() {
        if (pointsNb > 1) {
            ApplyVerlet();
            ApplyConstraints();
            ApplyAttach();
        }

        if (line)
            line.SetPositions(pos);
        
        if (attachBody)
            mass[pointsNb - 1] = attachBody.mass;
            attachBody.MovePosition(pos[pointsNb - 1]);
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
        }

        if (line) line.positionCount = pointsNb;
    }

    private void ApplyAttach() {
        foreach (int point in attachPoints.Keys)
        {
            if (point >= 0 && point < pointsNb) {
                pos[point] = attachPoints[point].position;
            }
        }
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
                // pos[i] -= delta * diff * invmass2;
                // pos[i - 1] += delta * diff * invmass1;
            }
        }
    }

    private static float InverseMass(float mass) {
        return mass == 0.0f ? 1000000000.0f : 1.0f / mass;
    }
}
