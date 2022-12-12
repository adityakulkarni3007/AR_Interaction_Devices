using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using static System.Math;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
// TODO: Remove me
public class mpu_2 : MonoBehaviour
{
    
//    SerialPort stream = new SerialPort("COM7", 115200);
   SerialPort stream = new SerialPort("COM8", 9600);
    public string strReceived;
     
    public string[] strData = new string[5];
    public string[] strData_received = new string[5];
    public float x,y,z,qw, qx, qy, qz, alpha, joyy, joyx,button, shaking;
    private int count;
    private float prev_mode, yz_thresh, x_thresh, joy_thresh, mode_thres;
    public GameObject pos_cube;
    public int mode, focus, joy_count;
    private Vector3 scale;
    //private Dictionary <int, GameObject> body;
    private Color focus_color, default_color;
    //public GameObject arteries, lungs, airways, kidneys, liver, skeleton, skin, intestines;
    public TextMeshProUGUI movement, opacity, scaling;
    public TextMeshProUGUI[] text_objs = new TextMeshProUGUI[3];
    private GameObject[] body = new GameObject[8];
    public TextMeshProUGUI[] body_objs = new TextMeshProUGUI[8];
    private GameObject body_parts;
    void Start()
    {
        stream.Open(); //Open the Serial Stream.
        scale           = transform.localScale;
        alpha           = 1.0f;
        count           = 0;
        mode            = 1;
        focus           = 0;
        joy_count       = 0;
        prev_mode       = 1.0f;
        yz_thresh       = 30f;
        x_thresh        = 20f;
        joy_thresh      = 2f;
        mode_thres      = 3f;
        default_color   = new Color(0.67f, 0.46f, 0.44f);
        focus_color     = new Color(0.19f, 0.32f, 0.6f);
        
        //TODO: Try to make FindTag work
        body[7] = GameObject.FindWithTag("airways");
        body[6] = GameObject.FindWithTag("arteries");
        body[5] = GameObject.FindWithTag("intestines");
        body[4] = GameObject.FindWithTag("kidneys");
        body[3] = GameObject.FindWithTag("liver");
        body[2] = GameObject.FindWithTag("lungs");
        body[1] = GameObject.FindWithTag("skeleton");
        body[0] = GameObject.FindWithTag("skin");

        body_parts = GameObject.FindWithTag("BodyParts");
        body_parts.SetActive(false);
        text_objs[0] = movement;
        text_objs[1] = opacity;
        text_objs[2] = scaling;

        HighlightText(3,mode);
        HighlightText2(8,focus+1);
    }

    void HighlightText(int n, int curr_mode)
    {
        for (int i = 0; i < n; i++)
        {
            if (curr_mode == i + 1)
            {
                Color curr = text_objs[i].color;
                curr.a = 1.0f;
                text_objs[i].color = curr;
            } else
            {
                Color curr = text_objs[i].color;
                curr.a = 0.5f;
                text_objs[i].color = curr;
            }
        }
    }
    
    void HighlightText2(int n, int curr_mode)
    {
        for (int i = 0; i < n; i++)
        {
            if (curr_mode == i + 1)
            {
                Color curr = body_objs[i].color;
                curr.a = 1.0f;
                body_objs[i].color = curr;
            } else
            {
                Color curr = body_objs[i].color;
                curr.a = 0.5f;
                body_objs[i].color = curr;
            }
        }
    }
    
    void ModeSwitch()
    {
        if(button==1f){return ;}
        if (((x > -x_thresh && x < x_thresh)  && (y < -180f + yz_thresh || y > 180f - yz_thresh) && (z > -yz_thresh || z < yz_thresh)) || ((x < -180f + x_thresh || x > 180f - x_thresh) && (y > -yz_thresh && y < yz_thresh) && (z < -180f + yz_thresh || z > 180f - yz_thresh))){
            if(prev_mode!=1){count=0;prev_mode = 1;} else{count++;}
            if(count >= mode_thres){
                mode = 1;
                HighlightText(3,mode);
            }
            Debug.Log("Mode is 1");
        }
        else if (((x > 90.0f - x_thresh && x < 90.0f + x_thresh) && (y < -180f + yz_thresh || y > 180f - yz_thresh) && (z > -yz_thresh || z < yz_thresh)) || ((x > 90.0f - x_thresh && x < 90.0f + x_thresh) && (y > -yz_thresh && y < yz_thresh) && (z < -180f + yz_thresh || z > 180f - yz_thresh))){
            if(prev_mode!=2){count=0;prev_mode = 2;} else{count++;}
            if (count >= mode_thres) {
                mode = 2;
                HighlightText(3,mode);
                
            }
            Debug.Log("Mode is 2");
        }
        else if (((x < -90.0f + x_thresh && x > -90.0f - x_thresh) && (y < -180f + yz_thresh || y > 180f - yz_thresh) && (z > -yz_thresh || z < yz_thresh)) || ((x < -90.0f + x_thresh && x > -90.0f - x_thresh) && (y > -yz_thresh && y < yz_thresh) && (z < -180f + yz_thresh || z > 180f - yz_thresh))){
            if(prev_mode!=3){count=0;prev_mode = 3;} else{count++;}
            if (count >= mode_thres) {
                mode = 3;
                HighlightText(3,mode);
            }
            Debug.Log("Mode is 3");
        }
        else if (((x < -180f + x_thresh || x > 180f - x_thresh) && (y < -180f + yz_thresh || y > 180f - yz_thresh) && (z > -yz_thresh || z < yz_thresh)) || ((x > -x_thresh && x < x_thresh) && (y > -yz_thresh && y < yz_thresh) && (z < -180f + yz_thresh || z > 180f - yz_thresh))){
            if(prev_mode!=4){count=0;prev_mode = 4;} else{count++;}
            if(count >= mode_thres){
                mode = 4;
                HighlightText(3,mode);
            }
            Debug.Log("Mode is 4");
        }
        return ;
    }

    void translation()
    {
            float delta_x = (joyx - 512.0f) * .05f / 1024.0f;
            float delta_y = (joyy - 512.0f) * .05f / 1024.0f;

            transform.position = transform.position + new Vector3(delta_y, delta_x, 0);
    }

    void rotation()
    {
        Quaternion rot = new Quaternion(-qy, -qz, qx, qw);
        // Quaternion spin=Quaternion.Euler(-y, x,-z);
        Quaternion spin=Quaternion.Euler(0, 180,0);
        transform.rotation = spin*rot;
    }

    void focus_region()
    {
        if (joyx>1000)
        {
            joy_count++;
            if(joy_count>joy_thresh)
            {
                body[focus].GetComponent<Renderer>().material.SetColor("_Color", default_color);
                focus++;
                Debug.Log(focus);
                if (focus > 7){focus=0;}
                joy_count=0;
            }
        }
        else if(joyx < 10)
        {
            joy_count++;
            if(joy_count>joy_thresh)
            {
                body[focus].GetComponent<Renderer>().material.SetColor("_Color", default_color);
                focus--;
                Debug.Log(focus);
                if (focus < 0){focus=7;}
                joy_count=0;
            }   
        }
        else
        {
            joy_count=0;
        }
        body[focus].GetComponent<Renderer>().material.SetColor("_Color", focus_color);
        body[focus].GetComponent<Renderer>().material.SetFloat("_alpha", Abs(x/90));
        HighlightText2(8,focus+1);
    }

    // Update is called once per frame
    void Update()
    {
        try{
            strReceived = stream.ReadLine(); //Read the information  
            Debug.Log(strReceived);
            strData = strReceived.Split(','); 
            if (strData.Length<8){
                return ;
            }
            for (int i=0;i<strData.Length;i++){
                if (strData[i] == ""){
                    return ;
                }
            }
        }
        catch{
            Debug.Log("Could not read sensor Data");
        }

        try{
            qw      = float.Parse(strData[0]);
            qx      = float.Parse(strData[1]);
            qy      = float.Parse(strData[2]);
            qz      = float.Parse(strData[3]); 
            button  = float.Parse(strData[4]);
            shaking = float.Parse(strData[5]);
            joyx    = float.Parse(strData[6]);
            joyy    = float.Parse(strData[7]);
            Quaternion rot = new Quaternion(-qy, -qz, qx, qw);
            Quaternion spin=Quaternion.Euler(0, 180,0);
            pos_cube.transform.rotation = spin*rot;
            Vector3 eul = pos_cube.transform.localEulerAngles;
            if (eul[0]<180){x=eul[0];}
            else{x = (eul[0] - 360);}
            if (eul[1]<180){y=eul[1];}
            else{y = (eul[1] - 360);}
            if (eul[2]<180){z=eul[2];}
            else{z = (eul[2] - 360);}

            ModeSwitch();
            if(mode == 1 && button!=0f){
                rotation();
                translation();
            }    
            if (mode == 2 && button !=0f){
                focus_region();
                body_parts.SetActive(true);
            }
            else{
                body[focus].GetComponent<Renderer>().material.SetColor("_Color", default_color);
                body_parts.SetActive(false);
            }
            if (mode == 3 && button != 0f){
                transform.localScale = new Vector3 (scale[0]*Abs(x)/90, scale[1]*Abs(x)/90, scale[2]*Abs(x)/90);
            }
            // else{transform.localScale = new Vector3(scale[0],scale[1],scale[2]);}
            
        }  
        catch{
            Debug.Log("Could not convert to float");
        }


    }
}