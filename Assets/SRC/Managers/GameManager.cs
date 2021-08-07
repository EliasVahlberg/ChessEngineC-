using System;
using System.Collections;
using System.Collections.Generic;
using ChessAI;
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
    #region AI
    public List<IAI> aiList;
    public IAIObject wAI;
    public IAIObject bAI;
    public bool whiteAIPlaying = false;
    public bool blackAIPlaying = false;
    public bool useAIDelay = true;
    public bool isAIPaused = false;
    public long aiDelayMs = 0;
    public DateTime aiDelayStart;
    #endregion
    //UI interface
    public int selectedMoveTo = -1;
    public int selectedPiece = -1;
    public bool blackForfit = false;
    public bool whiteForfit = false;
    public bool started = false;
    public bool ended = false;
    public bool isNetworked
    {
        get { return NetworkUIManager.instance.IsConnected(); }
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


    public Board createBoard()
    {
        return createBoard(FENUtills.DEFAULT_START_FEN); ;
    }

    public Board createBoard(string fen)
    {
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
                else
                    onNewTurn(board.lastMove, !board.whiteTurn);
                string winMes;
                if ((winMes = isEndGameCondition()) != "")
                    uiManager.winText.text = winMes;

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
        else
            Debug.Log("NOSTART");

    }

    // public void selectPosition(int from, int to)
    // {
    //     if (started)
    //     {
    //         if (from == to)
    //             return;
    //         if (!board.tryMove(from, to, uiManager) && !board.isCheckMate())
    //             Debug.Log("MOVE FAIL: { from = " + from + ", to = " + to + " }");
    //         if (board.isCheckMate())
    //             uiManager.winText.text = "Checkmate " + (board.whiteTurn ? "Black" : "White") + " Won! \n Press \"R\" to restart.";
    //     }
    // }
    #region Network

    public void resetSentMove()
    {
        sentMove = new Move(-1, -1, -1);
        moveSent = false;
    }

    public void UpdateBoard()
    {
        if (myColor == board.ColorTurn)
        {
            if (!board.useMove(sentMove, uiManager) && !board.isCheckMate())
                Debug.Log("MOVE FAIL: { from = " + selectedPiece + ", to = " + selectedMoveTo + " }");
            else
                onNewTurn(board.lastMove, !board.whiteTurn);
            moveSent = false;
        }
        else
        {
            if (!board.useMove(recivedMove, uiManager) && !board.isCheckMate())
                Debug.Log("MOVE FAIL: { from = " + recivedMove.StartSquare + ", to = " + recivedMove.TargetSquare + " }");
            else
                onNewTurn(board.lastMove, !board.whiteTurn);
        }
    }

    public bool ReciveMove(Move move)
    {
        if (myColor == board.ColorTurn)
        {
            Debug.Log("OPPONENT TURN MISSMATCH!");
            return false;
        }
        if (board.containsMove(move))
        {
            recivedMove = move;
            UpdateBoard();
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

            onResetGame(validFEN);
            MenuManager.instance.hideLobby();
            Debug.Log("FEN ACCEPTED");
            return true;
        }
        Debug.Log("FEN NOT ACCEPTED");
        return false;
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

                onResetGame(validFEN);
                MenuManager.instance.hideLobby();
                Debug.Log("SENT FEN USED");
                return;
            }
            Debug.Log("SENT FEN FAIL");
        }
        Debug.Log("FEN NOT SENT");
    }
    #endregion
    public void resetBoard()
    {

        board = createBoard();
        started = true;
        UIManager.instance.generatePieceUI();
        string s = "Turn:" + (board.Turn + 1) + "\n" + "Color: " + (board.whiteTurn ? "White" : "Black") + "\n" + "Check: " + (board.Check ? (board.WhiteInCheck ? "White" : "Black") : "None");
        UIManager.instance.gameText.text = s;
        board.generateNewMoves();

    }

    public void resetBoard(string fen)
    {

        board = createBoard(fen);
        started = true;
        UIManager.instance.generatePieceUI();
        string s = "Turn:" + (board.Turn + 1) + "\n" + "Color: " + (board.whiteTurn ? "White" : "Black") + "\n" + "Check: " + (board.Check ? (board.WhiteInCheck ? "White" : "Black") : "None");
        UIManager.instance.gameText.text = s;
        board.generateNewMoves();
    }

    public void forfit()
    {

        if (blackForfit)
            uiManager.winText.text = "Black Forfit ! \n Press \"R\" to restart.";
        else if (whiteForfit)
            uiManager.winText.text = "White Forfit ! \n Press \"R\" to restart.";
        started = false;
        ended = true;

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
        if (mes != "")
        {
            started = false;
            ended = true;
        }
        return mes;

    }

    private Move getAIMove()
    {
        if (whiteAIPlaying && board.whiteTurn)
        {
            if (board.Moves.Count == 0)
            {
                Debug.Log("White AI Lost");
            }
            Debug.Log("White AI:" + wAI.Name);
            return AIManager.instance.SelectMove(wAI, board);
        }
        else if (blackAIPlaying && !board.whiteTurn)
        {
            if (board.Moves.Count == 0)
            {
                Debug.Log("Black AI Lost");
            }
            Debug.Log("Black AI:" + bAI.Name);
            return AIManager.instance.SelectMove(bAI, board);
        }
        return new Move(0);
    }

    public bool playAIMove()
    {
        if (ended || !started)
            return false;
        if (isAIPaused)
            return false;
        if (useAIDelay && aiDelayStart == null)
            aiDelayStart = DateTime.Now;
        if (useAIDelay && (((TimeSpan)(DateTime.Now - aiDelayStart)).TotalMilliseconds < aiDelayMs))
            return false;
        else aiDelayStart = DateTime.Now;
        Move move = getAIMove();
        if (move.StartSquare == 0 && move.TargetSquare == 0)
        {
            return false;
        }
        board.useMove(move, uiManager);
        onNewTurn(board.lastMove, !board.whiteTurn);
        string winMes;
        if ((winMes = isEndGameCondition()) != "")
        {
            uiManager.winText.text = winMes;
            AIManager.instance.toggleAIPaus();

        }
        return true;
    }

    private void pausAI()
    {
        if (blackAIPlaying || whiteAIPlaying)
            isAIPaused = true;
    }
    private void resumeAI()
    {
        if (blackAIPlaying || whiteAIPlaying)
            isAIPaused = false;
        playAIMove();
    }
    public void toggleAIPaus()
    {
        if (isAIPaused)
            resumeAI();
        else
            pausAI();

    }

    public void onNewTurn(Move move, bool wasWhite)
    {
        if (!started)
        {
            return;
        }
        board.generateNewMoves();
        Debug.Log("NUM MOVES:" + board.Moves.Count);
        uiManager.LastMoveTint(move.StartSquare, move.TargetSquare);

        GameHistoryPanel.instance.addHistoryItem(board.boardToFEN(), move, wasWhite, board.lastMoveWasCapture);
        if (board.lastMoveWasCapture)
            uiManager.updateScore(Piece.PieceValueDictionary[Piece.PieceType(board.lastMoveCaptured)], wasWhite);
        if (whiteAIPlaying && !wasWhite || blackAIPlaying && wasWhite)
        {
            AIManager.instance.letAIPlayButton.gameObject.SetActive(false);
            AIManager.instance.aiSelect.gameObject.SetActive(false);
            AIManager.instance.toggleAIPausButton.gameObject.SetActive(true);
            playAIMove();
        }
        else
        {
            AIManager.instance.toggleAIPausButton.gameObject.SetActive(false);
            AIManager.instance.aiSelect.gameObject.SetActive(true);
            AIManager.instance.letAIPlayButton.gameObject.SetActive(true);
        }

        uiManager.hideDanger();
    }

    public void onStartingGame()
    {
        board.generateNewMoves();
        UIManager.instance.ShowInGameUI();


    }
    //TODO IMPLEMENT
    public void onStoppingGame()
    {
        board = null;
        UIManager.instance.ResetInGameUI();
        UIManager.instance.HideInGameUI();
        ended = false;
        started = false;
    }

    public void onResetGame()
    {
        UIManager.instance.ResetInGameUI();
        ended = false;
        resetBoard();
    }

    public void onResetGame(string FEN)
    {
        UIManager.instance.ResetInGameUI();
        ended = false;
        resetBoard(FEN);
    }

    public void UnmakeMove()
    {
        int val = -Piece.PieceValueDictionary[Piece.PieceType(board.lastMoveCaptured)];
        if (board.UnmakeMove(uiManager))
        {
            Debug.Log("UNMADE");
            UIManager.instance.updateScore(val, !board.whiteTurn);
            uiManager.playUndoSound();
            GameHistoryPanel.instance.removeLast();
        }
        else { Debug.Log("UNDOFAIL"); }
        board.generateNewMoves();
    }

    private void Update()
    {
        if (whiteAIPlaying && board.whiteTurn || blackAIPlaying && !board.whiteTurn)
            if (useAIDelay)
                playAIMove();
    }

    public void ResetAI()
    {
        isAIPaused = false;
        whiteAIPlaying = false;
        blackAIPlaying = false;
        wAI = null;
        bAI = null;
        aiDelayMs = 0;
    }
}
