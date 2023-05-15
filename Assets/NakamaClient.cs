using System.Collections.Generic;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;

public class NakamaClient : MonoBehaviour
{
    
    private const string RoomName = "heroes";
    private readonly IClient client = new Client("http","192.168.71.167",7350,"defaultkey");

    private ISocket socket;
    async void Start()
    {
        const string email = "cjj9931207@gmail.com";
        const string password = "ETpy@hJVQxY2mwC";
        var session = await client.AuthenticateEmailAsync(email,password);
        Debug.Log(session);

        socket = client.NewSocket();

        socket.Connected +=() => Debug.Log("Socket connected.");

        socket.Closed +=() => Debug.Log("Socket closed");

        await socket.ConnectAsync(session);

        string rpcId = "healthcheck";

        var response = await socket.RpcAsync(rpcId);

        Debug.Log("RPC Response: " + response.Payload);
        
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
