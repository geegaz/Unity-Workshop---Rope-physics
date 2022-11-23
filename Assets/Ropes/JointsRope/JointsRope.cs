using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointsRope : MonoBehaviour
{
    [SerializeField]
    private Rigidbody pointPrefab;

    [SerializeField]
    private int pointsNb = 20;

    private LineRenderer line;
    private Rigidbody body;
    private Rigidbody[] points;
    
    private void Awake() {
        body = GetComponent<Rigidbody>();
        line = GetComponent<LineRenderer>();
    }

    private void Start() {
        points = new Rigidbody[pointsNb];
        points[0] = body;

        Rigidbody point;
        Rigidbody previousPoint = null;
        Joint pointJoint;
        for (int i = 1; i < pointsNb; i++)
        {
            point = Instantiate<Rigidbody>(pointPrefab, transform.position, transform.rotation);
            pointJoint = point.GetComponent<Joint>();

            if (previousPoint != null) {
                pointJoint.connectedBody = previousPoint;
            } else {
                pointJoint.connectedBody = body;
            }
            previousPoint = point;
            points[i] = point;
        }

        if (line != null) {
            line.positionCount = pointsNb;
        }
    }

    private void Update() {
        if (line != null) {
            for (int i = 0; i < pointsNb; i++) {
                line.SetPosition(i, points[i].position);
            }
        }
    }
}
