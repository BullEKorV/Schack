using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

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

            Debug.LogError(reader.GetString());

            reader.Recycle();
        };

        client = new NetManager(netListener);
        client.Start();
        client.Connect("localhost", 9050, "schack");
    }

    void Update()
    {
        if (Input.GetKeyDown("j"))
        {
            client.Start();
            client.Connect("localhost", 9050, "schack");
        }
        if (Input.GetKeyDown("k"))
        {
            client.Stop();
            Debug.LogError($"Disconnected from server");
        }
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