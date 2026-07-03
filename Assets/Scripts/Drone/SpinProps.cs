using UnityEngine;

public class DroneProps : MonoBehaviour
{
    public Transform propFL;
    public Transform propFR;
    public Transform propBL;
    public Transform propBR;

    public float spinSpeed = 2000f;

    void Update()
    {
        float spin = spinSpeed * Time.deltaTime;

        propFL.Rotate(Vector3.up * spin);
        propBR.Rotate(Vector3.up * spin);

        // opposite direction
        propFR.Rotate(Vector3.up * -spin);
        propBL.Rotate(Vector3.up * -spin);
    }
}