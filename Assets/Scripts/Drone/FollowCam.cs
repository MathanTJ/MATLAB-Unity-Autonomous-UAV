using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;

    // Camera offset relative to target
    public Vector3 offset = new Vector3(0f, 5f, -10f);

    // Higher = faster yaw response
    public float yawSmoothSpeed = 5f;

    private float currentYaw;

    void Start()
    {
        if (target != null)
        {
            currentYaw = target.eulerAngles.y;
        }
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // Smoothly ease toward target yaw
        currentYaw = Mathf.LerpAngle(
            currentYaw,
            target.eulerAngles.y,
            yawSmoothSpeed * Time.deltaTime
        );

        // Only rotate around Y axis
        Quaternion yawRotation = Quaternion.Euler(0f, currentYaw, 0f);

        // Rotate offset around Y
        Vector3 rotatedOffset = yawRotation * offset;

        // Position camera
        transform.position = target.position + rotatedOffset;

        // Keep camera level while looking at target
        Vector3 lookTarget = target.position + Vector3.up * offset.y;

        transform.LookAt(lookTarget);
    }
}