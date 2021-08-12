using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    @File NetworkUIManager.cs
    @author Elias Vahlberg 
    @Date 2021-07
*/
public class NetworkUIManager : MonoBehaviour
{
    public static NetworkUIManager instance;
    public GameObject startMenu;
    public InputField userNameField;
    public InputField ipField;
    public bool connectedTCP = false;
    public bool connectedUDP = false;
    public bool IsConnected() => connectedTCP || connectedUDP;
    // * SINGELTON IMPLEMENTATION
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("SAMEINSTACE ");
            Destroy(this);
        }
    }

    public void ConnectToServer()
    {
        Client.instance.ConnectToServer();

    }

    public void ConnectToServerNewIp()
    {
        ConnectToServer(ipField.text);
    }

    public void ConnectToServer(string ip)
    {

        Client.instance.ConnectToServer(ip);
        //MenuManager.instance.hideNetworkMenu(false);
    }
    public void onConnect()
    {
        startMenu.SetActive(false);
        userNameField.interactable = false;
        ipField.interactable = false;
        MenuManager.instance.showLobby();
    }
    public void onDisconnect()
    {
        MenuManager.instance.showNetworkMenu();
    }
    public void Disconnect()
    {
        Client.instance.Disconnect();
    }
    #region Move
    public void SendMove(Move move)
    {
        ClientSend.ChessMove(move.MoveValue);
    }
    public void ReciveMove(ushort move)
    {
        NetworkGameManager.instance.onRecieveMove(new Move(move));
    }

    public void SendMoveResponse(bool accepted)
    {
        ClientSend.ChessMoveResponse(accepted);
    }

    public void ReciveMoveResponse(bool accepted)
    {
        NetworkGameManager.instance.sentMoveResponse(accepted);
    }

    #endregion

    #region FEN
    public void SendFen(string Fen, bool isWhite)
    {
        ClientSend.FenSelect(Fen, isWhite);
    }
    public void ReciveFen(string Fen, bool isBlack)
    {
        NetworkGameManager.instance.onRecieveFEN(Fen, isBlack);
    }

    public void SendFenResponse(bool accepted)
    {
        ClientSend.FenSelectResponse(accepted);
    }
    public void ReciveFenResponse(bool accepted)
    {
        NetworkGameManager.instance.sentFENResponse(accepted);
    }
    #endregion

}
