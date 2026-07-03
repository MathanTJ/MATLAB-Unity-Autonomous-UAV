using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;
using System;
using System.Collections.Generic;

public class SimplePointCloudPublisher : MonoBehaviour
{
    public string pointCloudTopic = "/point_cloud";

    public int numRays = 360;
    public float fovDeg = 360f;

    public float rangeMax = 30f;

    public float scanRateHz = 10f;

    public Vector3 lidarOffset = new Vector3(0.0f, 0.2f, 0.0f);

    ROSConnection ros;
    float nextPublishTime = 0f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PointCloud2Msg>(pointCloudTopic);
    }

    void Update()
    {
        float now = Time.time;

        if (now < nextPublishTime)
            return;

        nextPublishTime = now + 1.0f / scanRateHz;

        PublishPointCloud();
    }

    void PublishPointCloud()
    {
        float angleMin = -fovDeg * 0.5f * Mathf.Deg2Rad;
        float angleMax =  fovDeg * 0.5f * Mathf.Deg2Rad;
        float angleInc = (angleMax - angleMin) / (numRays - 1);

        Vector3 pos = transform.position;
        Vector3 origin = pos + lidarOffset;

        float YawRad = transform.eulerAngles.y * Mathf.Deg2Rad;
     
        List<byte> pointData = new List<byte>();

        for (int i = 0; i < numRays; i++)
        {
            float angle = angleMin + i * angleInc;
            float globalAngle = YawRad + angle;

            float dirX = Mathf.Cos(globalAngle);
            float dirZ = Mathf.Sin(globalAngle);

            Vector3 dirWorld = new Vector3(dirX, 0f, dirZ);

            float dist;

            if (Physics.Raycast(origin, dirWorld, out RaycastHit hit, rangeMax))
            {
                dist = hit.distance;
            }
            else
            {
                // No hit
                dist = rangeMax + 1f;
            }

            // Point in lidar frame (MATLAB Axis)
            float px = Mathf.Sin(angle) * dist;
            float py = Mathf.Cos(angle) * dist;
            float pz = 0f;

            pointData.AddRange(BitConverter.GetBytes(px));
            pointData.AddRange(BitConverter.GetBytes(py));
            pointData.AddRange(BitConverter.GetBytes(pz));
        }

        PointFieldMsg[] fields = new PointFieldMsg[]
        {
            new PointFieldMsg("x", 0, PointFieldMsg.FLOAT32, 1),
            new PointFieldMsg("y", 4, PointFieldMsg.FLOAT32, 1),
            new PointFieldMsg("z", 8, PointFieldMsg.FLOAT32, 1)
        };

        int pointStep = 12;
        int pointCount = pointData.Count / pointStep;

        PointCloud2Msg cloudMsg = new PointCloud2Msg
        (
            header: new HeaderMsg(
                stamp: new TimeMsg(),
                frame_id: "lidar_link"
            ),
            height: 1,
            width: (uint)pointCount,
            fields: fields,
            is_bigendian: false,
            point_step: (uint)pointStep,
            row_step: (uint)(pointStep * pointCount),
            data: pointData.ToArray(),
            is_dense: true
        );

        ros.Publish(pointCloudTopic, cloudMsg);
    }
}