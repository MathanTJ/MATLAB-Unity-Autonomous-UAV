using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;

public class CameraPublisher : MonoBehaviour
{
    public Camera cam;
    public string topicName = "/image_raw";
    public int width = 460;
    public int height = 300;
    public float publishMessageFrequency = 0.1f; //seconds - to frequent and system lags
    private float timeElapsed = 0f;

    ROSConnection ros;
    Texture2D tex;
    RenderTexture rt;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ImageMsg>(topicName);
        //ros.RegisterPublisher<Float32Msg>("/time"); //debugging

        rt = new RenderTexture(width, height, 24);
        cam.targetTexture = rt;

        tex = new Texture2D(width, height, TextureFormat.RGB24, false);
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        
        if (timeElapsed > publishMessageFrequency)
        {
            timeElapsed = 0;
            
            RenderTexture.active = rt;
            cam.Render(); //try disableing

            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            byte[] imageBytes = tex.GetRawTextureData();

            ImageMsg msg = new ImageMsg
            {
                height = (uint)height,
                width = (uint)width,
                encoding = "rgb8",
                is_bigendian = 0,
                step = (uint)(width * 3),
                data = imageBytes
            };

            ros.Publish(topicName, msg);

            RenderTexture.active = null;
        }
        
        /*Float32Msg msg1 = new Float32Msg(timeElapsed);
        ros.Publish("/time", msg1);*/ //for debugging
    }
}