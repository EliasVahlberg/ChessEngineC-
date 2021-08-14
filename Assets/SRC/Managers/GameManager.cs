using System;
using System.Collections;
using System.Collections.Generic;
using ChessAI;
using SimpleChess;
using UnityEngine;

/*
    @File GameManager.cs
    @author Elias Vahlberg 
    @Date 2021-07
*/
public class GameManager : MonoBehaviour
{
    //TODO Remove
    #region Managers

    [HideInInspector]
    public UIManager uiManager;
    [HideInInspector]
    public MenuManager menuManager;
    [HideInInspector]
    public NetworkGameManager networkGameManager;
    [HideInInspector]
    public NetworkUIManager networkUIManager;
    #endregion

    [HideInInspector]
    public Board board;

    private Board whiteBoard;
    private Board blackBoard;


    #region Move_Net
    public int myColor = 0;
    public Move recivedMove = new Move(-1, -1, -1);

    public Move sentMove = new Move(-1, -1, -1);
    public bool moveSent = false;
    #endregion

    #region FEN_Net


    public string sentFen = null;
    public bool sentIsWhite = false;
    public bool fenSent = false;
    #endregion

    #region AI Variables
    public List<IAI> aiList;
    public IAIObject wAI;
    public IAIObject bAI;
    public bool whiteAIPlaying = false;
    public bool blackAIPlaying = false;
    public bool useAIDelay = true;
    public bool isAIPaused = false;
    public int aiDelayMs = 50;
    public bool aiWaitingToMove = false;
    public static int DEFAULT_AI_DELAY_MS = 50;
    public DateTime aiDelayStart;
    private bool aiPendingSearchMove = false;
    private bool aiPendingComplete = false;
    public bool AIPendingComplete { get => aiPendingComplete; set => aiPendingComplete = value; }
    private bool isWhiteAIPending = false;
    #endregion

    #region State

    public bool newTurnFlag = false;

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
    #endregion

    public static GameManager instance;

    #region INIT

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

    #endregion

    #region BoardCreation
    public Board createBoard()
    {
        return createBoard(FENUtills.DEFAULT_START_FEN); ;
    }

    public Board createBoard(string fen)
    {
        return new Board(fen);
    }
    #endregion

    //TODO Fix new board system
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
            if (!SynchronizedUseMove(sentMove) && !board.isCheckMate())
                Debug.Log("MOVE FAIL: { from = " + selectedPiece + ", to = " + selectedMoveTo + " }");
            else
                newTurnFlag = true;
            moveSent = false;
        }
        else
        {
            if (!SynchronizedUseMove(recivedMove) && !board.isCheckMate())
                Debug.Log("MOVE FAIL: { from = " + recivedMove.StartSquare + ", to = " + recivedMove.TargetSquare + " }");
            else
                newTurnFlag = true;
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

    #region AI

    private Move getAIMove()
    {
        if (whiteAIPlaying && board.whiteTurn)
            return AIManager.instance.SelectMove(wAI, whiteBoard);
        else if (blackAIPlaying && !board.whiteTurn)
            return AIManager.instance.SelectMove(bAI, blackBoard);
        return new Move(0);
    }

    public bool playAIMove()
    {
        isWhiteAIPending = board.whiteTurn;
        if (ended || !started)
            return false;
        if (isAIPaused)
            return false;
        if (useAIDelay && aiDelayStart == null)
            aiDelayStart = DateTime.Now;
        if (useAIDelay && (((TimeSpan)(DateTime.Now - aiDelayStart)).TotalMilliseconds < aiDelayMs))
        { return false; }

        Move move = getAIMove();
        if (move.Equals(IAIObject.PENDING_SEARCH_MOVE))
        {
            Debug.Log("PENDING");
            aiPendingSearchMove = true;

            return false;
        }
        if (move.StartSquare == 0 && move.TargetSquare == 0)
        {
            return false;
        }
        SynchronizedUseMove(move);

        string winMes;
        if ((winMes = isEndGameCondition()) != "")
        {
            uiManager.winText.text = winMes;
            AIManager.instance.toggleAIPaus();
            return false;

        }
        aiDelayStart = DateTime.Now;
        return true;
    }

    public void checkPendingMove()
    {
        if (!started || ended)
        {
            aiPendingSearchMove = false;
            aiPendingComplete = false;
            return;
        }
        Move move = (isWhiteAIPending ? wAI.SelectMove(null) : bAI.SelectMove(null));
        if (move.Equals(IAIObject.PENDING_SEARCH_MOVE))
        {
            throw new ArgumentNullException("NO MOVE FOUND HERE");
        }
        SynchronizedUseMove(move);
        aiPendingSearchMove = false;
        aiPendingComplete = false;
        aiDelayStart = DateTime.Now;
        newTurnFlag = true;

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
    }

    public void toggleAIPaus()
    {
        if (isAIPaused)
            resumeAI();
        else
            pausAI();

    }

    public void ResetAI()
    {
        isAIPaused = false;
        whiteAIPlaying = false;
        blackAIPlaying = false;
        aiWaitingToMove = true;
        wAI = null;
        bAI = null;
        aiDelayMs = DEFAULT_AI_DELAY_MS;
        AIManager.instance.resetAIManager();
    }

    #endregion

    #region Events

    public string isEndGameCondition()
    {
        string mes = "";
        board.generateNewMoves();
        if (board.isCheckMate())
            mes = "Checkmate " + (board.whiteTurn ? "Black" : "White") + " Won! \n";
        else if (board.isDraw())
            mes = "Draw! \n";
        else if (blackForfit)
            mes = "Black Forfit ! \n";
        else if (whiteForfit)
            mes = "White Forfit ! \n";
        if (mes != "")
        {
            started = false;
            ended = true;
            return mes + " Press \"R\" to restart or \"ESC\" to go back.";
        }
        return mes;

    }

    public void onNewTurn(Move move, bool wasWhite)
    {
        if (!started)
        {
            return;
        }
        board.generateNewMoves();
        whiteBoard.generateNewMoves();
        blackBoard.generateNewMoves();
        //Debug.Log("NUM MOVES:" + board.Moves.Count);
        uiManager.LastMoveTint(move.StartSquare, move.TargetSquare);

        GameHistoryPanel.instance.addHistoryItem(board.boardToFEN(), move, wasWhite, board.lastMoveWasCapture);
        if (board.lastMoveWasCapture)
            uiManager.updateScore(Piece.PieceValueDictionary[Piece.PieceType(board.lastMoveCaptured)], wasWhite);
        if (whiteAIPlaying && !wasWhite || blackAIPlaying && wasWhite)
        {
            AIManager.instance.letAIPlayButton.gameObject.SetActive(false);
            AIManager.instance.aiSelect.gameObject.SetActive(false);
            AIManager.instance.toggleAIPausButton.gameObject.SetActive(true);
            aiWaitingToMove = true;
        }
        else
        {
            AIManager.instance.toggleAIPausButton.gameObject.SetActive(false);
            AIManager.instance.aiSelect.gameObject.SetActive(true);
            AIManager.instance.letAIPlayButton.gameObject.SetActive(true);
        }

        newTurnFlag = false;
        uiManager.hideDanger();
    }

    public void onStartingGame()
    {

        board.generateNewMoves();
        whiteBoard.generateNewMoves();
        blackBoard.generateNewMoves();
        UIManager.instance.ShowInGameUI();


    }

    public void onStoppingGame()
    {
        board = null;
        whiteBoard = null;
        blackBoard = null;
        UIManager.instance.ResetInGameUI();
        UIManager.instance.HideInGameUI();
        ended = false;
        started = false;
    }

    public void onResetGame()
    {
        UIManager.instance.ShowInGameUI();
        whiteForfit = false;
        blackForfit = false;
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
    #endregion

    #region Actions
    public void selectPosition()
    {
        if (started)
        {
            if (!isNetworked)
            {

                if (selectedPiece == selectedMoveTo)
                    return;
                Move move = (board.whiteTurn) ? whiteBoard.getMove(selectedPiece, selectedMoveTo) : blackBoard.getMove(selectedPiece, selectedMoveTo);
                Debug.Log(move.ToString());
                if (move.MoveValue == 0)
                {
                    Debug.Log("MOVE FAIL: { from = " + selectedPiece + ", to = " + selectedMoveTo + " }");
                    return;
                }
                if (move.isPromotion())
                {
                    Debug.Log("Is promotion!");
                    UIManager.instance.onPromotionSelectEnter(move);
                    return; //Await response
                }
                SynchronizedUseMove(move);

            }
            else
            {
                //TODO FIX for multiplayer
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

    public void selectPromotionMove(int option, int start, int target)
    {
        if (option == 0)
            return;
        Debug.Log(start + ", " + target + ", " + option);
        Move promMove = new Move(0);
        switch (option)
        {
            case Piece.QUEEN:
                promMove = new Move(start, target, Move.Flag.PromoteToQueen);
                break;
            case Piece.KNIGHT:
                promMove = new Move(start, target, Move.Flag.PromoteToKnight);
                break;
            case Piece.ROOK:
                promMove = new Move(start, target, Move.Flag.PromoteToRook);
                break;
            case Piece.BISHOP:
                promMove = new Move(start, target, Move.Flag.PromoteToBishop);
                break;
            default:
                Debug.Log("FAILED IN PROMOTION MOVE");
                return;
        }
        SynchronizedUseMove(promMove);

    }

    public void UnmakeMove()
    {
        if (isNetworked)
            return;
        int val = -Piece.PieceValueDictionary[Piece.PieceType(board.lastMoveCaptured)];
        if (SynchronizedUnmakeMove())
        {
            UIManager.instance.updateScore(val, !board.whiteTurn);
            uiManager.playUndoSound();
            GameHistoryPanel.instance.removeLast();
        }
        else { Debug.Log("UNDOFAIL"); }

    }

    public void resetBoard()
    {

        board = createBoard();
        whiteBoard = createBoard();
        blackBoard = createBoard();
        started = true;
        UIManager.instance.generatePieceUI();
        string s = "Turn:" + (board.Turn + 1) + "\n" + "Color: " + (board.whiteTurn ? "White" : "Black") + "\n" + "Check: " + (board.Check ? (board.WhiteInCheck ? "White" : "Black") : "None");
        UIManager.instance.gameText.text = s;
        board.generateNewMoves();
        whiteBoard.generateNewMoves();
        blackBoard.generateNewMoves();

    }

    public void resetBoard(string fen)
    {

        board = createBoard(fen);
        whiteBoard = createBoard(fen);
        blackBoard = createBoard(fen);
        started = true;
        UIManager.instance.generatePieceUI();
        string s = "Turn:" + (board.Turn + 1) + "\n" + "Color: " + (board.whiteTurn ? "White" : "Black") + "\n" + "Check: " + (board.Check ? (board.WhiteInCheck ? "White" : "Black") : "None");
        UIManager.instance.gameText.text = s;
        board.generateNewMoves();
        whiteBoard.generateNewMoves();
        blackBoard.generateNewMoves();
    }

    public void forfit()
    {
        uiManager.winText.text = isEndGameCondition();
        started = false;
        ended = true;

    }

    #endregion

    #region SynchronizedBoardActions
    public bool SynchronizedUseMove(Move move)
    {
        Debug.Log("Move Used : " + move.ToString());
        if (!board.useMove(move, UIManager.instance))
            Debug.LogError("MANAGER BOARD:{MOVE FAIL: { from = " + move.StartSquare + ", to = " + move.TargetSquare + " } }");
        else if (!whiteBoard.useMove(move))
            Debug.LogError("WHITE BOARD:{MOVE FAIL: { from = " + move.StartSquare + ", to = " + move.TargetSquare + " } }");
        else if (!blackBoard.useMove(move))
            Debug.LogError("BLACK BOARD:{MOVE FAIL: { from = " + move.StartSquare + ", to = " + move.TargetSquare + " } }");
        else
        {
            newTurnFlag = true;
            string winMes;
            if ((winMes = isEndGameCondition()) != "")
                uiManager.winText.text = winMes;

            return true;

        }
        return false;
    }

    public bool SynchronizedUnmakeMove()
    {
        if (!board.UnmakeMove(uiManager))
            Debug.LogError("MANAGER BOARD:{Unmake Move FAIL}");
        else if (!whiteBoard.UnmakeMove())
            Debug.LogError("WHITE BOARD:{Unmake Move FAIL}");
        else if (!blackBoard.UnmakeMove())
            Debug.LogError("BLACK BOARD:{Unmake Move FAIL}");
        else
        {
            board.generateNewMoves();
            whiteBoard.generateNewMoves();
            blackBoard.generateNewMoves();
            return true;
        }
        return false;
    }

    #endregion
    private void Update()
    {
        if (started)
        {
            if (aiPendingSearchMove && !aiPendingComplete)
            { }
            else if (aiPendingSearchMove && aiPendingComplete)
                checkPendingMove();
            else if (newTurnFlag)
                onNewTurn(board.lastMove, !board.whiteTurn);
            else if (whiteAIPlaying && board.whiteTurn || blackAIPlaying && !board.whiteTurn)
            {
                if (useAIDelay && aiWaitingToMove)
                    playAIMove();
            }
        }
    }


}
