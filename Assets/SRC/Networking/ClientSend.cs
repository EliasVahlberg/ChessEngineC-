using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        if (_packet != null)
        {
            _packet.WriteLength();
            Client.instance.tcp.SendData(_packet);
        }
        else
            Debug.Log("FAIL! NULL PACKET IN SendTCPData");
    }
    private static void SendUDPData(Packet _packet)
    {
        if (_packet != null)
        {
            _packet.WriteLength();
            Client.instance.udp.SendData(_packet);
        }
        else
            Debug.Log("FAIL! NULL PACKET IN SendTCPData");
    }
    #region TCP_PACKETS

    public static void WelcomeReccived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(NetworkUIManager.instance.userNameField.text);
            SendTCPData(_packet);
        }
    }
    public static void ChessMove(short _move)
    {
        using (Packet _packet = new Packet((int)ClientPackets.moveRequest))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(_move);
            SendTCPData(_packet);
        }
    }
    public static void ChessMoveResponse(bool _accept)
    {
        using (Packet _packet = new Packet((int)ClientPackets.moveResponse))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(_accept);
            SendTCPData(_packet);
        }
    }

    public static void FenSelect(string _fen, bool _isWhite)
    {
        using (Packet _packet = new Packet((int)ClientPackets.fenSelect))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(_fen);
            _packet.Write(_isWhite);
            SendTCPData(_packet);
        }
    }

    public static void FenSelectResponse(bool _accept)
    {
        using (Packet _packet = new Packet((int)ClientPackets.fenResponse))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(_accept);
            SendTCPData(_packet);
        }
    }
    #endregion
    #region UDP_PACKETS
    public static void UDPConnectReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.UDPConnectReceived))
        {
            _packet.Write(Client.instance.myId);
            string _msg = $"FROM {Client.instance.myId}: {{UDP CONNECTION RECEIVED}} ";
            _packet.Write(_msg);
            SendTCPData(_packet);
        }
    }
    #endregion
}
