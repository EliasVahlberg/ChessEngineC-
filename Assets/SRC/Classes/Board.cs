using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Piece;

public class Board
{
    public int[] tiles;

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

            }
            return whitePieces;
        }
    }

    private int lastGeneratedPinnedBlack = -1;
    private int[][] pinnedMapBlack = new int[64][];
    private int[][] PinnedMapBlack
    {
        get
        {
            if (lastGeneratedPinnedBlack != turn || turn == 0)
            {
                MoveLegalityUtills.genPinnedMap(this, BLACK, pinnedMapBlack);
                lastGeneratedPinnedBlack = turn;
                //GENPIN
                Debug.Log("GENPINB");
            }
            return pinnedMapBlack;
        }
    }

    private int lastGeneratedPinnedWhite = -1;
    private int[][] pinnedMapWhite = new int[64][];
    public int[][] PinnedMapWhite
    {
        get
        {
            if (lastGeneratedPinnedWhite != turn || turn == 0)
            {
                MoveLegalityUtills.genPinnedMap(this, WHITE, pinnedMapWhite);
                lastGeneratedPinnedWhite = turn;
            }
            return pinnedMapWhite;
        }
    }


    //private List<int[]> pinnedBlack;
    //public List<int[]> PinnedBlack
    //{
    //    get
    //    {
    //        if (lastGeneratedPinnedBlack != turn)
    //        {

    //            pinnedBlack = MoveLegalityUtills.checkPinned(this, BLACK);
    //            lastGeneratedPinnedBlack = turn;
    //        }
    //        return pinnedBlack;
    //    }
    //}


    //private List<int[]> pinnedWhite;
    //public List<int[]> PinnedWhite
    //{
    //    get
    //    {
    //        if (lastGeneratedPinnedWhite != turn)
    //        {
    //            pinnedWhite = MoveLegalityUtills.checkPinned(this, WHITE);
    //            lastGeneratedPinnedWhite = turn;
    //        }
    //        return pinnedWhite;
    //    }
    //}

    private int lastGeneratedWhiteCaptureable = -1;
    private bool[] whiteCaptureable = new bool[64];
    private List<int>[] whiteCapturableMapList = new List<int>[64];
    //* is Somehow 0% called much less (Never not equals turn)
    public bool[] WhiteCap
    {
        get
        {
            if (lastGeneratedWhiteCaptureable != turn || turn == 0)
            {
                whiteCaptureable = MoveLegalityUtills.updateCapturable(this, true);
                lastGeneratedWhiteCaptureable = turn;
            }
            return whiteCaptureable;
        }
        set { whiteCaptureable = value; }
    }
    // public List<int>[] WhiteCapMapList
    // {
    //     get
    //     {
    //         if (lastGeneratedWhiteCaptureable != turn)
    //         {
    //             MoveUtills.updateCapturable(this, ((whiteTurn) ? KinglessMoves : KinglessMovesOp));
    //             lastGeneratedWhiteCaptureable = turn;
    //         }
    //         return whiteCapturableMapList;
    //     }
    //     set { whiteCapturableMapList = value; }
    // }


    private int lastGeneratedBlackCaptureable = -1;
    private bool[] blackCaptureable = new bool[64];
    private List<int>[] blackCapturableMapList = new List<int>[64];
    //TODO OPTIMIZE 1.2% (Self = 0%)
    public bool[] BlackCap
    {
        get
        {
            if (lastGeneratedBlackCaptureable != turn || turn == 0)
            {
                blackCaptureable = MoveLegalityUtills.updateCapturable(this, false); //TODO OPTIMIZE 1.2%
                lastGeneratedBlackCaptureable = turn;
            }
            return blackCaptureable;
        }
        set { blackCaptureable = value; }
    }

    // public List<int>[] BlackCapMapList
    // {
    //     get
    //     {
    //         if (lastGeneratedBlackCaptureable != turn)
    //         {
    //             MoveUtills.updateCapturable(this, ((whiteTurn) ? KinglessMoves : KinglessMovesOp));
    //             lastGeneratedBlackCaptureable = turn;
    //         }
    //         return blackCapturableMapList;
    //     }
    //     set { blackCapturableMapList = value; }
    // }

    public int whiteKingPos;
    public int blackKingPos;

    private List<Move> moves = new List<Move>();
    private int lastTurnGenerated = -1;
    public List<Move> Moves
    {
        get
        {
            if (lastTurnGenerated != turn || turn == 0)
            {
                moves = MoveUtills.GenerateMoves(this);
                lastTurnGenerated = turn;
            }
            return moves;
        }
    }

    // private int lastTurnGeneratedKLMoves = -1;
    // private List<Move> kinglessMoves;
    // public List<Move> KinglessMoves
    // {
    //     get
    //     {
    //         if (lastTurnGeneratedKLMoves != turn)
    //         {
    //             kinglessMoves = MoveUtills.GenerateMovesExceptKing(this);
    //             lastTurnGeneratedKLMoves = turn;
    //         }
    //         return kinglessMoves;
    //     }
    // }


    // private int lastTurnGeneratedKLMovesOp = -1;
    // private List<Move> kinglessMovesOp;
    // public List<Move> KinglessMovesOp
    // {
    //     get
    //     {
    //         if (lastTurnGeneratedKLMovesOp != turn)
    //         {
    //             whiteTurn = !whiteTurn;
    //             kinglessMoves = MoveUtills.GenerateMovesExceptKing(this);
    //             lastTurnGeneratedKLMoves = turn;
    //             whiteTurn = !whiteTurn;
    //         }
    //         return kinglessMovesOp;
    //     }
    // }

    private int lastTurnGeneratedMoveMap = -1;
    private List<Move>[] moveMap;
    public List<Move>[] MoveMap
    {
        get
        {
            if (lastTurnGeneratedMoveMap != turn || turn == 0)
            {
                moveMap = MoveUtills.sortMovesBasedOnPosition(Moves);
                lastTurnGeneratedMoveMap = turn;
            }
            return moveMap;
        }
        set { moveMap = value; }
    }
    private List<Move> oponentMoves;
    private int lastTurnGeneratedOpo = -1;
    public List<Move> OponentMoves
    {
        get
        {
            if (lastTurnGeneratedOpo != turn || turn == 0)
            {
                whiteTurn = !whiteTurn;
                moves = MoveUtills.GenerateMoves(this);
                whiteTurn = !whiteTurn;
                lastTurnGeneratedOpo = turn;
            }
            return moves;
        }
    }


    private int turn = 0;

    public int Turn { get => turn; set => turn = value; }
    public bool WhiteInCheck { get => BlackCap[whiteKingPos]; }
    public bool BlackInCheck { get => WhiteCap[blackKingPos]; }
    public bool whiteCastleKingside;
    public bool whiteCastleQueenside;
    public bool blackCastleKingside;
    public bool blackCastleQueenside;

    public bool whiteTurn;
    public int enPassantAble = -1;
    public int fiftyCount = 1;
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
        get { return (whiteTurn) ? BlackCap[whiteKingPos] : WhiteCap[blackKingPos]; }
    }


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


    public Board()
    {
        tiles = new int[64];
        for (int ii = 0; ii < 64; ii++)
        {
            pinnedMapBlack[ii] = new int[3] { -1, -1, -1 };
            pinnedMapWhite[ii] = new int[3] { -1, -1, -1 };
        }

    }

    public Board(string fen)
    {
        tiles = new int[64];
        FENUtills.GameStateInfo gameStateInfo = FENUtills.generatePiecePositions(fen);
        tiles = gameStateInfo.squareGrid;
        whiteTurn = gameStateInfo.whiteTurn;
        whiteCastleKingside = gameStateInfo.whiteCastleKingside;
        whiteCastleQueenside = gameStateInfo.whiteCastleQueenside;
        blackCastleKingside = gameStateInfo.blackCastleKingside;
        blackCastleQueenside = gameStateInfo.blackCastleQueenside;
        turn = ((gameStateInfo.plyCount - 1) * 2);
        if (turn < 0) turn = 0;
        fiftyCount = gameStateInfo.fiftyCount;
        int i = 0;
        foreach (int piece in tiles)
        {
            if (Piece.IsType(piece, Piece.KING))
            {
                if (Piece.IsColour(piece, Piece.WHITE))
                    whiteKingPos = i;
                else
                    blackKingPos = i;

            }
            i++;
        }
        for (int ii = 0; ii < 64; ii++)
        {
            pinnedMapBlack[ii] = new int[3] { -1, -1, -1 };
            pinnedMapWhite[ii] = new int[3] { -1, -1, -1 };
        }
    }

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
    //!DEPRECATED (I think at leas)
    public Board copy()
    {
        Board copy = new Board();
        copy.tiles = new int[64];
        copy.whiteCaptureable = new bool[64];
        copy.blackCaptureable = new bool[64];
        Array.Copy(tiles, copy.tiles, 64);
        Array.Copy(whiteCaptureable, copy.whiteCaptureable, 64);
        Array.Copy(blackCaptureable, copy.blackCaptureable, 64);
        copy.blackCaptureable = blackCaptureable;
        copy.turn = turn;
        copy.whiteCastleKingside = whiteCastleKingside;
        copy.whiteCastleQueenside = whiteCastleQueenside;
        copy.blackCastleKingside = blackCastleKingside;
        copy.blackCastleQueenside = blackCastleQueenside;
        copy.whiteTurn = whiteTurn;
        copy.enPassantAble = enPassantAble;
        return copy;

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
        copy.lastTurnGenerated = -1;
        copy.lastTurnGeneratedMoveMap = -1;
        copy.lastTurnGeneratedOpo = -1;
        copy.lastGeneratedWhiteCaptureable = -1;
        copy.lastGeneratedBlackCaptureable = -1;
        copy.lastGeneratedPinnedWhite = -1;
        copy.lastGeneratedPinnedBlack = -1;
        if (whitePieces != null)
            copy.whitePieces = new List<int>(whitePieces);
        if (blackPieces != null)
            copy.blackPieces = new List<int>(blackPieces);
        refreshMoves();

        return copy;

    }

    private bool MoveInner(Move move, UIManager uiManager)
    {
        try
        {
            int from = move.StartSquare, to = move.TargetSquare;
            int enPas = -1;
            if (move.moveFlag == Move.Flag.EnPassantCapture)
                enPas = enPassantAble;
            if (!MoveInner(move))
                return false;
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

    private bool MoveInner(Move move)
    {
        try
        {
            lastMoveFiftyCount = fiftyCount;
            lastMoveEnPas = enPassantAble;
            lastMoveWhiteCastleKingside = whiteCastleKingside;
            lastMoveWhiteCastleQueenside = whiteCastleQueenside;
            lastMoveBlackCastleKingside = blackCastleKingside;
            lastMoveBlackCastleQueenside = blackCastleQueenside;
            int from = move.StartSquare;
            int to = move.TargetSquare;
            if (enPassantAble != -1 && move.moveFlag != Move.Flag.EnPassantCapture)
                enPassantAble = -1;
            if (tiles[to] == 0 && !IsType(tiles[from], PAWN))
                fiftyCount++;
            else
                fiftyCount = 0;
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
            if (move.moveFlag == Move.Flag.EnPassantCapture)
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
                    enPassantAble = to;
                    tiles[to] = tiles[from];
                    tiles[from] = 0;
                    break;
                case Move.Flag.EnPassantCapture:
                    tiles[enPassantAble] = 0;

                    tiles[to] = tiles[from];
                    tiles[from] = 0;
                    enPassantAble = -1;
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
            debugPrintMoves();
            return true;
        }
        catch (Exception _ex)
        {
            Debug.Log("EXCEPTION DURING MoveInner, ex:" + _ex.ToString());
            return false;
        }
    }

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
                tiles[enPassantAble] = lastMoveCaptured;

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

    private bool revertCasteling(int from, int to)
    {
        return false;
    }
    public bool useMove(Move move, UIManager uiManager)
    {
        return MoveInner(move, uiManager);
    }

    public bool useMove(Move move)
    {
        return MoveInner(move);
    }

    public bool tryMove(int from, int to, UIManager uiManager)
    {

        List<Move> moves = MoveUtills.generateMovesForThisSquare(from, this);
        foreach (Move move in moves)
        {
            if (move.TargetSquare == to)
            {
                return MoveInner(move, uiManager);
            }
        }
        return false;
    }

    public bool tryMove(int from, int to)
    {

        List<Move> moves = MoveUtills.generateMovesForThisSquare(from, this);
        foreach (Move move in moves)
        {
            if (move.TargetSquare == to)
            {
                return MoveInner(move);
            }
        }
        return false;
    }

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
        List<Move> moves = MoveUtills.generateMovesForThisSquare(from, this);
        Move move1 = new Move(-1, -1, -1);
        foreach (Move move in moves)
            if (move.TargetSquare == to)
                return move;
        return move1;
    }

    public bool containsMove(Move move)
    {
        if (move.TargetSquare >= 0 && move.StartSquare >= 0 && move.moveFlag >= 0)
        {
            List<Move> moves = MoveUtills.generateMovesForThisSquare(move.StartSquare, this);
            foreach (Move m in moves)
                if (move.Equals(m))
                    return true;
        }
        return false;
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

    //uiManager.movePiece(from, to);
    //if (from < to)
    //uiManager.movePiece(from + 3, from + 1);
    //else
    //uiManager.movePiece(from - 4, from - 1);

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

            if (whiteTurn)
                WhitePieces[WhitePieces.FindIndex(i => i == from - 4)] = from - 1;
            else
                BlackPieces[BlackPieces.FindIndex(i => i == from - 4)] = from - 1;
        }
    }

    public bool isPinned(int pos)
    {
        int piece = tiles[pos];
        if (IsWhite(piece)) return PinnedMapWhite[pos][0] != -1;
        else if (IsBlack(piece)) return PinnedMapBlack[pos][0] != -1;
        else return false;
    }
    //TODO Fix for each
    //TODO OPTIMIZE 
    //*
    public int[] Pin(int pos)
    {
        int piece = tiles[pos];
        if (IsWhite(piece))
        {
            if (lastGeneratedPinnedWhite != turn)
            {
                MoveLegalityUtills.genPinnedMap(this, WHITE, pinnedMapWhite);
                lastGeneratedPinnedWhite = turn;
            }
            if (PinnedMapWhite[pos][0] != -1)
                return PinnedMapWhite[pos];
            //foreach (int[] pinnedPosDir in PinnedWhite)
            //    if (pinnedPosDir[0] == pos)
            //        return pinnedPosDir;
            return null;
        }
        else if (IsBlack(piece))
        {
            if (pos == 29)
                Debug.Log("STILLPINN: " + lastGeneratedPinnedBlack + "," + lastGeneratedPinnedWhite + "," + turn);

            if (lastGeneratedPinnedBlack != turn)
            {
                MoveLegalityUtills.genPinnedMap(this, BLACK, pinnedMapBlack);
                lastGeneratedPinnedBlack = turn;
            }
            if (PinnedMapBlack[pos][0] != -1)
                return PinnedMapBlack[pos];
            return null;
        }
        else
            return null;
    }

    public void onStart()
    {
        blackCaptureable = MoveLegalityUtills.updateCapturable(this, false);
        MoveLegalityUtills.updateCapturable(this, true);
        MoveLegalityUtills.genPinnedMap(this, BLACK, pinnedMapBlack);
        MoveLegalityUtills.genPinnedMap(this, BLACK, pinnedMapWhite);
        //pinnedBlack = MoveLegalityUtills.checkPinned(this, BLACK);
        //pinnedWhite = MoveLegalityUtills.checkPinned(this, WHITE);

    }
    public void refreshMoves()
    {
        moves = MoveUtills.GenerateMoves(this);
        lastTurnGenerated = turn;
    }
    public void refreshMoveMap()
    {
        moveMap = MoveUtills.sortMovesBasedOnPosition(Moves);
        lastTurnGeneratedMoveMap = turn;
    }
    private void debugPrintMoves()
    {
        foreach (Move move in moves)
            Debug.Log(MoveTest.MoveStringRepresentation(move));
    }
}
