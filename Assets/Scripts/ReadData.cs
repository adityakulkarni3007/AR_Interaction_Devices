using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Threading;
using System;

public class ReadData : MonoBehaviour
{
    public PlayerData playerData;
    WebSocket socket;
    public GameObject player;
    private bool first_time = true;

    // Start is called before the first frame update
    void Start()
    {
        socket = new WebSocket("ws://10.0.0.169:8080");
        // socket = new WebSocket("ws://localhost:8080");
        socket.Connect();

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
                    // Debug.Log(JObject.Parse(e.Data));
                    Debug.Log(playerData.qx);
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


    void SendPacket()
    {
        if (socket == null)
        {
            return;
        }
         if (player != null)
        {
            //Grab player current position and rotation data
            playerData.qw = 0f;
            playerData.qx = 0f;
            playerData.qy = 0f;
            playerData.qz = 0f;
            playerData.button = 0f;
            playerData.shaking = 0f;
            playerData.joyx = 0f;
            playerData.joyy = 0f;
            playerData.id   = "0";

            System.DateTime epochStart =  new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
            double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
            Debug.Log(timestamp);
            playerData.timestamp = timestamp;

            string playerDataJSON = JsonUtility.ToJson(playerData);
            socket.Send(playerDataJSON);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (first_time && socket != null){
            Debug.Log("Entering");
            SendPacket();
            first_time = false;
        }
    }
}