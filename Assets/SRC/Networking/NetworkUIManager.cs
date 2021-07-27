using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUIManager : MonoBehaviour
{
    public static NetworkUIManager instance;
    public GameObject startMenu;
    public InputField userNameField;
    public InputField ipField;
    public bool connectedTCP = false;
    public bool connectedUDP = false;
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
        startMenu.SetActive(false);
        userNameField.interactable = false;
        ipField.interactable = false;
        Client.instance.ConnectToServer();

    }

    public void ConnectToServerNewIp()
    {
        ConnectToServer(ipField.text);
    }

    public void ConnectToServer(string ip)
    {
        startMenu.SetActive(false);
        userNameField.interactable = false;
        ipField.interactable = false;
        Client.instance.ConnectToServer(ip);
        MenuManager.instance.hideNetworkMenu(false);
    }
    public void onConnect()
    {
        MenuManager.instance.showLobby();
    }
    public void onDisconnect()
    {
        MenuManager.instance.showNetworkMenu();
    }

    #region Move
    public void SendMove(Move move)
    {
        ClientSend.ChessMove(move.toShort());
    }
    public void ReciveMove(short move)
    {
        NetworkGameManager.instance.onRecieveMove(new Move(((ushort)move)));
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
