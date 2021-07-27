using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public MenuManager menuManager;
    public NetworkGameManager networkGameManager;
    public NetworkUIManager networkUIManager;
    public Board board;

    public int myColor = 0;
    #region Move_Net
    public Move recivedMove = new Move(-1, -1, -1);

    public Move sentMove = new Move(-1, -1, -1);
    public bool moveSent = false;
    #endregion
    #region FEN_Net


    public string sentFen = null;
    public bool sentIsWhite = false;
    public bool fenSent = false;
    #endregion


    //UI interface
    public int selectedMoveTo = -1;
    public int selectedPiece = -1;
    public bool blackForfit = false;
    public bool whiteForfit = false;
    public bool started = false;
    public bool isNetworked
    {
        get { return networkUIManager.connectedTCP && networkUIManager.connectedUDP; }
    }
    //
    public static GameManager instance;
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
    void Start()
    {
        uiManager = UIManager.instance;
        menuManager = MenuManager.instance;
        networkGameManager = NetworkGameManager.instance;
        networkUIManager = NetworkUIManager.instance;
        uiManager.gameText.text = "";
        uiManager.winText.text = "";

    }


    void Update()
    {

    }

    public Board createBoard()
    {
        return createBoard(FENUtills.DEFAULT_START_FEN); ;
    }

    public Board createBoard(string fen)
    {
        started = true;
        return new Board(fen);
    }

    public void selectPosition()
    {
        if (started)
        {
            if (!isNetworked)
            {

                if (selectedPiece == selectedMoveTo)
                    return;
                if (!board.tryMove(selectedPiece, selectedMoveTo, uiManager) && !board.isCheckMate())
                    Debug.Log("MOVE FAIL: { from = " + selectedPiece + ", to = " + selectedMoveTo + " }");
                string winMes;
                if ((winMes = isEndGameCondition()) != "")
                {
                    uiManager.winText.text = winMes;
                    started = false;
                }
            }
            else
            {
                if (board.ColorTurn == myColor)
                {
                    if (!moveSent)
                    {
                        if (selectedPiece == selectedMoveTo)
                            return;
                        Move move = board.getMove(selectedPiece, selectedMoveTo);
                        if (move.StartSquare == -1)
                        {
                            Debug.Log("NO such move found!");
                            return;
                        }
                        if (networkGameManager.sendMove(move))
                        {
                            sentMove = move;
                            Debug.Log("Move sent, waiting for response!");
                        }
                        else
                            Debug.Log("Move failed!");
                    }
                    else
                        Debug.Log("Move allready sent, waiting for response!");
                }
                else
                    Debug.Log("Not your Turn!");

            }
        }

    }

    public void selectPosition(int from, int to)
    {
        if (started)
        {
            if (from == to)
                return;
            if (!board.tryMove(from, to, uiManager) && !board.isCheckMate())
                Debug.Log("MOVE FAIL: { from = " + from + ", to = " + to + " }");
            if (board.isCheckMate())
                uiManager.winText.text = "Checkmate " + (board.whiteTurn ? "Black" : "White") + " Won! \n Press \"R\" to restart.";
        }
    }
    public void resetSentMove()
    {
        sentMove = new Move(-1, -1, -1);
        moveSent = false;
    }
    public void UpdateBoard()
    {
        if (networkGameManager.myTurn)
        {
            if (!board.tryMove(selectedPiece, selectedMoveTo, uiManager) && !board.isCheckMate())
                Debug.Log("MOVE FAIL: { from = " + selectedPiece + ", to = " + selectedMoveTo + " }");
            moveSent = false;
        }
        else
        {
            if (!board.tryMove(selectedPiece, selectedMoveTo, uiManager) && !board.isCheckMate())
                Debug.Log("MOVE FAIL: { from = " + selectedPiece + ", to = " + selectedMoveTo + " }");
        }
    }
    public bool ReciveMove(Move move)
    {
        if (board.containsMove(move))
        {
            recivedMove = move;
            GameManager.instance.UpdateBoard();
            return true;
        }
        else
            return false;
    }
    public bool ReciveFEN(string fen, bool isBlack)
    {
        //TODO CHECK FEN
        myColor = isBlack ? Piece.BLACK : Piece.WHITE;
        string fenc = fen;
        int[] valFenSections = FENUtills.validFen(fenc);
        if (valFenSections[0] >= 2)
        {
            string[] sections = fenc.Split(' ');
            string validFEN = "";
            for (int i = 0; i <= valFenSections[0]; i++)
                validFEN += (i != 0 ? " " : "") + sections[i];

            resetBoard(validFEN);
            MenuManager.instance.hideLobby();

        }
        return true;
    }
    public void UseSentFen()
    {
        if (fenSent)
        {
            myColor = !sentIsWhite ? Piece.BLACK : Piece.WHITE;
            string fen = sentFen;
            int[] valFenSections = FENUtills.validFen(fen);
            if (valFenSections[0] >= 2)
            {
                string[] sections = fen.Split(' ');
                string validFEN = "";
                for (int i = 0; i <= valFenSections[0]; i++)
                    validFEN += (i != 0 ? " " : "") + sections[i];

                resetBoard(validFEN);
                MenuManager.instance.hideLobby();

            }

        }
    }
    public void resetBoard()
    {
        board = createBoard();
        uiManager.generatePieceUI();
        string s = "Turn:" + (board.Turn + 1) + "\n" + "Color: " + (board.whiteTurn ? "White" : "Black") + "\n" + "Check: " + (board.Check ? (board.WhiteInCheck ? "White" : "Black") : "None");
        uiManager.gameText.text = s;
        uiManager.winText.text = "";
        board.onStart();

    }
    public void resetBoard(string fen)
    {
        board = createBoard(fen);
        uiManager.generatePieceUI();
        string s = "Turn:" + (board.Turn + 1) + "\n" + "Color: " + (board.whiteTurn ? "White" : "Black") + "\n" + "Check: " + (board.Check ? (board.WhiteInCheck ? "White" : "Black") : "None");
        uiManager.gameText.text = s;
        uiManager.winText.text = "";
        board.onStart();
    }


    public void forfit()
    {
        if (blackForfit)
        {
            uiManager.winText.text = "Black Forfit ! \n Press \"R\" to restart.";
            started = false;
        }
        else if (whiteForfit)
        {
            uiManager.winText.text = "White Forfit ! \n Press \"R\" to restart.";
            started = false;
        }
    }
    public string isEndGameCondition()
    {
        string mes = "";

        if (board.isCheckMate())
            mes = "Checkmate " + (board.whiteTurn ? "Black" : "White") + " Won! \n Press \"R\" to restart.";
        else if (board.isDraw())
            mes = "Draw! \n Press \"R\" to restart.";
        else if (blackForfit)
            mes = "Black Forfit ! \n Press \"R\" to restart.";
        else if (whiteForfit)
            mes = "White Forfit ! \n Press \"R\" to restart.";
        return mes;

    }

}
