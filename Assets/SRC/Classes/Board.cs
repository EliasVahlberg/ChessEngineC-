using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Piece;

public class Board
{
    public int[] tiles;

    #region New
    private MoveUtills moveGenerator;
    private bool hasGeneratedMoves = false;
    public const int WhiteIndex = 0;
    public const int BlackIndex = 1;
    public int ColorIndex = 0;
    //public int ColorToMove = 0;
    public int[] KingSquares;
    public ulong ZobristKey;
    public Stack<ulong> HashHistory;
    public Stack<GameState> gameStateHistory;
    public GameState currGameState;
    public PieceTable[] allPieceTables;
    public PieceTable[] rooks;
    public PieceTable[] bishops;
    public PieceTable[] queens;
    public PieceTable[] knights;
    public PieceTable[] pawns;
    public uint castleRights;
    #endregion
    //TODO REPLACE
    #region PieceLists

    private List<int> blackPieces = null;
    public List<int> BlackPieces
    {
        get
        {
            if (blackPieces == null)
            {
                blackPieces = new List<int>();
                for (int ii = 0; ii < 64; ii++)
                    if (Piece.IsBlack(tiles[ii]))
                        blackPieces.Add(ii);

            }
            return blackPieces;
        }
    }

    private List<int> whitePieces = null;
    public List<int> WhitePieces
    {
        get
        {
            if (whitePieces == null)
            {
                whitePieces = new List<int>();
                for (int ii = 0; ii < 64; ii++)
                    if (Piece.IsWhite(tiles[ii]))
                        whitePieces.Add(ii);
                //Debug.Log("GENPW");

            }
            return whitePieces;
        }
    }

    #endregion

    //TODO REPLACE/REMOVE
    #region Pinned
    // private int lastGeneratedPinnedBlack = -1;
    // private int[][] pinnedMapBlack = new int[64][];
    // private int[][] PinnedMapBlack
    // {
    //     get
    //     {
    //         if (lastGeneratedPinnedBlack != turn || turn == 0)
    //         {
    //             MoveLegalityUtills.genPinnedMap(this, BLACK, pinnedMapBlack);
    //             lastGeneratedPinnedBlack = turn;
    //             //GENPIN
    //             //Debug.Log("GENPINB");
    //         }
    //         return pinnedMapBlack;
    //     }
    // }

    // private int lastGeneratedPinnedWhite = -1;
    // private int[][] pinnedMapWhite = new int[64][];
    // public int[][] PinnedMapWhite
    // {
    //     get
    //     {
    //         if (lastGeneratedPinnedWhite != turn || turn == 0)
    //         {
    //             MoveLegalityUtills.genPinnedMap(this, WHITE, pinnedMapWhite);
    //             lastGeneratedPinnedWhite = turn;
    //         }
    //         return pinnedMapWhite;
    //     }
    // }

    #endregion

    //TODO Replace/Remove
    #region Capturable
    // private int lastGeneratedWhiteCaptureable = -1;
    // private bool[] whiteCaptureable = new bool[64];

    // //!DEPRECATED
    // private List<int>[] whiteCapturableMapList = new List<int>[64];
    // //* is Somehow 0% called much less (Never not equals turn)
    public bool[] WhiteCap;
    // {
    //     get
    //     {
    //         if (lastGeneratedWhiteCaptureable != turn || turn == 0)
    //         {
    //             whiteCaptureable = MoveLegalityUtills.updateCapturable(this, true);
    //             lastGeneratedWhiteCaptureable = turn;
    //         }
    //         return whiteCaptureable;
    //     }
    //     set { whiteCaptureable = value; }
    // }

    // private int lastGeneratedBlackCaptureable = -1;
    // private bool[] blackCaptureable = new bool[64];
    // //!DEPRECATED
    // private List<int>[] blackCapturableMapList = new List<int>[64];
    // //TODO OPTIMIZE 1.2% (Self = 0%)
    public bool[] BlackCap;
    // {
    //     get
    //     {
    //         if (lastGeneratedBlackCaptureable != turn || turn == 0)
    //         {
    //             blackCaptureable = MoveLegalityUtills.updateCapturable(this, false); //TODO OPTIMIZE 1.2%
    //             lastGeneratedBlackCaptureable = turn;
    //         }
    //         return blackCaptureable;
    //     }
    //     set { blackCaptureable = value; }
    // }

    #endregion

    //TODO Maby extract
    #region Moves
    //private List<Move> moves = new List<Move>();
    //private int lastTurnGenerated = -1;
    public List<Move> Moves
    {
        get
        {
            return moveGenerator.Moves;
        }
    }

    // //!DEPRECATED
    // private int lastTurnGeneratedMoveMap = -1;
    // //!DEPRECATED
    // private List<Move>[] moveMap;
    // //!DEPRECATED
    public List<Move>[] MoveMap;
    // {
    //     get
    //     {
    //         if (lastTurnGeneratedMoveMap != turn || turn == 0)
    //         {
    //             moveMap = MoveUtills.sortMovesBasedOnPosition(Moves);
    //             lastTurnGeneratedMoveMap = turn;
    //         }
    //         return moveMap;
    //     }
    //     set { moveMap = value; }
    // }

    // //!DEPRECATED
    // private List<Move> oponentMoves;
    // //!DEPRECATED
    // private int lastTurnGeneratedOpo = -1;
    // //!DEPRECATED
    // public List<Move> OponentMoves
    // {
    //     get
    //     {
    //         if (lastTurnGeneratedOpo != turn || turn == 0)
    //         {
    //             whiteTurn = !whiteTurn;
    //             moves = MoveUtills.GenerateMoves(this);
    //             whiteTurn = !whiteTurn;
    //             lastTurnGeneratedOpo = turn;
    //         }
    //         return moves;
    //     }
    // }

    #endregion

    #region State

    private int turn = 0;
    public int Turn { get => turn; set => turn = value; }
    public bool whiteTurn;

    public int whiteKingPos
    {
        get
        {
            return KingSquares[0];
        }
        set
        {
            KingSquares[0] = value;
        }
    }
    public int blackKingPos
    {
        get
        {
            return KingSquares[1];
        }
        set
        {
            KingSquares[1] = value;
        }
    }

    public bool WhiteInCheck;
    public bool BlackInCheck;

    public bool whiteCastleKingside
    {
        get
        {
            return currGameState.WhiteCastleKingside();
        }
        set
        {
            currGameState.SetWhiteCastleKingside(value);
        }
    }
    public bool whiteCastleQueenside
    {
        get
        {
            return currGameState.WhiteCastleQueenside();
        }
        set
        {
            currGameState.SetWhiteCastleQueenside(value);
        }
    }
    public bool blackCastleKingside
    {
        get
        {
            return currGameState.BlackCastleKingside();
        }
        set
        {
            currGameState.SetBlackCastleKingside(value);
        }
    }
    public bool blackCastleQueenside
    {
        get
        {
            return currGameState.BlackCastleQueenside();
        }
        set
        {
            currGameState.SetBlackCastleQueenside(value);
        }
    }

    public int enPassantAble
    {
        get
        {
            return currGameState.EnPassant();
        }
        set
        {
            currGameState.SetEnPassant(value);
        }
    }
    public int fiftyCount
    {
        get
        {
            return currGameState.FiftyTurnCount();
        }
        set
        {
            currGameState.SetFiftyTurnCount(value);
        }
    }
    #endregion

    #region LastState

    public Move lastMove;
    public bool lastMoveWasCapture = false;
    public int lastMoveCaptured = 0;

    private bool hasReverted = false;

    private int lastMoveEnPas = 0;
    private int lastMoveFiftyCount = 0;

    private bool lastMoveWhiteCastleKingside;
    private bool lastMoveWhiteCastleQueenside;
    private bool lastMoveBlackCastleKingside;
    private bool lastMoveBlackCastleQueenside;
    #endregion

    #region Constructors
    public Board()
    {
        tiles = new int[64];
    }

    public Board(string fen)
    {
        tiles = new int[64];
        currGameState = new GameState(0);
        FENUtills.GameStateInfo gameStateInfo = FENUtills.generatePiecePositions(fen);
        tiles = gameStateInfo.squareGrid;
        currGameState.SetWhiteTurn(gameStateInfo.whiteTurn);
        whiteTurn = gameStateInfo.whiteTurn;
        currGameState.SetWhiteCastleKingside(gameStateInfo.whiteCastleKingside);
        whiteCastleKingside = gameStateInfo.whiteCastleKingside;
        currGameState.SetWhiteCastleQueenside(gameStateInfo.whiteCastleQueenside);
        whiteCastleQueenside = gameStateInfo.whiteCastleQueenside;
        currGameState.SetBlackCastleKingside(gameStateInfo.blackCastleKingside);
        blackCastleKingside = gameStateInfo.blackCastleKingside;
        currGameState.SetBlackCastleQueenside(gameStateInfo.blackCastleQueenside);
        blackCastleQueenside = gameStateInfo.blackCastleQueenside;


        turn = ((gameStateInfo.plyCount - 1) * 2);
        if (turn < 0) turn = 0;
        fiftyCount = gameStateInfo.fiftyCount;
        currGameState.SetFiftyTurnCount(gameStateInfo.fiftyCount);
        currGameState.SetEnPassant(gameStateInfo.epindex);
        LoadPosition();
        refreshPieces();
    }
    #endregion

    #region StateInfo

    public bool Check
    {
        get { return WhiteInCheck || BlackInCheck; }
    }

    public int ColorTurn
    {
        get { return whiteTurn ? WHITE : BLACK; }
    }

    public int OpoColor
    {
        get { return !whiteTurn ? WHITE : BLACK; }
    }

    public bool CurPlayerInCheck
    {
        get { return moveGenerator.InCheck; }
    }
    #endregion

    #region Utillity

    public string boardToFEN()
    {
        FENUtills.GameStateInfo gameStateInfo = new FENUtills.GameStateInfo();
        gameStateInfo.squareGrid = tiles;
        gameStateInfo.whiteTurn = whiteTurn;
        gameStateInfo.whiteCastleKingside = whiteCastleKingside;
        gameStateInfo.whiteCastleQueenside = whiteCastleQueenside;
        gameStateInfo.blackCastleKingside = blackCastleKingside;
        gameStateInfo.blackCastleQueenside = blackCastleQueenside;
        if (enPassantAble != -1)
            gameStateInfo.epindex = enPassantAble;

        gameStateInfo.fiftyCount = fiftyCount;
        gameStateInfo.plyCount = Turn / 2 + 1;

        return FENUtills.generateFEN(gameStateInfo);
    }


    public Board Clone()
    {
        Board copy = new Board();
        copy.tiles = new int[64];
        Array.Copy(tiles, copy.tiles, 64);
        copy.turn = 0;
        copy.whiteCastleKingside = whiteCastleKingside;
        copy.whiteCastleQueenside = whiteCastleQueenside;
        copy.blackCastleKingside = blackCastleKingside;
        copy.blackCastleQueenside = blackCastleQueenside;
        copy.whiteTurn = whiteTurn;
        copy.enPassantAble = enPassantAble;
        copy.whiteKingPos = whiteKingPos;
        copy.blackKingPos = blackKingPos;
        LoadPosition();
        //generateNewMoves();

        return copy;

    }
    #endregion

    #region InnerFuncs
    private bool MoveInnerV2(Move move, UIManager uiManager)
    {
        try
        {
            int from = move.StartSquare, to = move.TargetSquare;
            int enPas = -1;
            if (move.moveFlag == Move.Flag.EnPassantCapture)
                enPas = enPassantAble;
            if (!MoveInnerV2(move))//MoveInner(move))
            { Debug.Log("FAILMOVE"); return false; }

            switch (move.moveFlag)
            {
                case Move.Flag.EnPassantCapture:
                    uiManager.movePiece(from, to);
                    uiManager.destroyPiece(enPas);
                    break;
                case Move.Flag.Castling:
                    uiManager.movePiece(from, to);
                    if (from < to)
                        uiManager.movePiece(from + 3, from + 1);
                    else
                        uiManager.movePiece(from - 4, from - 1);
                    break;
                case Move.Flag.PromoteToQueen:

                    uiManager.movePiece(from, to);
                    uiManager.pieceUI[to].setSprite(uiManager.piceSprites[uiManager.pieceTypeToSprite[tiles[to]]]);
                    break;
                case Move.Flag.PromoteToRook:
                    uiManager.movePiece(from, to);
                    uiManager.pieceUI[to].setSprite(uiManager.piceSprites[uiManager.pieceTypeToSprite[tiles[to]]]);
                    break;
                case Move.Flag.PromoteToBishop:
                    uiManager.movePiece(from, to);
                    uiManager.pieceUI[to].setSprite(uiManager.piceSprites[uiManager.pieceTypeToSprite[tiles[to]]]);
                    break;
                case Move.Flag.PromoteToKnight:
                    uiManager.movePiece(from, to);
                    uiManager.pieceUI[to].setSprite(uiManager.piceSprites[uiManager.pieceTypeToSprite[tiles[to]]]);
                    break;
                default:
                    uiManager.movePiece(from, to);
                    break;
            }
            string s = "Turn:" + (Turn + 1) + "\n" + "Color: " + (whiteTurn ? "White" : "Black") + "\n" + "Check: " + (Check ? (WhiteInCheck ? "White" : "Black") : "None");
            uiManager.gameText.text = s;
            lastMove = move;
            return true;
        }
        catch (Exception _ex)
        {
            Debug.Log("EXCEPTION DURING MoveInnerUI, ex:" + _ex.ToString());
            return false;
        }
    }

    private bool MoveInnerV2(Move move)
    {
        try
        {


            bool WhiteToMove = currGameState.WhiteTurn();
            uint newCastleState = currGameState.CastleRights();
            uint oldCastleState = currGameState.CastleRights();
            int fiftyMoveCounter = currGameState.FiftyTurnCount();
            gameStateHistory.Push(currGameState);
            GameState nextGameState = new GameState(0);//new GameState(currGameState.gameStateValue);

            int ColorToMove = (whiteTurn) ? WHITE : BLACK;
            int opponentColourIndex = 1 - ColorIndex;
            int moveFrom = move.StartSquare;
            int moveTo = move.TargetSquare;

            int capturedPieceType = Piece.PieceType(tiles[moveTo]);
            int movePiece = tiles[moveFrom];
            int movePieceType = Piece.PieceType(movePiece);

            int moveFlag = move.moveFlag;
            bool isPromotion = move.isPromotion();
            bool isEnPassant = moveFlag == Move.Flag.EnPassantCapture;



            // Handle captures
            if (capturedPieceType != 0)
            {
                nextGameState.SetPrevCapturedIndex(moveTo);
                nextGameState.SetPrevCapturedType(capturedPieceType);
            }
            if (capturedPieceType != 0 && !isEnPassant)
            {
                //ZobristKey ^= Zobrist.piecesArray[capturedPieceType, opponentColourIndex, moveTo];
                GetPieceTable(capturedPieceType, opponentColourIndex).RemovePieceAtSquare(moveTo);
            }

            // Move pieces in piece lists
            if (movePieceType == Piece.KING)
            {
                KingSquares[ColorIndex] = moveTo;
                newCastleState &= (whiteTurn) ? 0b1100U : 0b0011U;
            }
            else
            {

                GetPieceTable(movePieceType, ColorIndex).MovePiece(moveFrom, moveTo);
            }

            int pieceOnTargetSquare = movePiece;

            // Handle promotion
            if (isPromotion)
            {
                int promoteType = 0;
                switch (moveFlag)
                {
                    case Move.Flag.PromoteToQueen:
                        promoteType = Piece.QUEEN;
                        queens[ColorIndex].AddPieceAtSquare(moveTo);
                        break;
                    case Move.Flag.PromoteToRook:
                        promoteType = Piece.ROOK;
                        rooks[ColorIndex].AddPieceAtSquare(moveTo);
                        break;
                    case Move.Flag.PromoteToBishop:
                        promoteType = Piece.BISHOP;
                        bishops[ColorIndex].AddPieceAtSquare(moveTo);
                        break;
                    case Move.Flag.PromoteToKnight:
                        promoteType = Piece.KNIGHT;
                        knights[ColorIndex].AddPieceAtSquare(moveTo);
                        break;

                }
                pieceOnTargetSquare = promoteType | ColorToMove;
                pawns[ColorIndex].RemovePieceAtSquare(moveTo);
            }
            else
            {
                // Handle other special moves (en-passant, and castling)
                switch (moveFlag)
                {
                    case Move.Flag.EnPassantCapture:
                        //!CAUSED MASSIVE ISSUE was:ColourIndex 
                        int epPawnSquare = moveTo + ((ColorToMove == Piece.WHITE) ? -8 : 8);
                        nextGameState.SetPrevCapturedIndex(epPawnSquare);
                        nextGameState.SetPrevCapturedType(PAWN);
                        //nextGameState.SetEnPassant(epPawnSquare);
                        tiles[epPawnSquare] = 0; // clear ep capture square
                        pawns[opponentColourIndex].RemovePieceAtSquare(epPawnSquare);
                        //ZobristKey ^= Zobrist.piecesArray[Piece.Pawn, opponentColourIndex, epPawnSquare];
                        break;
                    case Move.Flag.Castling:
                        bool kingside = moveTo == BoardUtills.g1 || moveTo == BoardUtills.g8;
                        int castlingRookFromIndex = (kingside) ? moveTo + 1 : moveTo - 2;
                        int castlingRookToIndex = (kingside) ? moveTo - 1 : moveTo + 1;

                        tiles[castlingRookFromIndex] = 0;
                        tiles[castlingRookToIndex] = Piece.ROOK | ColorToMove;

                        rooks[ColorIndex].MovePiece(castlingRookFromIndex, castlingRookToIndex);
                        //ZobristKey ^= Zobrist.piecesArray[Piece.Rook, ColourToMoveIndex, castlingRookFromIndex];
                        //ZobristKey ^= Zobrist.piecesArray[Piece.Rook, ColourToMoveIndex, castlingRookToIndex];
                        break;
                }
            }

            // Update the board representation:
            tiles[moveTo] = pieceOnTargetSquare;
            tiles[moveFrom] = 0;

            // Pawn has moved two forwards, mark file with en-passant flag
            if (moveFlag == Move.Flag.PawnTwoForward)
            {
                nextGameState.SetEnPassant(moveTo);
                //ZobristKey ^= Zobrist.enPassantFile[file];
            }

            // Piece moving to/from rook square removes castling right for that side
            if (oldCastleState != 0)
            {
                if (moveTo == BoardUtills.h1 || moveFrom == BoardUtills.h1)
                {
                    newCastleState &= ~(0b0001U);
                }
                else if (moveTo == BoardUtills.a1 || moveFrom == BoardUtills.a1)
                {
                    newCastleState &= ~(0b0010U);
                }
                if (moveTo == BoardUtills.h8 || moveFrom == BoardUtills.h8)
                {
                    newCastleState &= ~(0b0100U);
                }
                else if (moveTo == BoardUtills.a8 || moveFrom == BoardUtills.a8)
                {
                    newCastleState &= ~(0b1000U);
                }
            }

            // Update zobrist key with new piece position and side to move
            //ZobristKey ^= Zobrist.sideToMove;
            //ZobristKey ^= Zobrist.piecesArray[movePieceType, ColourToMoveIndex, moveFrom];
            //ZobristKey ^= Zobrist.piecesArray[Piece.PieceType(pieceOnTargetSquare), ColourToMoveIndex, moveTo];

            //if (oldEnPassantFile != 0)
            //    ZobristKey ^= Zobrist.enPassantFile[oldEnPassantFile];

            //if (newCastleState != originalCastleState)
            //{
            //    ZobristKey ^= Zobrist.castlingRights[originalCastleState]; // remove old castling rights state
            //    ZobristKey ^= Zobrist.castlingRights[newCastleState]; // add new castling rights state
            //}
            if (movePieceType == Piece.PAWN || capturedPieceType != NONE)
            {
                fiftyMoveCounter = 0;
            }
            nextGameState.SetCastleRights(newCastleState);
            nextGameState.PrevMove = move;
            nextGameState.SetWhiteTurn(!whiteTurn);
            nextGameState.SetFiftyTurnCount(fiftyMoveCounter);
            currGameState = nextGameState;
            ColorIndex = 1 - ColorIndex;
            Turn++;
            whiteTurn = !whiteTurn;
            hasGeneratedMoves = false;

            //currentGameState |= newCastleState;
            //currentGameState |= (uint)fiftyMoveCounter << 14;
            //gameStateHistory.Push(currentGameState);
            // Change side to move
            //WhiteToMove = !WhiteToMove;
            //ColourToMove = (WhiteToMove) ? Piece.White : Piece.Black;
            //OpponentColour = (WhiteToMove) ? Piece.Black : Piece.White;
            //ColourToMoveIndex = 1 - ColourToMoveIndex;
            //plyCount++;
            //fiftyMoveCounter++;

            //Debug.Log(Convert.ToString(currGameState.gameStateValue, 2));
            //Debug.Log(currGameState.gameStateValue);
            Debug.Log("M:" + Moves.Count);
            return true;


        }
        catch (Exception _ex)
        {
            Debug.LogError("EXCEPTION DURING MoveInnerV2, ex:" + _ex.ToString());
            return false;
        }

    }

    //!DEPRECATED
    private bool MoveInner(Move move)
    {
        try
        {
            if (WhitePieces == null || BlackPieces == null)
                refreshPieces();
            lastMoveFiftyCount = fiftyCount;
            lastMoveEnPas = enPassantAble;
            lastMoveWhiteCastleKingside = whiteCastleKingside;
            lastMoveWhiteCastleQueenside = whiteCastleQueenside;
            lastMoveBlackCastleKingside = blackCastleKingside;
            lastMoveBlackCastleQueenside = blackCastleQueenside;
            int from = move.StartSquare;
            int to = move.TargetSquare;
            if (enPassantAble != -1 && move.moveFlag != Move.Flag.EnPassantCapture)
                currGameState.SetEnPassant(0);
            if (tiles[to] == 0 && !IsType(tiles[from], PAWN))
                currGameState.IncrementFiftyTurnCount();
            else
                currGameState.SetFiftyTurnCount(0);
            if (IsType(tiles[to], ROOK))
                updateCasteRook(to);
            if (tiles[to] != 0)
            {
                lastMoveWasCapture = true;
                lastMoveCaptured = tiles[to];
                if (whiteTurn)
                    BlackPieces.Remove(to);
                else
                    WhitePieces.Remove(to);
            }
            else if (move.moveFlag == Move.Flag.EnPassantCapture)
            {
                if (whiteTurn)
                    BlackPieces.Remove(enPassantAble);
                else
                    WhitePieces.Remove(enPassantAble);
            }
            else
                lastMoveWasCapture = false;

            switch (move.moveFlag)
            {
                case Move.Flag.PawnTwoForward:
                    currGameState.SetEnPassant(to);
                    tiles[to] = tiles[from];
                    tiles[from] = 0;
                    break;
                case Move.Flag.EnPassantCapture:
                    tiles[enPassantAble] = 0;

                    tiles[to] = tiles[from];
                    tiles[from] = 0;
                    currGameState.SetEnPassant(0);
                    break;
                case Move.Flag.Castling:
                    castelMove(move);
                    break;
                case Move.Flag.PromoteToQueen:
                    tiles[to] = QUEEN | (whiteTurn ? WHITE : BLACK);
                    tiles[from] = 0;
                    break;
                case Move.Flag.PromoteToRook:
                    tiles[to] = ROOK | (whiteTurn ? WHITE : BLACK);
                    tiles[from] = 0;
                    break;
                case Move.Flag.PromoteToBishop:
                    tiles[to] = BISHOP | (whiteTurn ? WHITE : BLACK);
                    tiles[from] = 0;
                    break;
                case Move.Flag.PromoteToKnight:
                    tiles[to] = KNIGHT | (whiteTurn ? WHITE : BLACK);
                    tiles[from] = 0;
                    break;
                default:
                    tiles[to] = tiles[from];
                    tiles[from] = 0;
                    break;
            }

            updateCasteling(from, to);
            if (IsType(tiles[to], KING))
            {
                if (whiteTurn)
                    whiteKingPos = to;
                else
                    blackKingPos = to;
            }
            if (whiteTurn)
            {
                for (int ii = 0; ii < WhitePieces.Count; ii++)
                    if (WhitePieces[ii] == from)
                    { WhitePieces[ii] = to; break; }
            }
            else
            {
                for (int ii = 0; ii < BlackPieces.Count; ii++)
                    if (BlackPieces[ii] == from)
                    { BlackPieces[ii] = to; break; }
            }

            Turn++;
            whiteTurn = !whiteTurn;
            lastMove = move;
            hasReverted = false;
            //debugPrintMoves();
            return true;
        }
        catch (Exception _ex)
        {
            Debug.Log("EXCEPTION DURING MoveInner, ex:" + _ex.ToString());
            return false;
        }
    }
    //!DEPRECATED
    private bool UnmakeMoveInner()
    {
        if (hasReverted || lastMove.MoveValue == 0)
            return false;

        fiftyCount = lastMoveFiftyCount;
        enPassantAble = lastMoveEnPas;
        whiteCastleKingside = lastMoveWhiteCastleKingside;
        whiteCastleQueenside = lastMoveWhiteCastleQueenside;
        blackCastleKingside = lastMoveBlackCastleKingside;
        blackCastleQueenside = lastMoveBlackCastleQueenside;
        Turn--;
        whiteTurn = !whiteTurn;

        int from = lastMove.TargetSquare, to = lastMove.StartSquare;


        if (lastMove.moveFlag == Move.Flag.EnPassantCapture)
        {
            if (whiteTurn)
                BlackPieces.Add(enPassantAble);
            else
                WhitePieces.Add(enPassantAble);
        }
        else
            lastMoveWasCapture = false;

        switch (lastMove.moveFlag)
        {
            case Move.Flag.EnPassantCapture:
                tiles[enPassantAble] = (PAWN | ((whiteTurn) ? BLACK : WHITE));

                tiles[from] = tiles[to];
                tiles[to] = 0;
                break;
            case Move.Flag.Castling:
                revertCasteling(from, to);
                break;
            case Move.Flag.PromoteToQueen:
                tiles[from] = PAWN | (whiteTurn ? WHITE : BLACK);
                tiles[to] = 0;
                break;
            case Move.Flag.PromoteToRook:
                tiles[from] = PAWN | (whiteTurn ? WHITE : BLACK);
                tiles[to] = 0;
                break;
            case Move.Flag.PromoteToBishop:
                tiles[from] = PAWN | (whiteTurn ? WHITE : BLACK);
                tiles[to] = 0;
                break;
            case Move.Flag.PromoteToKnight:
                tiles[from] = PAWN | (whiteTurn ? WHITE : BLACK);
                tiles[to] = 0;
                break;
            default:
                tiles[from] = tiles[from];
                tiles[to] = 0;
                break;
        }



        if (IsType(tiles[from], KING))
        {
            if (whiteTurn)
                whiteKingPos = from;
            else
                blackKingPos = from;
        }
        if (whiteTurn)
        {
            for (int ii = 0; ii < WhitePieces.Count; ii++)
                if (WhitePieces[ii] == to)
                { WhitePieces[ii] = from; break; }
        }
        else
        {
            for (int ii = 0; ii < BlackPieces.Count; ii++)
                if (BlackPieces[ii] == to)
                { BlackPieces[ii] = from; break; }
        }
        if (lastMoveWasCapture)
        {
            tiles[to] = lastMoveCaptured;
            if (whiteTurn)
                BlackPieces.Add(to);
            else
                WhitePieces.Add(to);
        }
        hasReverted = true;
        return true;

    }

    private bool UnmakeMoveInnerV2()
    {
        try
        {


            if (gameStateHistory.Count == 0)
            {
                Debug.Log("FAIL in UnmakeMoveInnerV2");
                Debug.Log("FIRST RECORDED TURN REACHED");
                return false;
            }
            Move move = currGameState.PrevMove;
            //int opponentColour = ColourToMove;
            //int ColourToMoveIndex = whiteTurn ? 0 : 1;
            int opponentColourIndex = ColorIndex;
            bool undoingWhiteMove = !whiteTurn;
            int ColourToMove = whiteTurn ? BLACK : WHITE; // side who made the move we are undoing
            int OpponentColour = (undoingWhiteMove) ? BLACK : WHITE;
            ColorIndex = 1 - ColorIndex;
            whiteTurn = !whiteTurn;

            uint originalCastleState = currGameState.CastleRights();



            int capturedPiece = currGameState.PrevCapturedType();// | (OpponentColour);

            capturedPiece |= capturedPiece == 0 ? 0 : OpponentColour;
            int capturedPieceType = PieceType(capturedPiece);

            int movedFrom = move.StartSquare;
            int movedTo = move.TargetSquare;
            int moveFlags = move.moveFlag;
            bool isEnPassant = moveFlags == Move.Flag.EnPassantCapture;
            bool isPromotion = move.isPromotion();

            int toSquarePieceType = Piece.PieceType(tiles[movedTo]);
            int movedPieceType = (isPromotion) ? PAWN : toSquarePieceType;



            // Update zobrist key with new piece position and side to move
            //ZobristKey ^= Zobrist.sideToMove;
            //ZobristKey ^= Zobrist.piecesArray[movedPieceType, ColourToMoveIndex, movedFrom]; // add piece back to square it moved from
            //ZobristKey ^= Zobrist.piecesArray[toSquarePieceType, ColourToMoveIndex, movedTo]; // remove piece from square it moved to

            //uint oldEnPassantFile = (currentGameState >> 4) & 15;
            //if (oldEnPassantFile != 0)
            //    ZobristKey ^= Zobrist.enPassantFile[oldEnPassantFile];

            // ignore ep captures, handled later

            if (capturedPieceType != 0 && !isEnPassant)
            {
                //ZobristKey ^= Zobrist.piecesArray[capturedPieceType, opponentColourIndex, movedTo];
                GetPieceTable(capturedPieceType, opponentColourIndex).AddPieceAtSquare(movedTo);
            }

            // Update king index
            if (movedPieceType == KING)
            {
                KingSquares[ColorIndex] = movedFrom;
            }
            else if (!isPromotion)
            {

                GetPieceTable(movedPieceType, ColorIndex).MovePiece(movedTo, movedFrom);
            }


            // put back moved piece
            tiles[movedFrom] = movedPieceType | ColourToMove; // note that if move was a pawn promotion, this will put the promoted piece back instead of the pawn. Handled in special move switch
            tiles[movedTo] = capturedPiece == 0 ? 0 : capturedPiece | (OpponentColour); // will be 0 if no piece was captured

            if (isPromotion)
            {

                pawns[ColorIndex].AddPieceAtSquare(movedFrom);



                switch (moveFlags)
                {
                    case Move.Flag.PromoteToQueen:
                        queens[ColorIndex].RemovePieceAtSquare(movedTo);
                        break;
                    case Move.Flag.PromoteToKnight:
                        knights[ColorIndex].RemovePieceAtSquare(movedTo);
                        break;
                    case Move.Flag.PromoteToRook:
                        rooks[ColorIndex].RemovePieceAtSquare(movedTo);
                        break;
                    case Move.Flag.PromoteToBishop:
                        bishops[ColorIndex].RemovePieceAtSquare(movedTo);
                        break;
                }
            }
            else if (isEnPassant)
            {
                int epIndex = movedTo + ((ColourToMove == WHITE) ? -8 : 8);
                tiles[movedTo] = 0;
                tiles[epIndex] = (int)PAWN | (OpponentColour);
                pawns[opponentColourIndex].AddPieceAtSquare(epIndex);
                //ZobristKey ^= Zobrist.piecesArray[Piece.Pawn, opponentColourIndex, epIndex];

            }
            else if (moveFlags == Move.Flag.Castling)
            { // castles: move rook back to starting square
                bool kingside = movedTo == 6 || movedTo == 62;
                int castlingRookFromIndex = (kingside) ? movedTo + 1 : movedTo - 2;
                int castlingRookToIndex = (kingside) ? movedTo - 1 : movedTo + 1;

                tiles[castlingRookToIndex] = 0;
                tiles[castlingRookFromIndex] = ROOK | ColourToMove;

                rooks[ColorIndex].MovePiece(castlingRookToIndex, castlingRookFromIndex);
                //ZobristKey ^= Zobrist.piecesArray[Piece.Rook, ColourToMoveIndex, castlingRookFromIndex];
                //ZobristKey ^= Zobrist.piecesArray[Piece.Rook, ColourToMoveIndex, castlingRookToIndex];

            }
            //gameStateHistory.Pop(); // removes current state from history
            currGameState = gameStateHistory.Pop(); // sets current state to previous state in history

            //fiftyMoveCounter = (int)(currentGameState & 4294950912) >> 14;
            //int newEnPassantFile = (int)(currentGameState >> 4) & 15;
            //if (newEnPassantFile != 0)
            //ZobristKey ^= Zobrist.enPassantFile[newEnPassantFile];

            //uint newCastleState = currentGameState & 0b1111;
            //if (newCastleState != originalCastleState)
            //{
            //    ZobristKey ^= Zobrist.castlingRights[originalCastleState]; // remove old castling rights state
            //    ZobristKey ^= Zobrist.castlingRights[newCastleState]; // add new castling rights state
            //}

            turn--;

            //if (!inSearch && RepetitionPositionHistory.Count > 0)
            //{
            //    RepetitionPositionHistory.Pop();
            //}
            hasGeneratedMoves = false;
            //Debug.Log(Convert.ToString(currGameState.gameStateValue, 2));
            //Debug.Log(currGameState.gameStateValue);

            return true;

        }
        catch (Exception _ex)
        {
            Debug.LogError("FAIL in UnmakeMoveInnerV2, _ex:" + _ex);
            return false;
        }

    }
    public void updateCasteling(int from, int to)
    {
        int type = PieceType(tiles[to]);
        if (type == ROOK)
        {
            if (whiteTurn)
            {
                if (from == 0)
                    whiteCastleQueenside = false;
                if (from == 7)
                    whiteCastleKingside = false;
            }
            else
            {
                if (from == 63 - 7)
                    blackCastleQueenside = false;
                if (from == 63)
                    blackCastleKingside = false;
            }
        }
        else if (type == KING)
        {
            if (whiteTurn)
            {
                whiteCastleQueenside = false;
                whiteCastleKingside = false;
            }
            else
            {
                blackCastleQueenside = false;
                blackCastleKingside = false;
            }
        }
    }

    public void updateCasteRook(int to)
    {
        int r1 = whiteTurn ? 7 : 63;
        int r2 = whiteTurn ? 0 : 56;
        if (to == r1)
        {
            if (whiteTurn)
                blackCastleKingside = false;
            else
                whiteCastleKingside = false;
        }
        else if (to == r2)
        {
            if (whiteTurn)
                blackCastleQueenside = false;
            else
                whiteCastleQueenside = false;
        }
    }

    public void castelMove(Move move)
    {
        int from = move.StartSquare;
        int to = move.TargetSquare;
        if (from < to)
        {
            tiles[to] = tiles[from];
            tiles[from] = 0;
            tiles[from + 1] = tiles[from + 3];
            tiles[from + 3] = 0;
            if (whiteTurn)
                WhitePieces[WhitePieces.FindIndex(i => i == from + 3)] = from + 1;
            else
                BlackPieces[BlackPieces.FindIndex(i => i == from + 3)] = from + 1;
        }
        else
        {

            //Debug.Log(from - 4);
            //Debug.Log(WhitePieces.Contains(from - 4));
            tiles[to] = tiles[from];
            tiles[from] = 0;
            tiles[from - 1] = tiles[from - 4];
            tiles[from - 4] = 0;
            if (whiteTurn)
                WhitePieces[WhitePieces.FindIndex(i => i == from - 4)] = from - 1;
            else
                BlackPieces[BlackPieces.FindIndex(i => i == from - 4)] = from - 1;
        }
    }
    #endregion

    #region OuterFuncs
    //!DEPRECATED
    public bool UnmakeMove(UIManager uiManager)
    {
        try
        {
            Move move = currGameState.PrevMove;
            int from = move.TargetSquare, to = move.StartSquare;
            int enPas = -1;
            bool reinstate = currGameState.PrevCapturedType() != 0;
            int reinstateindex = currGameState.PrevCapturedIndex();
            if (!UnmakeMoveInnerV2())
                return false;
            if (move.moveFlag == Move.Flag.EnPassantCapture)
                enPas = move.TargetSquare + ((whiteTurn) ? -8 : 8); ;
            switch (move.moveFlag)
            {
                case Move.Flag.EnPassantCapture:
                    uiManager.movePiece(from, to);
                    uiManager.reinstatePiece(enPas);
                    break;
                case Move.Flag.Castling:
                    uiManager.movePiece(from, to);
                    if (from < to)
                        uiManager.movePiece(from + 3, from + 1);
                    else
                        uiManager.movePiece(from - 4, from - 1);
                    break;
                case Move.Flag.PromoteToQueen:

                    uiManager.movePiece(from, to);
                    uiManager.pieceUI[to].setSprite(uiManager.piceSprites[uiManager.pieceTypeToSprite[tiles[to]]]);
                    break;
                case Move.Flag.PromoteToRook:
                    uiManager.movePiece(from, to);
                    uiManager.pieceUI[to].setSprite(uiManager.piceSprites[uiManager.pieceTypeToSprite[tiles[to]]]);
                    break;
                case Move.Flag.PromoteToBishop:
                    uiManager.movePiece(from, to);
                    uiManager.pieceUI[to].setSprite(uiManager.piceSprites[uiManager.pieceTypeToSprite[tiles[to]]]);
                    break;
                case Move.Flag.PromoteToKnight:
                    uiManager.movePiece(from, to);
                    uiManager.pieceUI[to].setSprite(uiManager.piceSprites[uiManager.pieceTypeToSprite[tiles[to]]]);
                    break;
                default:
                    uiManager.movePiece(from, to);
                    break;
            }
            if (reinstate)
                uiManager.reinstatePiece(reinstateindex);
            string s = "Turn:" + (Turn + 1) + "\n" + "Color: " + (whiteTurn ? "White" : "Black") + "\n" + "Check: " + (Check ? (WhiteInCheck ? "White" : "Black") : "None");
            uiManager.gameText.text = s;
            //lastMove = move;

            return true;
        }
        catch (Exception _ex)
        {
            Debug.Log("EXCEPTION DURING MoveInnerUI, ex:" + _ex.ToString());
            return false;
        }
    }
    public bool UnmakeMove()
    {
        return UnmakeMoveInnerV2();
    }

    private bool revertCasteling(int from, int to)
    {
        try
        {

            if (from < to)
            {
                tiles[from] = tiles[to];
                tiles[to] = 0;
                tiles[from + 3] = tiles[from + 1];
                tiles[from + 1] = 0;
                if (whiteTurn)
                    WhitePieces[WhitePieces.FindIndex(i => i == from + 1)] = from + 3;
                else
                    BlackPieces[BlackPieces.FindIndex(i => i == from + 1)] = from + 3;
            }
            else
            {

                //Debug.Log(from - 4);
                //Debug.Log(WhitePieces.Contains(from - 4));
                tiles[from] = tiles[to];
                tiles[to] = 0;
                tiles[from - 4] = tiles[from - 1];
                tiles[from - 1] = 0;
                if (whiteTurn)
                    WhitePieces[WhitePieces.FindIndex(i => i == from - 1)] = from - 4;
                else
                    BlackPieces[BlackPieces.FindIndex(i => i == from - 1)] = from - 4;
            }
            return true;

        }
        catch (Exception ex)
        {

            Debug.LogError("FAIL During revertCasteling ex: " + ex);
            return false;
        }
    }

    public bool useMove(Move move, UIManager uiManager)
    {
        //if (!hasGeneratedMoves)
        //{
        //    Debug.LogError("NO NEW MOVES GENERATED");
        //    return false;
        //}
        return MoveInnerV2(move, uiManager);
    }

    public bool useMove(Move move)
    {
        //if (!hasGeneratedMoves)
        //{
        //    Debug.LogError("NO NEW MOVES GENERATED");
        //    return false;
        //}
        return MoveInnerV2(move);
    }

    public bool tryMove(int from, int to, UIManager uiManager)
    {

        if (!hasGeneratedMoves)
        {
            Debug.LogError("NO NEW MOVES GENERATED");
            return false;
        }
        foreach (Move move in Moves)
        {
            if (move.TargetSquare == to && move.StartSquare == from)
            {
                return MoveInnerV2(move, uiManager);
            }
        }
        return false;
    }

    public bool tryMove(int from, int to)
    {
        if (!hasGeneratedMoves)
        {
            Debug.LogError("NO NEW MOVES GENERATED");
            return false;
        }


        foreach (Move move in Moves)
        {
            if (move.TargetSquare == to)
            {
                return MoveInnerV2(move);
            }
        }
        return false;
    }
    #endregion

    #region GetStateInfo
    public bool isCheckMate()
    {
        return Moves.Count == 0 && (whiteTurn ? WhiteInCheck : BlackInCheck);
    }

    public bool isDraw()
    {
        return fiftyCount == 50;
    }

    public bool isStalemate()
    {
        return Moves.Count == 0 && (whiteTurn ? !WhiteInCheck : !BlackInCheck);
    }

    public Move getMove(int from, int to)
    {
        Move move1 = new Move(-1, -1, -1);
        if (!hasGeneratedMoves)
        {
            Debug.LogError("NO MOVES GENERATED BEFORE getMove");
            return move1;
        }
        foreach (Move move in Moves)
            if (move.TargetSquare == to && move.TargetSquare == from)
                return move;
        return move1;
    }

    public bool containsMove(Move move)
    {
        if (!hasGeneratedMoves)
        {
            Debug.LogError("NO MOVES GENERATED BEFORE getMove");
            return false;
        }
        if (move.TargetSquare >= 0 && move.StartSquare >= 0 && move.moveFlag >= 0)
        {
            foreach (Move m in Moves)
                if (move.Equals(m))
                    return true;
        }
        return false;
    }

    // public bool isPinned(int pos)
    // {
    //     int piece = tiles[pos];
    //     if (IsWhite(piece)) return PinnedMapWhite[pos][0] != -1;
    //     else if (IsBlack(piece)) return PinnedMapBlack[pos][0] != -1;
    //     else return false;
    // }
    #endregion

    #region Refresh
    //TODO OPTIMIZE 
    //*


    public void refreshMoveMap()
    {
        MoveMap = moveGenerator.sortMovesBasedOnPosition();
    }

    public void refreshPieces()
    {
        whitePieces = new List<int>();
        blackPieces = new List<int>();
        for (int ii = 0; ii < 64; ii++)
        {
            if (Piece.IsWhite(tiles[ii]))
                whitePieces.Add(ii);
            else if (Piece.IsBlack(tiles[ii]))
                blackPieces.Add(ii);
        }
    }

    private void debugPrintMoves()
    {
        if (!hasGeneratedMoves)
        {
            Debug.LogError("NO MOVES GENERATED BEFORE debugPrintMoves");
            return;
        }
        StringBuilder stringBuilder = new StringBuilder();
        foreach (Move move in Moves)
            stringBuilder.Append(MoveTest.MoveStringRepresentation(move) + "\n");
        Debug.Log("Turn: " + (turn - 1) + "Number of moves: " + Moves.Count);
        Debug.Log(stringBuilder.ToString());
    }
    #endregion

    #region NewMethods
    public void LoadPosition()
    {
        Initialize();

        // Load pieces into board array and piece lists
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
        {
            int piece = tiles[squareIndex];

            if (piece != Piece.NONE)
            {
                int pieceType = Piece.PieceType(piece);
                int pieceColourIndex = (Piece.IsColour(piece, Piece.WHITE)) ? WhiteIndex : BlackIndex;
                if (Piece.IsSlidingPiece(piece))
                {
                    if (pieceType == Piece.QUEEN)
                    {
                        queens[pieceColourIndex].AddPieceAtSquare(squareIndex);
                    }
                    else if (pieceType == Piece.ROOK)
                    {
                        rooks[pieceColourIndex].AddPieceAtSquare(squareIndex);
                    }
                    else if (pieceType == Piece.BISHOP)
                    {
                        bishops[pieceColourIndex].AddPieceAtSquare(squareIndex);
                    }
                }
                else if (pieceType == Piece.KNIGHT)
                {
                    knights[pieceColourIndex].AddPieceAtSquare(squareIndex);
                }
                else if (pieceType == Piece.PAWN)
                {
                    pawns[pieceColourIndex].AddPieceAtSquare(squareIndex);
                }
                else if (pieceType == Piece.KING)
                {
                    KingSquares[pieceColourIndex] = squareIndex;
                }
            }
        }

        // Initialize zobrist key
        //ZobristKey = Zobrist.CalculateZobristKey(this);
    }
    //TODO USE THIS and replace OnStart
    private void Initialize()
    {
        moveGenerator = new MoveUtills();
        BlackCap = new bool[64];
        WhiteCap = new bool[64];
        KingSquares = new int[2];
        ColorIndex = whiteTurn ? 0 : 1;
        //ColorToMove = whiteTurn ? WHITE : BLACK;
        gameStateHistory = new Stack<GameState>();
        ZobristKey = 0;
        HashHistory = new Stack<ulong>();
        knights = new PieceTable[] { new PieceTable(10), new PieceTable(10) };
        pawns = new PieceTable[] { new PieceTable(8), new PieceTable(8) };
        rooks = new PieceTable[] { new PieceTable(10), new PieceTable(10) };
        bishops = new PieceTable[] { new PieceTable(10), new PieceTable(10) };
        queens = new PieceTable[] { new PieceTable(9), new PieceTable(9) };
        PieceTable emptyList = new PieceTable(0);
        allPieceTables = new PieceTable[] {
            emptyList,
            emptyList,
            pawns[WhiteIndex],
            knights[WhiteIndex],
            emptyList,
            bishops[WhiteIndex],
            rooks[WhiteIndex],
            queens[WhiteIndex],
            emptyList,
            emptyList,
            pawns[BlackIndex],
            knights[BlackIndex],
            emptyList,
            bishops[BlackIndex],
            rooks[BlackIndex],
            queens[BlackIndex],
            };
    }

    PieceTable GetPieceTable(int pieceType, int colourIndex)
    {
        if (pieceType == 0 || pieceType == 1 || pieceType == 4 || pieceType == 8 || pieceType == 9 || pieceType == 12 || pieceType < 0 || pieceType > 15)
            throw new ArgumentException("Illegal argument: {pieceType = " + pieceType + "}");
        return allPieceTables[colourIndex * 8 + pieceType];
    }

    public void generateNewMoves()
    {
        if (hasGeneratedMoves)
            return;
        moveGenerator.Generate(this);
        if (whiteTurn)
        {
            WhiteInCheck = moveGenerator.InCheck;
            BlackCap = BoardUtills.BitBoardToBoolArray(moveGenerator.CurrentAttackMap);
        }
        else
        {
            BlackInCheck = moveGenerator.InCheck;
            WhiteCap = BoardUtills.BitBoardToBoolArray(moveGenerator.CurrentAttackMap);
        }
        refreshMoveMap();
        hasGeneratedMoves = true;
    }
    #endregion
}
