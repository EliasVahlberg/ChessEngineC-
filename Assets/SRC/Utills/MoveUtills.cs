using System.Collections;
using System.Collections.Generic;
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

    //TODO Optimize
    public static List<Move> GenerateMoves(Board board)
    {
        List<Move> moves = new List<Move>();
        for (int pos = 0; pos < 64; pos++)
        {
            int piece = board.tiles[pos];
            if (IsColour(piece, board.ColorTurn))
            {
                if (IsSlidingPiece(piece))
                {
                    generateSlidingMoves(pos, piece, board).ForEach(move => moves.Add(move));
                }
                switch (PieceType(piece))
                {
                    case PAWN:
                        generatePawnMoves(pos, board).ForEach(move => moves.Add(move));
                        break;
                    case KNIGHT:
                        generateKnightMoves(pos, board).ForEach(move => moves.Add(move));
                        //moves.AddRange(generateKnightMoves(pos, board));
                        break;
                    case KING:
                        generateKingMoves(pos, board).ForEach(move => moves.Add(move));
                        break;

                    default: break;
                }
            }
        }
        return moves;
    }

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
                    generateSlidingMoves(pos, piece, board).ForEach(move => list.Add(move));
                }
                switch (PieceType(piece))
                {
                    case PAWN:
                        generatePawnMoves(pos, board).ForEach(move => list.Add(move));
                        break;
                    case KNIGHT:
                        generateKnightMoves(pos, board).ForEach(move => list.Add(move));
                        break;

                    default: break;
                }
            }
        }
        return list;
    }

    public static List<Move> generateSlidingMoves(int position, int type, Board board)
    {
        int startDir = (IsType(type, BISHOP)) ? 4 : 0;
        int endDir = (IsType(type, ROOK)) ? 4 : 8;
        List<Move> slidingMoveL = new List<Move>();
        int opoColor = board.whiteTurn ? BLACK : WHITE;
        for (int direction = startDir; direction < endDir; direction++)
        {
            for (int n = 0; n < numSquaresToEdge[position][direction]; n++)
            {
                int target = position + directionOffsets[direction] * (n + 1);
                int pieceOnTarget = board.tiles[target];
                if (IsColour(pieceOnTarget, board.ColorTurn)) //Friendly
                    break;
                if (MoveLegalityUtills.IsLegal(position, target, board))
                    slidingMoveL.Add(new Move(position, target));
                if (IsColour(pieceOnTarget, opoColor)) //Oponent
                    break;
            }
        }


        return slidingMoveL;
    }

    public static List<Move> generateKnightMoves(int position, Board board)
    {
        List<Move> knightMoveL = new List<Move>();
        int[] arr = getKnightMoves(position);
        foreach (int target in arr)
        {
            if (IsColour(board.tiles[target], board.ColorTurn))
                continue;
            if (MoveLegalityUtills.IsLegal(position, target, board))
                knightMoveL.Add(new Move(position, target));
        }
        return knightMoveL;
    }

    public static List<Move> generateKingMoves(int position, Board board)
    {
        List<Move> kingMoveL = new List<Move>();
        foreach (int target in getKingMoves(position))
            if (!IsColour(board.tiles[target], board.ColorTurn))
                if (MoveLegalityUtills.IsLegal(position, target, board))
                    kingMoveL.Add(new Move(position, target));

        kingMoveL.AddRange(generateCastelingMoves(position, board));
        return kingMoveL;
    }

    public static List<Move> generateCastelingMoves(int position, Board board)
    {
        List<Move> castelMoves = new List<Move>();
        if (board.whiteTurn)
        {
            bool whiteCastleKingside = board.whiteCastleKingside &&
            (board.tiles[position + 1] == 0) &&
            (board.tiles[position + 2] == 0) &&
            (IsType(board.tiles[position + 3], ROOK)) &&
            (IsColour(board.tiles[position + 3], WHITE)) &&
            (!board.WhiteInCheck) &&
            (isSafePosition(position + 2, board));

            bool whiteCastleQueenside = board.whiteCastleQueenside &&
            (board.tiles[position - 1] == 0) &&
            (board.tiles[position - 2] == 0) &&
            (board.tiles[position - 3] == 0) &&
            (IsType(board.tiles[position - 4], ROOK)) &&
            (IsColour(board.tiles[position - 4], WHITE)) &&
            (!board.WhiteInCheck) &&
            (isSafePosition(position - 2, board));

            if (whiteCastleKingside)
            {
                castelMoves.Add(new Move(position, position + 2, Move.Flag.Castling));
            }
            if (whiteCastleQueenside)
            {
                castelMoves.Add(new Move(position, position - 2, Move.Flag.Castling));
            }
        }
        else
        {
            bool blackCastleKingside = board.blackCastleKingside &&
            (board.tiles[position + 1] == 0) &&
            (board.tiles[position + 2] == 0) &&
            (IsType(board.tiles[position + 3], ROOK)) &&
            (IsColour(board.tiles[position + 3], BLACK)) &&
            (!board.BlackInCheck) &&
            (isSafePosition(position + 2, board));

            bool blackCastleQueenside = board.blackCastleQueenside &&
            (board.tiles[position - 1] == 0) &&
            (board.tiles[position - 2] == 0) &&
            (board.tiles[position - 3] == 0) &&
            (IsType(board.tiles[position - 4], ROOK)) &&
            (IsColour(board.tiles[position - 4], BLACK)) &&
            (!board.BlackInCheck) &&
            (isSafePosition(position - 2, board));

            if (blackCastleKingside)
            {
                castelMoves.Add(new Move(position, position + 2, Move.Flag.Castling));
            }
            if (blackCastleQueenside)
            {
                castelMoves.Add(new Move(position, position - 2, Move.Flag.Castling));
            }
        }
        return castelMoves;
    }

    public static List<Move> generatePawnMoves(int position, Board board)
    {
        List<Move> pawnMoveL = new List<Move>();
        int moveDir = (board.whiteTurn) ? directionOffsets[NORTH_I] : directionOffsets[SOUTH_I];
        int[] attackDir = (board.whiteTurn) ? pawnAttackDirections[0] : pawnAttackDirections[1];
        bool isInStarting = ((board.whiteTurn && (position / 8 == 1)) || ((!board.whiteTurn) && (position / 8 == 6)));

        int target = position + moveDir;
        bool willUpgrade = (target / 8 == 7 || target / 8 == 0);
        if ((board.tiles[target] == 0) && willUpgrade)
            pawnMoveL.AddRange(generatePawnUpgrade(position, target));
        else if (board.tiles[target] == 0)
        {
            if (MoveLegalityUtills.IsLegal(position, target, board))
                pawnMoveL.Add(new Move(position, target));

            if (isInStarting && board.tiles[(target += moveDir)] == 0)
                if (MoveLegalityUtills.IsLegal(position, target, board))
                    pawnMoveL.Add(new Move(position, target, Move.Flag.PawnTwoForward));
        }

        target = position + attackDir[0];
        if (IsOnBoard(target) && Colour(board.tiles[target]) == ((board.whiteTurn) ? BLACK : WHITE) && (position - 8 * (position / 8)) != 0)
        {
            if (MoveLegalityUtills.IsLegal(position, target, board))
            {
                if (willUpgrade)
                    pawnMoveL.AddRange(generatePawnUpgrade(position, target));
                else
                    pawnMoveL.Add(new Move(position, target));
            }
        }
        target = position + attackDir[1];
        if (IsOnBoard(target) && Colour(board.tiles[target]) == ((board.whiteTurn) ? BLACK : WHITE) && (position - 8 * (position / 8)) != 7)
        {
            if (MoveLegalityUtills.IsLegal(position, target, board))
            {
                if (willUpgrade)
                    pawnMoveL.AddRange(generatePawnUpgrade(position, target));
                else
                    pawnMoveL.Add(new Move(position, target));
            }
        }
        if (board.enPassantAble == position - 1 || board.enPassantAble == position + 1)
        {
            if (MoveLegalityUtills.IsLegal(position, target, board))
            {
                //TODO FIX
                pawnMoveL.Add(new Move(position, board.enPassantAble + moveDir, Move.Flag.EnPassantCapture));
            }
        }


        return pawnMoveL;
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

    public static List<Move> generateMovesForThisSquare(int pos, Board board)
    {
        List<Move> moveL;

        int piece = board.tiles[pos];
        if (IsColour(piece, board.ColorTurn))
        {
            if (IsSlidingPiece(piece))
            {
                moveL = generateSlidingMoves(pos, piece, board);
            }
            else
            {
                switch (PieceType(piece))
                {
                    case PAWN:
                        moveL = generatePawnMoves(pos, board);
                        break;
                    case KNIGHT:
                        moveL = generateKnightMoves(pos, board);
                        break;
                    case KING:
                        moveL = generateKingMoves(pos, board);
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

    //DEPRECATED 
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


    //DEPRECATED 
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

    public static List<int> getPieces(Board board, int color)
    {
        //TODO Optimize
        List<int> pieces = new List<int>();
        for (int pos = 0; pos < 64; pos++)
            if (IsColour(board.tiles[pos], color))
                pieces.Add(pos);
        return pieces;
    }
    public static List<int> getSlidingPieces(Board board, int color)
    {
        //TODO Optimize
        List<int> pieces = new List<int>();
        for (int pos = 0; pos < 64; pos++)
            if (IsColour(board.tiles[pos], color) && IsSlidingPiece(board.tiles[pos]))
                pieces.Add(pos);
        return pieces;
    }

}
