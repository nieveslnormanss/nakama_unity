using System.Collections.Generic;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;
using System.Collections;

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

        var enc = System.Text.Encoding.UTF8;
        socket.ReceivedMatchState += newState =>
        {
            var content = enc.GetString(newState.State);

            switch (newState.OpCode)
            {
                case 1:
                    Debug.Log("A custom opcode.");
                    Debug.Log(content);
                    break;
                default:
                    Debug.Log("default");
                    break;
            }
        };

        await socket.ConnectAsync(session);

        string rpcId = "rpc_create_match";

        var response = await socket.RpcAsync(rpcId);

        Debug.Log("RPC Response: " + response.Payload);

        var match = await socket.JoinMatchAsync(response.Payload);

        Debug.Log(match);

        var newState = new Dictionary<string, string> {{"hello", "world"}}.ToJson();

        //await socket.SendMatchStateAsync(response.Payload,1,newState);

        StartCoroutine(ExecuteEverySecond(response.Payload,newState));

    }

    private IEnumerator ExecuteEverySecond(string matchID,string newstate)
    {
        while (true)
        {
            // 执行需要每秒执行的逻辑
            Debug.Log("Repeated execution every 1 sec");
            socket.SendMatchStateAsync(matchID,1,newstate);

            yield return new WaitForSeconds(1f);
        }
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
