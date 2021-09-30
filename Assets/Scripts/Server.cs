using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class Server : MonoBehaviour
{
    EventBasedNetListener netListener;
    NetManager server;
    NetPacketProcessor netProcessor;

    // Start is called before the first frame update
    void Start()
    {
        netListener = new EventBasedNetListener();
        server = new NetManager(netListener);
        netProcessor = new NetPacketProcessor();

        server.Start(9050);

        netListener.ConnectionRequestEvent += request =>
        {
            if (server.ConnectedPeersCount < 10 /* max connections */)
                request.AcceptIfKey("leo");
            else
                request.Reject();
        };

        netListener.PeerConnectedEvent += (client) =>
        {
            Debug.LogError($"Client connected: {client.EndPoint}");
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            foreach (NetPeer peer in server.ConnectedPeerList)
            {
                netProcessor.Send(peer, new FooPacket() { NumberValue = 1, StringValue = "Test" }, DeliveryMethod.ReliableOrdered);
            }
        }
        server.PollEvents();
    }
}
