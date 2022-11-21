using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RopeRoot : MonoBehaviour
{
    public Rigidbody attach;

    [Header("Rope")]
    // Rope variables
    [SerializeField]
    private Rigidbody ropePointPrefab;
    [SerializeField][Range(0, 200)]
    private int ropePointAmount = 10;
    [SerializeField]
    private float ropePointScale = 0.1f;
    [SerializeField][Range(1.0f, 200.0f)]
    private float ropeLength = 2.5f;

    [Header("Constraints")]
    [SerializeField]
    private bool constraintsOverride = true;
    [SerializeField][Range(1, 100)]
    private int constraintsIterations = 10;
    private float constraintDistance;

    protected List<Rigidbody> points = new List<Rigidbody>();
    
    private LineRenderer line;
    private Rigidbody body;

    private void Awake() {
        line = GetComponent<LineRenderer>();
        body = GetComponent<Rigidbody>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Calculate positions
        Vector3 targetPosition;
        if (attach != null) targetPosition = attach.position;
        else targetPosition = transform.position + Vector3.down * ropeLength;
        
        // Create points
        points.Add(body);
        if (attach != null) points.Add(attach);
        for (int i = 0; i < ropePointAmount; i++) {
            Vector3 position = Vector3.Lerp(transform.position, targetPosition, (float)i / (ropePointAmount - 1));
            Rigidbody newPoint = Instantiate(ropePointPrefab, position, transform.rotation);
            newPoint.transform.localScale *= ropePointScale;
            points.Add(newPoint);
        }
        if (line != null) line.positionCount = points.Count;
        
        // Setup constraints
        constraintDistance = ropeLength / (ropePointAmount + 2);
    }

    private void FixedUpdate() {
        UpdatePoints();
        UpdateLine();
    }

    public void UpdatePoints() {
        if (!constraintsOverride) return;

        for (int iteration = 0; iteration < constraintsIterations; iteration++) {
            for (int pointIndex = 0; pointIndex < points.Count - 1; pointIndex++) {
                DistanceConstraint.ApplyConstraint(points[pointIndex], points[pointIndex + 1], constraintDistance);
            }
        }
    }

    public void UpdateLine() {
        if (line == null) return;

        for (int pointIndex = 0; pointIndex < points.Count - 1; pointIndex++) {
            line.SetPosition(pointIndex, points[pointIndex].position);
        }
    }
}
