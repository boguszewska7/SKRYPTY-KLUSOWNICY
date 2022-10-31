using System.Collections;
using WebSocketSharp;
//using System.Collections.Generic;
using System;
using UnityEngine;
using System.Globalization;
using System.Text;
using UnityEngine.UI;


public class rotation : MonoBehaviour
{
    public GameObject WSclient;
    Vector3 controllerRotation;
    public Vector3 firstRotation;
    Vector3 waitRotation;
    float firstAccx;
    private WSClient ws;
    bool first_data= true;
    // Start is called before the first frame update
    void Start()
    {
        ws = WSclient.GetComponent<WSClient>();
    }

    // Update is called once per frame
    void Update()
    {
        Rotation();
        //transform.rotation = new Quaternion(ws.quats[1], ws.quats[2], ws.quats[3], ws.quats[0]);
    }


    Vector3 calculateRotation(float[] quat)
    {
        Vector3 output;
        // roll (x-axis rotation)
        double sinr_cosp = 2 * (quat[0] * quat[1] + quat[2] * quat[3]);
        double cosr_cosp = 1 - 2 * (quat[1] * quat[1] + quat[2] * quat[2]);
        output.z = (float)Math.Atan2(sinr_cosp, cosr_cosp);
        // pitch (y-axis rotation)
        float sinp = 2 * (quat[0] * quat[2] - quat[3] * quat[1]);
        if (Math.Abs(sinp) >= 1)
            output.x = MathF.Sign((float)sinp) * (float)Math.PI / 2.0f;
        else
            output.x = (float)Math.Asin(sinp);
        // yaw (z-axis rotation)
        double siny_cosp = 2 * (quat[0] * quat[3] + quat[1] * quat[2]);
        double cosy_cosp = 1 - 2 * (quat[2] * quat[2] + quat[3] * quat[3]);
        output.y = (float)Math.Atan2(siny_cosp, cosy_cosp);
        output.x = output.x * 180 / (float)Math.PI;
        output.y = -output.y * 180 / (float)Math.PI;
        output.z = -output.z * 180 / (float)Math.PI;
        return output;

    }
    void Rotation()
    {
        if (first_data)
        {
            firstRotation = calculateRotation(ws.quats);
           
                first_data = false;
               
        }
        else
        {

            controllerRotation = calculateRotation(ws.quats);
            controllerRotation.x -= firstRotation.x;
            controllerRotation.y -= firstRotation.y;
            controllerRotation.z -= firstRotation.z;


            /* if (Math.Abs(prev - controllerRotation.y) < 1.5f)
             {
                 controllerRotation.y = prev;
             }*/

           
           Vector3 oneAxis = new Vector3(-controllerRotation.x, controllerRotation.y, -controllerRotation.z);

           
            transform.eulerAngles = controllerRotation;
        }

    }
}
