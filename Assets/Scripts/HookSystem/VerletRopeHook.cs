using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VerletRopeHook : MonoBehaviour
{
    public VerletRope grabRope;

    [Header("Hooking")]
    [SerializeField] private float hookRange;
    
    private void Start() {
        if (grabRope != null) {
            transform.localScale = Vector3.one * hookRange;
        }
    }

    private void FixedUpdate() {
        if (grabRope != null) {
            transform.position = grabRope.prevPos[grabRope.pointsNb - 1];
        }
    }

    private void OnTriggerEnter(Collider other) {
        
    }
}
