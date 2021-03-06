using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    @File NetworkGameManager.cs
    @author Elias Vahlberg 
    @Date 2021-07
    @Credit Tom Weiland
*/
public class NetworkGameManager : MonoBehaviour
{
    public static NetworkGameManager instance;
    public bool myTurn { get { return (NetworkUIManager.instance.IsConnected() && GameManager.instance.myColor == GameManager.instance.board.ColorTurn); } }
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
    #region Move

    public void onRecieveMove(Move move)
    {
        bool accepted = GameManager.instance.ReciveMove(move);
        NetworkUIManager.instance.SendMoveResponse(accepted);
        Debug.Log("Sending Move response: ACCEPTED = " + accepted);
    }

    public bool sendMove(Move move)
    {
        NetworkUIManager.instance.SendMove(move);
        GameManager.instance.moveSent = true;
        return true;
    }

    public void sentMoveResponse(bool accepted)
    {
        if (accepted)
            GameManager.instance.UpdateBoard();
        else
            GameManager.instance.resetSentMove();
        GameManager.instance.moveSent = false;
    }
    #endregion
    #region FEN


    public void onRecieveFEN(string fen, bool isBlack)
    {
        bool accepted = GameManager.instance.ReciveFEN(fen, isBlack);
        NetworkUIManager.instance.SendFenResponse(accepted);
    }

    public bool sendFEN(string fen, bool isWhite)
    {
        int[] fenValidation = FENUtills.validFen(fen);
        if (fenValidation[0] > 3)
        {
            NetworkUIManager.instance.SendFen(fen, isWhite);
            GameManager.instance.sentFen = fen;
            GameManager.instance.sentIsWhite = isWhite;
            GameManager.instance.fenSent = true;

            return true;
        }

        return false;
    }

    public bool sendStandardFEN(bool isWhite)
    {
        string fen = FENUtills.DEFAULT_START_FEN;
        GameManager.instance.sentFen = fen;
        GameManager.instance.sentIsWhite = isWhite;
        NetworkUIManager.instance.SendFen(fen, isWhite);
        GameManager.instance.fenSent = true;

        return true;
    }

    public void sentFENResponse(bool accepted)
    {
        if (accepted)
        {
            Debug.Log("ACCEPTED, Game starting");
            GameManager.instance.resetBoard(GameManager.instance.sentFen);
            GameManager.instance.myColor = GameManager.instance.sentIsWhite ? Piece.WHITE : Piece.BLACK;
            MenuManager.instance.hideLobby();
            GameManager.instance.sentFen = null;
            GameManager.instance.sentIsWhite = false;
        }
        else
        {
            Debug.Log("DECLINED, Game not starting");
            GameManager.instance.sentFen = null;
            GameManager.instance.sentIsWhite = false;
        }
        GameManager.instance.fenSent = true;
    }

    #endregion

}
