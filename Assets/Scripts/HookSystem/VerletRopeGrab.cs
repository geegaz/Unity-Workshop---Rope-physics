using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VerletRopeGrab : XRSimpleInteractable
{
    [Header("Grabbing")]
    public Transform grabHand;
    public VerletRope grabRope;
    [SerializeField] private float grabRange = 0.1f;

    private VerletRope.AttachedPoint grabbedPoint = null;
    private int closestPointID = -1;

    private void Start() {
        if (grabRope != null) {
            grabRope.AttachPoint(0, grabRope.transform);
        }
    }

    private void Update() {
        if (grabHand != null && grabRope != null) {
            if (grabbedPoint != null) {
                transform.position = grabHand.position;
            } else {
                closestPointID = grabRope.GetClosestPoint(grabHand.position, grabRange);
                if (closestPointID >= 0) {
                    transform.position = grabRope.prevPos[closestPointID];
                } else {
                    transform.position = grabRope.transform.position;
                }
            }
        }
    }

    public void TryGrab(SelectEnterEventArgs args) {
       if (closestPointID > 0 && args.interactorObject.transform == grabHand) {
            grabbedPoint = grabRope.AttachPoint(closestPointID, transform);
            
        }
    }

    public void TryThrow(SelectExitEventArgs args) {
        if (grabbedPoint != null) {
            grabRope.DetachPoint(grabbedPoint);
            grabbedPoint = null;
        }
    }
}
