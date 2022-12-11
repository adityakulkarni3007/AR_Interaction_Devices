using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class mpu6050 : MonoBehaviour
{
    
   SerialPort stream = new SerialPort("COM8", 115200);
    public string strReceived;
     
    public string[] strData = new string[5];
    public string[] strData_received = new string[5];
    public float qw, qx, qy, qz;
    public int mode=1;
    public float button;
    void Start()
    {
        stream.Open(); //Open the Serial Stream.
    }

    // Update is called once per frame
    void Update()
    {
        try{
            strReceived = stream.ReadLine(); //Read the information  
        }
        catch{
            Debug.Log("Could not read sensor Data");
        }

        Debug.Log(strReceived);
        strData = strReceived.Split(','); 
        Debug.Log(strData.Length);
        if (strData[0] != "" && strData[1] != "" && strData[2] != "")
        {
            qw      = float.Parse(strData[0]);
            qx      = -float.Parse(strData[1]);
            qy      = float.Parse(strData[2]);
            if (qw > -10.0f && qw < 10.0f){
                mode = 1;
                Debug.Log("Mode is 1");
            }
            else if (qw > 80.0f && qw < 100.0f){
                mode = 2;
                Debug.Log("Mode is 2");
            }
            else if (qw < -80.0f && qw > -100.0f){
                mode = 3;
                Debug.Log("Mode is 3");
            }
            else if (qw < -170.0f && qw < 180.0f){
                mode = 4;
                Debug.Log("Mode is 4");
            }
            // qz      = float.Parse(strData[3]);
            // button  = float.Parse(strData[4]);
            // Debug.Log(new string(strData[0] + strData[1] + strData[2]));
      
            transform.rotation = Quaternion.Euler(qx,qw,qy);
            // transform.rotation = new Quaternion(-qy, -qz, qx, qw);
        }      
    }
}