using System.Collections;
using System.Collections.Generic;
using System.Net;
using GameClient;
using UnityEngine;

/*
    @File ClientHandle.cs
    @author Elias Vahlberg 
    @Date 2021-07
    @Credit Tom Weiland
*/
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
        NetworkUIManager.instance.onConnect();
        NetworkUIManager.instance.connectedUDP = true;  //TODO REMOVE

        //TODO send packet back
    }

    public static void Move(Packet _packet)
    {
        int _id = _packet.ReadInt();
        ushort _move = _packet.ReadUShort();
        Debug.Log($"Move: \n{_move}, From: {_id}");
        ConsoleHistory.instance.addLogHistory($"<color=blue>  Move from server:</color>\n <color=green>\"{_move}\"</color>");
        NetworkUIManager.instance.ReciveMove(_move);
        //* DON'T send back
    }

    public static void MoveResponse(Packet _packet)
    {
        int _id = _packet.ReadInt();
        bool _accept = _packet.ReadBool();
        Debug.Log($"MoveResponse: ACCEPT = \n{_accept}, From: {_id}");
        ConsoleHistory.instance.addLogHistory($"<color=blue>  Response to move from server:</color>\n <color=green>\"{_accept}\"</color>");
        NetworkUIManager.instance.ReciveMoveResponse(_accept);
        //* DON'T send back
    }

    public static void Fen(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _fen = _packet.ReadString();
        bool _isWhite = _packet.ReadBool();
        Debug.Log($"Fen: \n{_fen}, From: {_id}, IsWhite: {_isWhite}");
        NetworkUIManager.instance.ReciveFen(_fen, _isWhite);
        //* DON'T send back
    }

    public static void FenResponse(Packet _packet)
    {
        int _id = _packet.ReadInt();
        bool _accept = _packet.ReadBool();
        Debug.Log($"FenResponse: ACCEPT = \n{_accept}, From: {_id}");
        ConsoleHistory.instance.addLogHistory($"<color=blue>  Response to fen from server:</color>\n <color=green>\"{_accept}\"</color>");
        NetworkUIManager.instance.ReciveFenResponse(_accept);
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
