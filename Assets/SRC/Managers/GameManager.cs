using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public MenuManager menuManager;
    public NetworkManager networkManager;
    public Board board;

    //UI interface
    public int selectedMoveTo = -1;
    public int selectedPiece = -1;
    public bool blackForfit = false;
    public bool whiteForfit = false;
    public bool started = false;
    //
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        menuManager = FindObjectOfType<MenuManager>();
        networkManager = FindObjectOfType<NetworkManager>();
        uiManager.gameManager = this;
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
        if (selectedPiece == selectedMoveTo)
            return;
        if (!board.tryMove(selectedPiece, selectedMoveTo, uiManager) && !board.isCheckMate())
            Debug.Log("MOVE FAIL: { from = " + selectedPiece + ", to = " + selectedMoveTo + " }");
        if (board.isCheckMate())
            uiManager.winText.text = "Checkmate " + (board.whiteTurn ? "Black" : "White") + " Won! \n Press \"R\" to restart.";
        if (board.isDraw())
            uiManager.winText.text = "Draw! \n Press \"R\" to restart.";
        if (blackForfit)
            uiManager.winText.text = "Black Forfit ! \n Press \"R\" to restart.";
        if (whiteForfit)
            uiManager.winText.text = "White Forfit ! \n Press \"R\" to restart.";
    }
    public void selectPosition(int from, int to)
    {
        if (from == to)
            return;
        if (!board.tryMove(from, to, uiManager) && !board.isCheckMate())
            Debug.Log("MOVE FAIL: { from = " + from + ", to = " + to + " }");
        if (board.isCheckMate())
            uiManager.winText.text = "Checkmate " + (board.whiteTurn ? "Black" : "White") + " Won! \n Press \"R\" to restart.";
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



}
