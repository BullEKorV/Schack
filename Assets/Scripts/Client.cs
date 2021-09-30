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
            netPacketProcessor.ReadAllPackets(reader, server);
        };

        netPacketProcessor.SubscribeReusable<FooPacket>((packet) =>
        {
            // Debug.Log("Got a foo packet!");
            // Debug.Log(packet.NumberValue);
        });

        client = new NetManager(netListener);
        client.Start();
        client.Connect("192.168.0.69", 9050, "leo");
    }

    void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            netPacketProcessor.Send(client.ConnectedPeerList[0], new FooPacket() { IpAdress = "ff", StringValue = "Test" }, DeliveryMethod.ReliableOrdered);
        }
        client.PollEvents();
    }
}
public class FooPacket
{
    public string IpAdress { get; set; }
    public string StringValue { get; set; }
}