using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private List<Transform> targets;
    [SerializeField]
    private float travelingSpeed = 2.0f;
    
    private float time = 1.0f;
    private int lastAxis = 0;
    private int currentTarget = 0;

    // Update is called once per frame
    private void Update()
    {
        int axis = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        if (Mathf.Abs(axis) > 0 && (time <= 0.0f || axis != lastAxis)) {
            currentTarget = Mathf.RoundToInt(Mathf.Repeat(currentTarget + axis, targets.Count));
            time = 1.0f;
        }
        lastAxis = axis;

        if (time >= 0.0f) {
            if (cam != null) {
                cam.transform.position = Vector3.Lerp(cam.transform.position, targets[currentTarget].position, 1.0f - time);
                cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, targets[currentTarget].rotation, 1.0f - time);
            }
            time -= Time.deltaTime * travelingSpeed;
        }
    }
}
