using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletRope : MonoBehaviour
{
    [Header("Rope")]
    [SerializeField]
    private Rigidbody attachedBody;
    [SerializeField]
    private float attachedBodiesDamping = 0.8f;

    private SortedDictionary<int, Transform> pointsAttach = new SortedDictionary<int, Transform>();
    private Dictionary<int, Vector3> pointsForce = new Dictionary<int, Vector3>();
    
    private Vector3[] pos;
    private Vector3[] prevPos;
    private float[] mass;
    
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
    }

    private void FixedUpdate() {
        if (pointsNb > 1) {
            ApplyVerlet();
            ApplyConstraints();
            
            ApplyAttach();
            ApplyForces();
        }

        if (line)
            line.SetPositions(pos);
    }

    private void CreatePoints() {
        pos = new Vector3[pointsNb];
        prevPos = new Vector3[pointsNb];
        mass = new float[pointsNb];

        Vector3 targetPos = attachedBody ? attachedBody.position : transform.position + Physics.gravity.normalized * (constraintDistance * pointsNb);
        for (int i = 0; i < pointsNb; i ++) {
            pos[i] = Vector3.Lerp(transform.position, targetPos, (float)i / (pointsNb - 1));
            prevPos[i] = pos[i];
            mass[i] = 1.0f;
        }

        CreateAttach(0, transform);
        if (attachedBody) CreateAttach(pointsNb - 1, attachedBody.transform);

        if (line) line.positionCount = pointsNb;
    }

    public void CreateAttach(int point, Transform attach) {
        if (point < 0 || point >= pointsNb){
            Debug.LogError(string.Format("Point {0} is outside of points amount {1}",point,pointsNb));
            return;
        }
        if (pointsAttach.ContainsKey(point)) {
            pointsAttach[point] = attach;
            pointsForce[point] = Vector3.zero;
            return;
        }

        pointsAttach.Add(point, attach);
        pointsForce.Add(point, Vector3.zero);
    }

    public void RemoveAttach(int point) {
        if (pointsAttach.ContainsKey(point)) {
            pointsAttach.Remove(point);
            pointsForce.Remove(point);
        } else {
            Debug.LogError(string.Format("No point {0} in attached points",point));
        }
    }

    private void ApplyAttach() {
        Vector3 delta;
        float distance;
        float length;
        float diff;
        float invmass1;
        float invmass2;
        int previousPoint = -1;
        foreach (int point in pointsAttach.Keys)
        {
            pos[point] = pointsAttach[point].position;

            if (previousPoint >= 0) {
                // pointsAttach should be ordered by id
                delta = pos[point] - pos[previousPoint];
                distance = constraintDistance * (point - previousPoint);
                length = delta.magnitude;
                invmass1 = InverseMass(mass[previousPoint]);
                invmass2 = InverseMass(mass[point]);
                diff = (length - distance) / (length * (invmass1 + invmass2));
                
                if (diff > 0.0f) {
                    pointsForce[previousPoint] += delta * diff * invmass1;
                    pointsForce[point] = -delta * diff * invmass2;
                } else {
                    pointsForce[point] = Vector3.zero;
                }
            } else {
                pointsForce[point] = Vector3.zero;
            }
            previousPoint = point;
        }
    }

    private void ApplyForces() {
        Rigidbody body;
        foreach (int point in pointsAttach.Keys) {
            body = pointsAttach[point].GetComponent<Rigidbody>();
            if (body) {
                mass[point] = body.mass;
                body.velocity += pointsForce[point];
                body.position += (body.velocity) * Time.deltaTime;
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
        float invmass1;
        float invmass2;
        for (int iteration = 0; iteration < constraintIterations; iteration++) {
            for (int i = 1; i < pointsNb; i++) {
                delta = pos[i] - pos[i - 1];
                length = delta.magnitude;
                invmass1 = InverseMass(mass[i - 1]);
                invmass2 = InverseMass(mass[i]);
                diff = (length - constraintDistance) / (length * (invmass1 + invmass2));
                
                pos[i - 1] += delta * diff * invmass1;
                pos[i] -= delta * diff * invmass2;
            }
        }
    }

    private static float InverseMass(float mass) {
        return mass == 0.0f ? 0.00000001f : 1.0f / mass;
    }
}
