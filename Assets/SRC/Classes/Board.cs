using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Piece;

public class Board
{
    public int[] tiles;

    private int lastGeneratedPinnedBlack = -1;
    private List<int[]> pinnedBlack;
    public List<int[]> PinnedBlack
    {
        get
        {
            if (lastGeneratedPinnedBlack != turn)
            {

                pinnedBlack = MoveLegalityUtills.checkPinned(this, BLACK);
                lastGeneratedPinnedBlack = turn;
            }
            return pinnedBlack;
        }
    }

    private int lastGeneratedPinnedWhite = -1;
    private List<int[]> pinnedWhite;
    public List<int[]> PinnedWhite
    {
        get
        {
            if (lastGeneratedPinnedWhite != turn)
            {
                pinnedWhite = MoveLegalityUtills.checkPinned(this, WHITE);
                lastGeneratedPinnedWhite = turn;
            }
            return pinnedWhite;
        }
    }

    private int lastGeneratedWhiteCaptureable = -1;
    private bool[] whiteCaptureable = new bool[64];
    private List<int>[] whiteCapturableMapList = new List<int>[64];

    public bool[] WhiteCap
    {
        get
        {
            if (lastGeneratedWhiteCaptureable != turn)
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

    public bool[] BlackCap
    {
        get
        {
            if (lastGeneratedBlackCaptureable != turn)
            {
                blackCaptureable = MoveLegalityUtills.updateCapturable(this, false);
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
            if (lastTurnGenerated != turn)
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
            if (lastTurnGeneratedMoveMap != turn)
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
            if (lastTurnGeneratedOpo != turn)
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

    public Board()
    {
        tiles = new int[64];
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

        gameStateInfo.plyCount = fiftyCount;
        gameStateInfo.plyCount = Turn / 2;

        return FENUtills.generateFEN(gameStateInfo);
    }

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

    public bool tryMove(int from, int to, UIManager uiManager)
    {

        List<Move> moves = MoveUtills.generateMovesForThisSquare(from, this);
        foreach (Move move in moves)
        {
            if (move.TargetSquare == to)
            {
                if (enPassantAble != -1 && move.moveFlag != Move.Flag.EnPassantCapture)
                    enPassantAble = -1;
                if (tiles[to] == 0 && !IsType(tiles[from], PAWN))
                    fiftyCount++;
                else
                    fiftyCount = 0;

                switch (move.moveFlag)
                {
                    case Move.Flag.PawnTwoForward:
                        enPassantAble = to;
                        tiles[to] = tiles[from];
                        tiles[from] = 0;
                        uiManager.movePiece(from, to);
                        break;
                    case Move.Flag.EnPassantCapture:
                        tiles[enPassantAble] = 0;

                        tiles[to] = tiles[from];
                        tiles[from] = 0;
                        uiManager.movePiece(from, to);

                        uiManager.destroyPiece(enPassantAble);
                        enPassantAble = -1;
                        break;
                    case Move.Flag.Castling:
                        castelMove(move, uiManager);
                        break;
                    case Move.Flag.PromoteToQueen:
                        tiles[to] = QUEEN | (whiteTurn ? WHITE : BLACK);
                        tiles[from] = 0;
                        uiManager.movePiece(from, to);
                        uiManager.pieceUI[to].setSprite(uiManager.piceSprites[uiManager.pieceTypeToSprite[tiles[to]]]);
                        break;
                    case Move.Flag.PromoteToRook:
                        tiles[to] = ROOK | (whiteTurn ? WHITE : BLACK);
                        tiles[from] = 0;
                        uiManager.movePiece(from, to);
                        uiManager.pieceUI[to].setSprite(uiManager.piceSprites[uiManager.pieceTypeToSprite[tiles[to]]]);
                        break;
                    case Move.Flag.PromoteToBishop:
                        tiles[to] = BISHOP | (whiteTurn ? WHITE : BLACK);
                        tiles[from] = 0;
                        uiManager.movePiece(from, to);
                        uiManager.pieceUI[to].setSprite(uiManager.piceSprites[uiManager.pieceTypeToSprite[tiles[to]]]);
                        break;
                    case Move.Flag.PromoteToKnight:
                        tiles[to] = KNIGHT | (whiteTurn ? WHITE : BLACK);
                        tiles[from] = 0;
                        uiManager.movePiece(from, to);
                        uiManager.pieceUI[to].setSprite(uiManager.piceSprites[uiManager.pieceTypeToSprite[tiles[to]]]);
                        break;
                    default:
                        tiles[to] = tiles[from];
                        tiles[from] = 0;
                        uiManager.movePiece(from, to);
                        break;
                }

                updateCasteling(from, to);
                //MoveUtills.updateCapturable(this);
                if (IsType(tiles[to], KING))
                {
                    if (whiteTurn)
                        whiteKingPos = to;
                    else
                        blackKingPos = to;
                }
                Turn++;
                whiteTurn = !whiteTurn;
                string s = "Turn:" + (Turn + 1) + "\n" + "Color: " + (whiteTurn ? "White" : "Black") + "\n" + "Check: " + (Check ? (WhiteInCheck ? "White" : "Black") : "None");
                uiManager.gameText.text = s;
                isCheckMate();
                return true;
            }
        }
        return false;
    }
    public bool isCheckMate()
    {
        if (Moves.Count == 0)
            Debug.Log("CheckMate, winner :" + (whiteTurn ? "BLACK" : "WHITE"));
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
    public bool tryMove(int from, int to)
    {

        List<Move> moves = MoveUtills.generateMovesForThisSquare(from, this);
        foreach (Move move in moves)
        {
            if (move.TargetSquare == to)
            {
                if (move.moveFlag != Move.Flag.EnPassantCapture)
                    enPassantAble = -1;
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
                //MoveUtills.updateCapturable(this);
                if (IsType(tiles[to], KING))
                {
                    if (whiteTurn)
                        whiteKingPos = to;
                    else
                        blackKingPos = to;
                }
                Turn++;
                whiteTurn = !whiteTurn;
                return true;
            }
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

    public void castelMove(Move move, UIManager uiManager)
    {
        int from = move.StartSquare;
        int to = move.TargetSquare;
        if (from < to)
        {
            tiles[to] = tiles[from];
            tiles[from] = 0;
            uiManager.movePiece(from, to);
            tiles[from + 1] = tiles[from + 3];
            tiles[from + 3] = 0;
            uiManager.movePiece(from + 3, from + 1);
        }
        else
        {
            tiles[to] = tiles[from];
            tiles[from] = 0;
            uiManager.movePiece(from, to);
            tiles[from - 1] = tiles[from - 4];
            tiles[from - 4] = 0;
            uiManager.movePiece(from - 4, from - 1);
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
        }
        else
        {
            tiles[to] = tiles[from];
            tiles[from] = 0;
            tiles[from - 1] = tiles[from - 4];
            tiles[from - 4] = 0;
        }
    }

    public int[] Pin(int pos)
    {
        int piece = tiles[pos];
        if (IsWhite(piece))
        {
            foreach (int[] pinnedPosDir in PinnedWhite)
                if (pinnedPosDir[0] == pos)
                { Debug.Log("POS " + pos); return pinnedPosDir; }
            return null;
        }
        else if (IsBlack(piece))
        {

            foreach (int[] pinnedPosDir in PinnedBlack)
                if (pinnedPosDir[0] == pos)
                    return pinnedPosDir;
            return null;
        }
        else
            return null;
    }
    public void onStart()
    {
        blackCaptureable = MoveLegalityUtills.updateCapturable(this, false);
        MoveLegalityUtills.updateCapturable(this, true);
        pinnedBlack = MoveLegalityUtills.checkPinned(this, BLACK);
        pinnedWhite = MoveLegalityUtills.checkPinned(this, WHITE);
    }
    public void refreshMoveMap()
    {
        moveMap = MoveUtills.sortMovesBasedOnPosition(Moves);
        lastTurnGeneratedMoveMap = turn;
    }
}
