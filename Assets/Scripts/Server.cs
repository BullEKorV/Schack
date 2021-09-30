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
            if (server.ConnectedPeersCount < 2 /* max connections */)
                request.AcceptIfKey("schack");
            else
                request.Reject();
        };

        netListener.NetworkReceiveEvent += (client, reader, deliveryMethod) =>
        {
            Debug.LogError("Server Recieved");
            // netProcessor.ReadAllPackets(reader, server);

            foreach (var peer in server.ConnectedPeerList)
            {
                if (peer != client)
                {
                    peer.Send(reader.GetRemainingBytes(), DeliveryMethod.ReliableOrdered);
                }
            }
            reader.Recycle();
        };

        netListener.PeerConnectedEvent += (client) =>
        {
            Debug.LogError($"[Server] Peer connected: {client.EndPoint}");
        };

        netListener.PeerDisconnectedEvent += (client, DisconnectInfo) =>
        {
            Debug.LogError($"[Server] Peer disconnected: {client.EndPoint}, reason: {DisconnectInfo.Reason}");
        };
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            foreach (NetPeer peer in server.ConnectedPeerList)
            {
                // netProcessor.Send(peer, new FooPacket() { NumberValue = 1, StringValue = "Test" }, DeliveryMethod.ReliableOrdered);
            }
        }
        server.PollEvents();
    }
}
