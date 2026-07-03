using UnityEngine;

public class FollowCameraSimple : MonoBehaviour
{
    public Transform target;

    // Fixed world-space offset
    public Vector3 offset = new Vector3(0f, 5f, -10f);

    void LateUpdate()
    {
        if (target == null)
            return;

        // Keep fixed offset
        transform.position = target.position + offset;

        // Look at target
        transform.LookAt(target);
    }
}