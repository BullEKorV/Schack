using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using System.Text;

public class Client : MonoBehaviour
{
    EventBasedNetListener netListener;
    NetManager client;
    NetPacketProcessor netPacketProcessor;

    void Start()
    {
        netListener = new EventBasedNetListener();
        netPacketProcessor = new NetPacketProcessor();

        netListener.PeerConnectedEvent += (server) =>
        {
            Debug.LogError($"Connected to server: {server.EndPoint}");
        };

        netListener.NetworkReceiveEvent += (server, reader, deliveryMethod) =>
        {
            Debug.LogError("Recieved");
            // netPacketProcessor.ReadAllPackets(reader, server);
            Debug.LogError(server.ToString());

            Debug.LogError(reader.GetString());

            reader.Recycle();
        };

        client = new NetManager(netListener);
        client.Start();
        client.Connect("localhost", 9050, "leo");
    }

    void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            Debug.LogError("Sending");
            NetDataWriter writer = new NetDataWriter();
            writer.Put("cringe");
            client.ConnectedPeerList[0].Send(writer, DeliveryMethod.ReliableOrdered);
        }
        client.PollEvents();
    }
}