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

        socket.ReceivedChannelMessage += message =>
        {
            Debug.LogFormat("Rcceived: {0}",message);
        };
    
        await socket.ConnectAsync(session);

        var channel = await socket.JoinChatAsync(RoomName,ChannelType.Room);

        Debug.LogFormat("Join chat channel: {0}",channel);

        var content = new Dictionary<string,string>{{"hello","world"}}.ToJson();

        _ = socket.WriteChatMessageAsync(channel,content);
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
