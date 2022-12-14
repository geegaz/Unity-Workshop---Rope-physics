using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletRope : MonoBehaviour
{
    public int pointsNb = 20;
    
    [System.Serializable]
    public class AttachedPoint
    {
        public int id = 0;
        public Transform transform;
        [HideInInspector] public Vector3 force = Vector3.zero;

        public AttachedPoint(int _id, Transform _transform) {
            id = _id;
            transform = _transform;
        }

        public bool IsValid(int maxPoints = 0) {
            return !(id < 0 || id >= maxPoints || transform == null);
        }
    }
    
    [Header("Rope")]
    [SerializeField] private float attachedBodiesDamping = 0.5f;

    [SerializeField] private List<AttachedPoint> attachedPoints = new List<AttachedPoint>();
    
    [HideInInspector] public Vector3[] pos;
    [HideInInspector] public Vector3[] prevPos;
    [HideInInspector] public float[] mass;

    [Header("Constraints")]

    public float constraintHeightMin = 0.01f;
    public float constraintHeightFriction = 0.5f;

    [Space]
    public float constraintDistance = 0.1f;
    public int constraintDistanceIterations = 20;

    private LineRenderer line;

    private void Awake() {
        line = GetComponent<LineRenderer>();
    }

    private void Start() {
        CreatePoints();
    }

    private void FixedUpdate() {
        if (pointsNb > 1) {
            ApplyForces();
            ApplyAttach();
            
            ApplyVerlet();
            ApplyConstraints();
        }

        if (line)
            line.SetPositions(pos);
    }

    private void CreatePoints() {
        pos = new Vector3[pointsNb];
        prevPos = new Vector3[pointsNb];
        mass = new float[pointsNb];

        Vector3 targetPos = transform.position + Physics.gravity.normalized * (constraintDistance * pointsNb);
        if (attachedPoints.Count > 1) {
            AttachedPoint lastPoint = attachedPoints[attachedPoints.Count - 1];
            if (lastPoint.IsValid(pointsNb)) targetPos = lastPoint.transform.position;
        }
        for (int i = 0; i < pointsNb; i ++) {
            pos[i] = Vector3.Lerp(transform.position, targetPos, (float)i / (pointsNb - 1));
            prevPos[i] = pos[i];
            mass[i] = 1.0f;
        }

        if (line) line.positionCount = pointsNb;
    }

    public AttachedPoint AttachPoint(int id, Transform attach) {
        AttachedPoint newPoint = new AttachedPoint(id, attach);
        AttachedPoint point;
        for (int i = 0; i < attachedPoints.Count; i++)
        {
            point = attachedPoints[i];
            if (point.id == id) {
                point.transform = attach;
                point.force = Vector3.zero;
                return point;
            } else if (point.id > id) {
                attachedPoints.Insert(i, newPoint);
                return newPoint;
            }
        }
        attachedPoints.Add(newPoint);
        return newPoint;
    }

    public void DetachPoint(AttachedPoint point) {
        attachedPoints.Remove(point);
        mass[point.id] = 1.0f;
    }

    public void DetachPoint(int id) {
        foreach (AttachedPoint point in attachedPoints)
        {
            if (point.id == id) {
                DetachPoint(point);
                return;
            }
        }
    }

    public int GetClosestPoint(Vector3 targetPos, float range = float.PositiveInfinity) {
        float distance;
        float distanceMin = range;
        int pointMin = -1;
        for (int i = 0; i < pointsNb; i++)
        {
            distance = Vector3.Distance(targetPos, pos[i]);
            if (distance < distanceMin) {
                distanceMin = distance;
                pointMin = i;
            }
        }
        return pointMin;
    }

    private float GetConstraint(int p1, int p2, float distance, Vector3[] constraint, bool useMass = true) {
        Vector3 delta = pos[p2] - pos[p1];
        float length = delta.magnitude;
        float difference;
        if (useMass) {
            float invmass1 = InverseMass(mass[p1]);
            float invmass2 = InverseMass(mass[p2]);
            difference = (length - distance) / (length * (invmass1 + invmass2));
            constraint[0] = delta * difference * invmass1;
            constraint[1] = -delta * difference * invmass2;
            return difference;
        } else {
            difference = (length - distance) / length;
            constraint[0] = delta * difference * 0.5f;
            constraint[1] = -delta * difference * 0.5f;
            return difference;
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
        Vector3[] constraint = new Vector3[2];
        for (int iteration = 0; iteration < constraintDistanceIterations; iteration++) {
            for (int i = 1; i < pointsNb; i++) {
                if (pos[i].y < constraintHeightMin) {
                    prevPos[i] = Vector3.Lerp(prevPos[i], pos[i], constraintHeightFriction);
                    pos[i].y = constraintHeightMin;
                }

                float diff = GetConstraint(i-1, i, constraintDistance, constraint);
                pos[i - 1] += constraint[0];
                pos[i] += constraint[1];
            }
        }
    }

    private void ApplyAttach() {
        Vector3[] constraint = new Vector3[2];
        AttachedPoint previousPoint = null;
        foreach (AttachedPoint point in attachedPoints){
            if (!point.IsValid(pointsNb)) continue;

            pos[point.id] = point.transform.position;
            prevPos[point.id] = point.transform.position;
            mass[point.id] = 0.0f;

            if (previousPoint != null) {
                int points = point.id - previousPoint.id;
                float diff = GetConstraint(previousPoint.id, point.id, constraintDistance * points, constraint);
                if (diff > 0.0f){
                    previousPoint.force += constraint[0];
                    point.force = constraint[1];
                }
            } else {
                point.force = Vector3.zero;
            }
            previousPoint = point;
        }
    }

    private void ApplyForces() {
        Rigidbody body = null;
        foreach (AttachedPoint point in attachedPoints) {
            if (!point.IsValid(pointsNb)) continue;

            body = point.transform.GetComponent<Rigidbody>();
            if (body != null && !body.isKinematic) {
                mass[point.id] = body.mass;
                body.velocity += point.force * attachedBodiesDamping;
            }
        }
    }

    private static float InverseMass(float mass) {
        return mass == 0.0f ? 0.00000001f : 1.0f / mass;
    }
}
