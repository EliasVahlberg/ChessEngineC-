using System.Collections;
using System.Collections.Generic;
using System.Net;
using GameClient;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    #region TCP_PACKETS


    public static void Welcome(Packet _packet)
    {
        string msg = _packet.ReadString();
        int id = _packet.ReadInt();
        Debug.Log($"Message from server: \"{msg}\"");
        ConsoleHistory.instance.addLogHistory($"<color=blue>  Message from server:</color>\n <color=green>\"{msg}\"</color>");
        Client.instance.myId = id;
        ClientSend.WelcomeReccived();
        NetworkUIManager.instance.connectedTCP = true;
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        //TODO send packet back
    }
    public static void Move(Packet _packet)
    {
        int _id = _packet.ReadInt();
        short _move = _packet.ReadShort();
        Debug.Log($"Move: \n{_move}, From: {_id}");
        ConsoleHistory.instance.addLogHistory($"<color=blue>  Response to move from server:</color>\n <color=green>\"{_move}\"</color>");
        //* DON'T send back
    }
    public static void Fen(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _fen = _packet.ReadString();
        bool _isWhite = _packet.ReadBool();
        Debug.Log($"Fen: \n{_fen}, From: {_id}, IsWhite: {_isWhite}");
        //* DON'T send back
    }
    #endregion
    #region UDP_PACKETS
    public static void UDPConnect(Packet _packet)
    {
        string _msg = _packet.ReadString();
        Debug.Log(_msg);
        ClientSend.UDPConnectReceived();
        NetworkUIManager.instance.connectedUDP = true;
    }
    #endregion
}
