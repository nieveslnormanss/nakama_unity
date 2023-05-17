using System.Collections.Generic;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;
using System;

[System.Serializable]
struct TestMatch{
     public readonly string OwnerId;
     public string Testinfo;
}

public class NakamaClient : MonoBehaviour
{
    public string Scheme = "http";
    public string Host = "192.168.71.167";
    public int Port = 7350;
    public string ServerKey = "defaultkey";
    public IClient client;
    private ISocket socket;
    //private const string RoomName = "heroes";
    async void Start()
    {
        try {

            client = new Client(Scheme,Host,Port,ServerKey);

            const string email = "cjj9931207@gmail.com";
            const string password = "ETpy@hJVQxY2mwC";
            var session = await client.AuthenticateEmailAsync(email,password);

            Debug.Log(session);

            socket = client.NewSocket();

            socket.Connected +=() => Debug.Log("Socket connected.");

            socket.Closed +=() => Debug.Log("Socket closed");

            await socket.ConnectAsync(session);

            await socket.AddMatchmakerAsync("*",2,2);

            socket.ReceivedMatchmakerMatched += async matched =>
            {
                Debug.LogFormat("Match: {0}",matched);
                var match = await socket.JoinMatchAsync(matched);

                var self = match.Self;
                Debug.LogFormat("Self: {0}",self);
                Debug.Log(match.Presences);
                TestMatch message = new TestMatch();
                message.Testinfo = "test";
                string json = JsonWriter.ToJson(message);
                await socket.SendMatchStateAsync(match.Id, 11, json);
            };
        } catch (Exception e)
        {
            // 在这里处理异常，例如打印错误信息
            Console.WriteLine($"Error sending match state: {e.Message}");
        }

        //string rpcId = "healthcheck";

        //var response = await socket.RpcAsync(rpcId);

        //Debug.Log("RPC Response: " + response.Payload);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        socket?.CloseAsync();
    }
}
