using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.BuiltinInterfaces;
using System.Collections.Generic;
using System;

public class SimpleLidarPublisher : MonoBehaviour
{
    public string scanTopic = "/lidar_scan";
    public int numRays = 360;
    public float fovDeg = 360f;
    public float rangeMin = 0.05f;
    public float rangeMax = 10f;
    public float scanRateHz = 10f;
    public Vector3 lidarOffset = new Vector3(0.0f, 0.2f, 0.0f);

    ROSConnection ros;
    float nextPublishTime = 0f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<LaserScanMsg>(scanTopic);
    }

    void Update()
    {
        float now = Time.time;
        if (now < nextPublishTime)
            return;

        nextPublishTime = now + 1.0f / scanRateHz;
        PublishScan();
    }

    void PublishScan()
    {
        // Angles in base_link frame: 0 along +X, CCW positive
        float angleMin = -fovDeg * 0.5f * Mathf.Deg2Rad;
        float angleMax = fovDeg * 0.5f * Mathf.Deg2Rad;
        float angleInc = (angleMax - angleMin) / (numRays - 1);

        var ranges = new List<float>(numRays);

        // Unity pose
        Vector3 pos = transform.position;

        // Unity yaw (deg) -> rad
        float unityYawRad = transform.eulerAngles.y * Mathf.Deg2Rad;

        // IMPORTANT:
        // In your controller: Unity yaw = -theta (ROS yaw),
        // so ROS yaw = -Unity yaw.
        float rosYaw = -unityYawRad;   // <-- this is the yaw used by odom/TF

        // Lidar origin in world
        Vector3 origin = pos + lidarOffset;

        for (int i = 0; i < numRays; i++)
        {
            float angle = angleMin + i * angleInc;   // angle in base_link frame
            float globalAngle = rosYaw + angle;      // world angle in odom plane

            float dirX = Mathf.Cos(globalAngle);
            float dirZ = Mathf.Sin(globalAngle);
            Vector3 dirWorld = new Vector3(dirX, 0f, dirZ);

            if (Physics.Raycast(origin, dirWorld, out RaycastHit hit, rangeMax))
            {
                float dist = hit.distance;
                if (dist < rangeMin) dist = rangeMin;
                ranges.Add(dist);
            }
            else
            {
                ranges.Add(float.PositiveInfinity);
            }
        }

   
	
	// Use real wall-clock time (Unix epoch) for the scan stamp
	DateTimeOffset now = DateTimeOffset.UtcNow;
	long secsLong = now.ToUnixTimeSeconds();
	long nsecsLong = (now.ToUnixTimeMilliseconds() % 1000) * 1000000L;

	int secs = (int)secsLong;
	uint nsecs = (uint)nsecsLong;
	TimeMsg stamp = new TimeMsg(secs, nsecs);


	LaserScanMsg scan = new LaserScanMsg
	{
	    header = new RosMessageTypes.Std.HeaderMsg(stamp, "base_link"),
	    angle_min = angleMin,
	    angle_max = angleMax,
	    angle_increment = angleInc,
	    time_increment = 0.0f,
	    scan_time = 1.0f / scanRateHz,
	    range_min = rangeMin,
	    range_max = rangeMax,
	    ranges = ranges.ToArray(),
	    intensities = new float[0]
	};

        ros.Publish(scanTopic, scan);
    }
}