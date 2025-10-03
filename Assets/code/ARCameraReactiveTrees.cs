using UnityEngine;
using System.Collections.Generic;

public class ARCameraReactiveTrees : MonoBehaviour
{
    [Header("Setup")]
    public Transform arCamera;                   // Drag your ARCamera here
    public List<Transform> targets;              // Drag tree objects here in Inspector

    [Header("Rotation Settings")]
    [Range(1f, 90f)] public float maxYawAngle = 30f;    // Max left/right rotation
    [Range(10f, 360f)] public float turnSpeed = 120f;   // How fast trees turn

    private Dictionary<Transform, Quaternion> baseRotations;

    void Start()
    {
        if (arCamera == null && Camera.main != null)
            arCamera = Camera.main.transform;

        // Store original local rotations
        baseRotations = new Dictionary<Transform, Quaternion>();
        foreach (var t in targets)
        {
            if (t != null) baseRotations[t] = t.localRotation;
        }
    }

    void Update()
    {
        if (arCamera == null) return;

        foreach (var t in targets)
        {
            if (t == null) continue;

            // Direction from object to camera (flattened on Y)
            Vector3 toCam = arCamera.position - t.position;
            toCam.y = 0; // ignore vertical tilt

            if (toCam.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(toCam, Vector3.up);
                Quaternion limitedRot = LimitYaw(baseRotations[t], targetRot, maxYawAngle);
                t.rotation = Quaternion.RotateTowards(t.rotation, limitedRot, turnSpeed * Time.deltaTime);
            }
        }
    }

    Quaternion LimitYaw(Quaternion baseRot, Quaternion targetRot, float limit)
    {
        float yawDiff = Mathf.DeltaAngle(baseRot.eulerAngles.y, targetRot.eulerAngles.y);
        yawDiff = Mathf.Clamp(yawDiff, -limit, limit);
        return Quaternion.Euler(0, baseRot.eulerAngles.y + yawDiff, 0);
    }
}
