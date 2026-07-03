using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class UAVControlSubscriber : MonoBehaviour
{
    ROSConnection ros;

    public string topicName = "/cntrl";

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        ros.Subscribe<PoseMsg>(topicName, PoseCallback);

        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    void Update()
//potentially move to pose callback 
    {
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    void PoseCallback(PoseMsg msg)
    {
        // Position
        targetPosition = new Vector3(
            (float)msg.position.y,
            (float)msg.position.z,
            (float)msg.position.x
        );

        // Rotation (Quaternion)
        targetRotation = new Quaternion(
            (float)msg.orientation.x,
            (float)msg.orientation.y,
            (float)msg.orientation.z,
            (float)msg.orientation.w
        );
    }
}