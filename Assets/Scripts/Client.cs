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
    NetPacketProcessor netProcessor;

    void Start()
    {
        netListener = new EventBasedNetListener();
        netProcessor = new NetPacketProcessor();

        netListener.PeerConnectedEvent += (server) =>
        {
            Debug.LogError($"Connected to server: {server.EndPoint}");
        };

        netListener.NetworkReceiveEvent += (server, reader, deliveryMethod) =>
        {
            netProcessor.ReadAllPackets(reader, server);
        };

        netProcessor.SubscribeReusable<FooPacket>((packet) =>
        {
            Debug.Log("Got a foo packet!");
            Debug.Log(packet.NumberValue);
        });

        client = new NetManager(netListener);
        client.Start();
        client.Connect("localhost", 9050, "leo");
    }

    void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            foreach (NetPeer peer in client.ConnectedPeerList)
            {
                netProcessor.Send(peer, new FooPacket() { NumberValue = 1, StringValue = "Test" }, DeliveryMethod.ReliableOrdered);
            }
        }
        client.PollEvents();
    }
}
public class FooPacket
{
    public int NumberValue { get; set; }
    public string StringValue { get; set; }
}