using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGameManager : MonoBehaviour
{
    public static NetworkGameManager instance;
    public bool myTurn = false;
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
    }

    public bool sendMove(Move move)
    {
        NetworkUIManager.instance.SendMove(move);
        return true;
    }

    public void sentMoveResponse(bool accepted)
    {
        if (accepted)
            GameManager.instance.UpdateBoard();
        else
            GameManager.instance.resetSentMove();
    }
    #endregion
    #region FEN


    public void onRecieveFEN(string fen, bool isBlack)
    {
        bool accepted = GameManager.instance.ReciveFEN(fen, isBlack);
        NetworkUIManager.instance.SendFenResponse(accepted);
        if (accepted)
        {

        }
    }

    public bool sendFEN(string fen, bool isWhite)
    {
        int[] fenValidation = FENUtills.validFen(fen);
        if (fenValidation[0] > 3)
        {
            NetworkUIManager.instance.SendFen(fen, isWhite);
            GameManager.instance.sentFen = fen;
            GameManager.instance.sentIsWhite = isWhite;
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

        return true;
    }

    public void sentFENResponse(bool accepted)
    {
        if (accepted)
        {
            GameManager.instance.createBoard(GameManager.instance.sentFen);
            GameManager.instance.myColor = GameManager.instance.sentIsWhite ? Piece.WHITE : Piece.BLACK;
            GameManager.instance.myColor = GameManager.instance.sentIsWhite ? Piece.WHITE : Piece.BLACK;
        }
        else
        {
            GameManager.instance.sentFen = null;
            GameManager.instance.sentIsWhite = false;
        }
    }
    #endregion

}
