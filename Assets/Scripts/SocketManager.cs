using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System.IO.Ports;
using Newtonsoft.Json.Linq;

public class SocketManager : MonoBehaviour
{
    WebSocket socket;
    public GameObject player;
    public PlayerData playerData;
    public string[] strData = new string[5];
    public string[] strData_received = new string[5];
    public string strReceived;
    SerialPort stream = new SerialPort("COM8", 9600);


    //Package URL for Newtonsoft JSON utilities
    string PackageURL = "https://github.com/jilleJr/Newtonsoft.Json-for-Unity.git#upm";

    // Start is called before the first frame update
    void Start()
    {
        stream.Open(); //Open the Serial Stream.
        // socket = new WebSocket("ws://localhost:8080");
        socket = new WebSocket("ws://10.191.108.61:8080");
        socket.Connect();

        //WebSocket onMessage function
        socket.OnMessage += (sender, e) =>
        {

            //If received data is type text...
            if (e.IsText)
            {
                // Debug.Log("IsText");
                // Debug.Log(e.Data);
                JObject jsonObj = JObject.Parse(e.Data);

                //Get Initial Data server ID data (From intial serverhandshake
                if (jsonObj["id"] != null)
                {
                    //Convert Intial player data Json (from server) to Player data object
                    PlayerData tempPlayerData = JsonUtility.FromJson<PlayerData>(e.Data);
                    playerData = tempPlayerData;
                    // Debug.Log("player ID is " + playerData.id);
                    return;
                }

            }

        };

        //If server connection closes (not client originated)
        socket.OnClose += (sender, e) =>
        {
            Debug.Log(e.Code);
            Debug.Log(e.Reason);
            Debug.Log("Connection Closed!");
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (socket == null)
        {
            return;
        }

        try
        {
            strReceived = stream.ReadLine(); //Read the information  
            //Debug.Log(strReceived);
            strData = strReceived.Split(',');
            if (strData.Length < 8)
            {
                return;
            }
            for (int i = 0; i < strData.Length; i++)
            {
                if (strData[i] == "")
                {
                    return;
                }
            }
        }
        catch
        {
            Debug.Log("Could not read sensor Data");
        }

        //If player is correctly configured, begin sending player data to server
        if (player != null)
        {
            //Grab player current position and rotation data
            try
            {
                playerData.qw = float.Parse(strData[0]);
                playerData.qx = float.Parse(strData[1]);
                playerData.qy = float.Parse(strData[2]);
                playerData.qz = float.Parse(strData[3]);
                playerData.button = float.Parse(strData[4]);
                playerData.shaking = float.Parse(strData[5]);
                playerData.joyx = float.Parse(strData[6]);
                playerData.joyy = float.Parse(strData[7]);
                playerData.id = "1";
            }
            catch
            {
                Debug.Log("Could not convert to float");
            }
            System.DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
            double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
            //Debug.Log(timestamp);
            playerData.timestamp = timestamp;

            string playerDataJSON = JsonUtility.ToJson(playerData);
            socket.Send(playerDataJSON);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            string messageJSON = "{\"message\": \"Some Message From Client\"}";
            socket.Send(messageJSON);
        }
    }

    private void OnDestroy()
    {
        //Close socket when exiting application
        socket.Close();
    }

}