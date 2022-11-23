using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    enum Axis {
        X,
        Y,
        Z
    }

    [SerializeField]
    private Axis rotationAxis = Axis.X;
    [SerializeField]
    private float rotationSpeed;

    // Update is called once per frame
    void Update()
    {
        switch (rotationAxis)
        {
            case Axis.X:
                transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
                break;
            case Axis.Y:
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                break;
            case Axis.Z:
                transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
                break;
        }
    }
}
