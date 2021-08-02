using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MoveData;
using static Piece;

public class MoveUtills
{
    public List<Move> moves;
    public Board board;

    public MoveUtills(Board board)
    {
        this.board = board;
        moves = new List<Move>();
    }

    //TODO Optimize 96.8%
    //TODO REMOVE FOREACH, SELF = 9.6%
    public static List<Move> GenerateMoves(Board board)
    {
        List<Move> moves = new List<Move>();    //TODO Make sure it is Linked list
        for (int pos = 0; pos < 64; pos++)
        {
            int piece = board.tiles[pos];
            if (IsColour(piece, board.ColorTurn))
            {
                if (IsSlidingPiece(piece))
                {
                    generateSlidingMoves(pos, piece, board, moves);
                }
                switch (PieceType(piece))
                {
                    case PAWN:
                        generatePawnMoves(pos, board, moves);
                        break;
                    case KNIGHT:
                        generateKnightMoves(pos, board, moves);
                        //moves.AddRange(generateKnightMoves(pos, board));
                        break;
                    case KING:
                        generateKingMoves(pos, board, moves);
                        break;

                    default: break;
                }
            }
        }
        return moves;
    }

    //!DEPRECATED
    public static List<Move> GenerateMovesExceptKings(Board board)
    {
        Debug.Log("KingLess");
        List<Move> list = new List<Move>();
        for (int pos = 0; pos < 64; pos++)
        {
            int piece = board.tiles[pos];
            if (IsColour(piece, board.ColorTurn))
            {
                if (IsSlidingPiece(piece))
                {
                    generateSlidingMoves(pos, piece, board, list);
                }
                switch (PieceType(piece))
                {
                    case PAWN:
                        generatePawnMoves(pos, board, list);
                        break;
                    case KNIGHT:
                        generateKnightMoves(pos, board, list);
                        break;

                    default: break;
                }
            }
        }
        return list;
    }

    // TODO OPTIMIZE 46%
    //*
    public static void generateSlidingMoves(int position, int type, Board board, List<Move> moveList)
    {
        int startDir = (IsType(type, BISHOP)) ? 4 : 0;
        int endDir = (IsType(type, ROOK)) ? 4 : 8;
        int opoColor = board.whiteTurn ? BLACK : WHITE;
        for (int direction = startDir; direction < endDir; direction++)
        {
            for (int n = 0; n < numSquaresToEdge[position][direction]; n++) //TODO Optimize save pice pos
            {
                int target = position + directionOffsets[direction] * (n + 1);
                int pieceOnTarget = board.tiles[target];
                if (IsColour(pieceOnTarget, board.ColorTurn)) //Friendly //TODO OPTIMIZE Make var for color turn
                    break;
                if (MoveLegalityUtills.IsLegal(position, target, board)) //TODO OPTIMZE 32.6%
                    moveList.Add(new Move(position, target));   //TODO OPTIMZE 2.8%
                if (IsColour(pieceOnTarget, opoColor)) //Oponent    //TODO OPTIMIZE 1.5%
                    break;
            }
        }
    }

    //TODO OPTIMIZE 10%,
    //*
    public static void generateKnightMoves(int position, Board board, List<Move> moveList)
    {
        int[] arr = getKnightMoves(position);
        for (int ii = 0; ii < arr.Length; ii++)
        {
            int target = arr[ii];
            if (IsColour(board.tiles[target], board.ColorTurn))
                continue;
            if (MoveLegalityUtills.IsLegal(position, target, board)) //TODO OPTIMIZE 6.7%
                moveList.Add(new Move(position, target));
        }
    }

    //TODO OPTIMIZE 5.4%, SELF = 0.7%
    //*
    public static void generateKingMoves(int position, Board board, List<Move> moveList)
    {
        int[] arr = getKingMoves(position);
        for (int ii = 0; ii < arr.Length; ii++)
        {
            int target = arr[ii];
            if (!IsColour(board.tiles[target], board.ColorTurn))
                if (MoveLegalityUtills.IsLegal(position, target, board)) //TODO OPTIMIZE 2.1%
                    moveList.Add(new Move(position, target));
        }
        generateCastelingMoves(position, board, moveList);
    }

    //TODO OPTIMIZE 1.6% (Is kinda optimzed)
    //*
    public static void generateCastelingMoves(int position, Board board, List<Move> moveList)
    {
        bool isWhite = Colour(board.tiles[position]) == WHITE;
        int color = isWhite ? WHITE : BLACK;
        bool[] castleRights = isWhite ?
            new bool[] { board.whiteCastleKingside, board.whiteCastleQueenside } :
            new bool[] { board.blackCastleKingside, board.blackCastleQueenside };
        int[][] squaresBetween = isWhite ?
            new int[][] { new int[] { 5, 6 }, new int[] { 1, 2, 3 } } :
            new int[][] { new int[] { 61, 62 }, new int[] { 57, 58, 59 } };
        int[] rookPos = isWhite ?
            new int[] { 7, 0 } :
            new int[] { 63, 56 };

        bool cas1 =
        castleRights[0] &&
        board.tiles[squaresBetween[0][0]] == 0 &&
        board.tiles[squaresBetween[0][1]] == 0 &&
        (isSafePosition(squaresBetween[0][0], board)) &&
        (isSafePosition(squaresBetween[0][1], board)) &&
        (IsType(board.tiles[rookPos[0]], ROOK)) &&
        (IsColour(board.tiles[rookPos[0]], color)) &&
        (!(isWhite ? board.WhiteInCheck : board.BlackInCheck));
        //* NOTE "The king does not pass through a square that is attacked by an enemy piece."
        //* since the he does not pass through the third square it is ok
        bool cas2 =
        castleRights[1] &&
        board.tiles[squaresBetween[1][0]] == 0 &&
        board.tiles[squaresBetween[1][1]] == 0 &&
        board.tiles[squaresBetween[1][2]] == 0 &&
        (isSafePosition(squaresBetween[1][0], board)) &&
        (isSafePosition(squaresBetween[1][1], board)) &&
        (IsType(board.tiles[rookPos[1]], ROOK)) &&
        (IsColour(board.tiles[rookPos[1]], color)) &&
        (!(isWhite ? board.WhiteInCheck : board.BlackInCheck));

        if (cas1)
        {
            moveList.Add(new Move(position, squaresBetween[0][1], Move.Flag.Castling));
        }
        if (cas2)
        {
            moveList.Add(new Move(position, squaresBetween[1][1], Move.Flag.Castling));
        }

    }

    // TODO OPTIMIZE 17.6%, SELF = 1.8%
    //*
    // TODO OPTIMIZE 18.8%, SELF = 1.6%
    public static void generatePawnMoves(int position, Board board, List<Move> moveList)
    {
        int moveDir = (board.whiteTurn) ? directionOffsets[NORTH_I] : directionOffsets[SOUTH_I];
        int[] attackDir = (board.whiteTurn) ? pawnAttackDirections[0] : pawnAttackDirections[1];
        bool isInStarting = ((board.whiteTurn && (position / 8 == 1)) || ((!board.whiteTurn) && (position / 8 == 6)));

        int target = position + moveDir;
        bool willUpgrade = (target / 8 == 7 || target / 8 == 0);
        if ((board.tiles[target] == 0) && willUpgrade)
            moveList.AddRange(generatePawnUpgrade(position, target));
        else if (board.tiles[target] == 0)
        {

            if (MoveLegalityUtills.IsLegal(position, target, board))
                moveList.Add(new Move(position, target));

            if (isInStarting && board.tiles[(target += moveDir)] == 0)
                if (MoveLegalityUtills.IsLegal(position, target, board))
                    moveList.Add(new Move(position, target, Move.Flag.PawnTwoForward));
        }

        target = position + attackDir[0];
        if (IsOnBoard(target) && Colour(board.tiles[target]) == ((board.whiteTurn) ? BLACK : WHITE) && (position - 8 * (position / 8)) != 0)
        {
            if (MoveLegalityUtills.IsLegal(position, target, board))
            {
                if (willUpgrade)
                    moveList.AddRange(generatePawnUpgrade(position, target));
                else
                    moveList.Add(new Move(position, target));
            }
        }
        target = position + attackDir[1];
        if (IsOnBoard(target) && Colour(board.tiles[target]) == ((board.whiteTurn) ? BLACK : WHITE) && (position - 8 * (position / 8)) != 7)
        {
            if (MoveLegalityUtills.IsLegal(position, target, board))
            {
                if (willUpgrade)
                    moveList.AddRange(generatePawnUpgrade(position, target));
                else
                    moveList.Add(new Move(position, target));
            }
        }
        if ((board.enPassantAble == position - 1 && numSquaresToEdge[position][WEST_I] != 0) || (board.enPassantAble == position + 1 && numSquaresToEdge[position][EAST_I] != 0))
        {
            if (MoveLegalityUtills.IsLegal(position, target, board))
            {
                //TODO FIX
                //!FUCK THIS COMMENT
                //*Fixed now but
                //*Was previously :(board.enPassantAble == position - 1  || board.enPassantAble == position + 1)
                //! *MASSIVE BLUNDER*

                //*Oh shit here we go again...
                //*Enpassant bypasses Pin kinda (Position 3 ->e2e4->g4e3->Illegal CheckMate)
                //*This was previously just one line below this.... Fucking chess
                int col = board.whiteTurn ? WHITE : BLACK;
                int oCol = board.whiteTurn ? BLACK : WHITE;
                bool kingOnRank = false;
                bool rookOrQueenLeft = false;
                bool rookOrQueenRight = false;
                int nPiecesOnLeft = 0;
                int nPiecesOnRight = 0;

                for (int ii = 1; ii <= numSquaresToEdge[position][WEST_I]; ii++)
                {
                    if (board.tiles[position - ii] != 0)
                    {
                        int piece = board.tiles[position - ii];
                        if (IsType(piece, KING) && IsColour(piece, col))
                            kingOnRank = true;
                        else if (IsRookOrQueen(piece) && IsColour(piece, oCol))
                        { rookOrQueenLeft = true; break; }
                        else if (!rookOrQueenLeft)
                            nPiecesOnLeft++;

                    }
                }
                for (int ii = 1; ii <= numSquaresToEdge[position][EAST_I]; ii++)
                {
                    if (board.tiles[position + ii] != 0)
                    {
                        int piece = board.tiles[position + ii];
                        if (IsType(piece, KING) && IsColour(piece, col))
                            kingOnRank = true;
                        else if (IsRookOrQueen(piece) && IsColour(piece, oCol))
                        { rookOrQueenRight = true; break; }
                        else if (!rookOrQueenRight)
                            nPiecesOnRight++;

                    }
                }
                //Debug.Log(kingOnRank
                //+ ", " + rookOrQueenLeft
                //+ ", " + rookOrQueenRight
                //+ ", " + nPiecesOnLeft
                //+ ", " + nPiecesOnRight);
                if (kingOnRank && ((rookOrQueenLeft && (nPiecesOnLeft <= 1) || (rookOrQueenRight && (nPiecesOnRight <= 1)))))
                    return;
                else
                    moveList.Add(new Move(position, board.enPassantAble + moveDir, Move.Flag.EnPassantCapture));

            }
        }
    }

    private static List<Move> generatePawnUpgrade(int position, int target)
    {
        List<Move> upgradeMoves = new List<Move>();
        upgradeMoves.Add(new Move(position, target, Move.Flag.PromoteToQueen));
        upgradeMoves.Add(new Move(position, target, Move.Flag.PromoteToRook));
        upgradeMoves.Add(new Move(position, target, Move.Flag.PromoteToBishop));
        upgradeMoves.Add(new Move(position, target, Move.Flag.PromoteToKnight));
        return upgradeMoves;
    }

    //TODO Make this //!DEPRECATED 
    public static List<Move> generateMovesForThisSquare(int pos, Board board)
    {
        List<Move> moveL = new List<Move>();

        int piece = board.tiles[pos];
        if (IsColour(piece, board.ColorTurn))
        {
            if (IsSlidingPiece(piece))
            {
                generateSlidingMoves(pos, piece, board, moveL);
            }
            else
            {
                switch (PieceType(piece))
                {
                    case PAWN:
                        generatePawnMoves(pos, board, moveL);
                        break;
                    case KNIGHT:
                        generateKnightMoves(pos, board, moveL);
                        break;
                    case KING:
                        generateKingMoves(pos, board, moveL);
                        break;

                    default:
                        moveL = new List<Move>();
                        break;
                }
            }
        }
        else
        {
            moveL = new List<Move>();
        }
        return moveL;
    }

    //!DEPRECATED 
    private static bool isSafePosition(int position, Board board)
    {
        return !(board.whiteTurn ? board.BlackCap[position] : board.WhiteCap[position]);
    }

    private static bool selfCheck(Move move, Board board)
    {
        if (IsType(board.tiles[move.StartSquare], KING))
            return !isSafePosition(move.TargetSquare, board);
        return false;
    }


    //!DEPRECATED 
    public static void updateCapturable(Board board, List<Move> moveList)
    {
        if (moveList == null)
            return;
        bool checkWhite = IsWhite(board.tiles[moveList[0].StartSquare]);
        bool[] capturable = new bool[64];
        List<int>[] capturableMapList = new List<int>[64];
        foreach (Move move in moveList)
        {
            if (IsType(board.tiles[move.StartSquare], PAWN))
                continue;
            capturable[move.TargetSquare] = true;
            if (capturableMapList[move.TargetSquare] == null)
                capturableMapList[move.TargetSquare] = new List<int>();
            capturableMapList[move.TargetSquare].Add(move.StartSquare);
        }
        for (int p = 0; p < 64; p++)
        {
            if (board.tiles[p] != (PAWN | (checkWhite ? WHITE : BLACK)))
                continue;
            int p1, dir1;
            int p2, dir2;
            if (checkWhite)
            {
                p1 = p + MoveData.pawnAttackDirections[0][0];
                p2 = p + MoveData.pawnAttackDirections[0][1];
                dir1 = NORTHWEST_I;
                dir2 = NORTHEAST_I;
            }
            else
            {
                p1 = p + MoveData.pawnAttackDirections[1][0];
                p2 = p + MoveData.pawnAttackDirections[1][1];
                dir1 = SOUTHEAST_I;
                dir2 = SOUTHWEST_I;
            }

            if (p1 < 64 && p1 >= 0 && MoveData.numSquaresToEdge[p][dir1] != 0)
            {
                capturable[p1] = true;
                if (capturableMapList[p1] == null)
                    capturableMapList[p1] = new List<int>();
                capturableMapList[p1].Add(p);
            }
            if (p2 < 64 && p2 >= 0 && MoveData.numSquaresToEdge[p][dir2] != 0)
            {
                capturable[p2] = true;
                if (capturableMapList[p2] == null)
                    capturableMapList[p2] = new List<int>();
                capturableMapList[p2].Add(p);
            }
        }
        if (checkWhite)
        {
            board.WhiteCap = capturable;
            //board.WhiteCapMapList = capturableMapList;
        }
        else
        {

            board.BlackCap = capturable;
            //board.BlackCapMapList = capturableMapList;
        }
    }


    public static List<Move>[] sortMovesBasedOnPosition(List<Move> moves)
    {
        List<Move>[] moveGrid = new List<Move>[64];
        foreach (Move move in moves)
        {
            if (moveGrid[move.StartSquare] == null)
                moveGrid[move.StartSquare] = new List<Move>();
            moveGrid[move.StartSquare].Add(move);
        }
        return moveGrid;
    }
    //TODO OPTIMIZE 0.4%, SELF = 0.2%
    public static List<int> getPieces(Board board, int color)
    {
        return (color == WHITE) ? board.WhitePieces : board.BlackPieces;
        //TODO Optimize
        //List<int> pieces = new List<int>();
        //for (int pos = 0; pos < 64; pos++)
        //    if (IsColour(board.tiles[pos], color)) //TODO OPTIMIZE 0.1%
        //        pieces.Add(pos);                    ////TODO OPTIMIZE 0.1%
        //return pieces;
    }
    public static List<int> getSlidingPieces(Board board, int color)
    {
        //TODO Optimize
        List<int> allpieces = (color == WHITE) ? board.WhitePieces : board.BlackPieces;
        return allpieces.Where(n => IsSlidingPiece(board.tiles[n])).ToList<int>();
        //for (int pos = 0; pos < 64; pos++)
        //    if (IsColour(board.tiles[pos], color) && IsSlidingPiece(board.tiles[pos]))
        //        pieces.Add(pos);
        //return pieces;
    }

}
