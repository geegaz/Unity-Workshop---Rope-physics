using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class VerletRopeHook : MonoBehaviour
{
    public VerletRope grabRope;
    [SerializeField] private float hookMass = 2.0f;

    [SerializeField] private InputActionProperty releaseAction;

    private VerletRope.AttachedPoint hookedPoint;
    private int ropePointID;
    
    private void Start() {
        if (releaseAction != null) {
            if (releaseAction.reference) releaseAction.reference.action.performed += OnRelease;
            else releaseAction.action.performed += OnRelease;
        }
        if (grabRope != null) {
            ropePointID = grabRope.pointsNb - 1;
        }
    }

    private void Update() {
        if (grabRope != null) {
            transform.position = grabRope.pos[ropePointID];
            if (hookedPoint == null) {
                grabRope.mass[ropePointID] = hookMass;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "HookPoint") {
            hookedPoint = grabRope.AttachPoint(ropePointID, other.transform);
        }
    }

    private void OnRelease(InputAction.CallbackContext ctx) {
        if (ctx.performed && hookedPoint != null) {
            grabRope.DetachPoint(hookedPoint);
            hookedPoint = null;
        }
    }
}
