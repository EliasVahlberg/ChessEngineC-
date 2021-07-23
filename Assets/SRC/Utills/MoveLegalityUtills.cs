using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MoveData;
using static Piece;
public class MoveLegalityUtills
{
    public static bool IsLegal(int pos, int target, Board board)
    {
        int piece = board.tiles[pos];
        int color = Colour(piece);
        if (IsType(piece, KING))
        {
            return IsSafePosition(target, board, color);
        }
        else if (color == WHITE && board.WhiteInCheck || color == BLACK && board.BlackInCheck)
            return resolvesCheckByNonKingMove(board, pos, target);
        else
        {
            int[] pinnedPosDir = board.Pin(pos);
            if (pinnedPosDir != null)
            {
                Debug.Log(board.PinnedWhite.Count);
                Debug.Log("PINNNNN : " + pinnedPosDir[0]);
                int kingPos = color == WHITE ? board.whiteKingPos : board.blackKingPos;
                for (int dist = 1; dist <= pinnedPosDir[3]; dist++)
                {
                    int newPos = kingPos + directionOffsets[pinnedPosDir[1]] * dist;
                    if (newPos == target)
                        return true;
                }
                return false;
            }
            return true;
        }
    }
    public static bool IsSafePosition(int position, Board board)
    {
        return !(board.whiteTurn ? board.BlackCap[position] : board.WhiteCap[position]);
    }
    public static bool IsSafePosition(int position, Board board, int color)
    {
        return !(color == WHITE ? board.BlackCap[position] : board.WhiteCap[position]);
    }
    public static bool[] updateCapturable(Board board, bool checkWhite)
    {
        bool[] attacked = new bool[64];
        return getAttackedSquares(board, attacked, checkWhite ? WHITE : BLACK);
    }

    private static bool[] getAttackedSquares(Board board, bool[] attacked, int color)
    {
        List<int> pieces = MoveUtills.getPieces(board, color);
        foreach (int pos in pieces)
            getAttackedByThisSquare(board, attacked, pos);
        return attacked;

    }
    private static bool[] getAttackedByThisSquare(Board board, bool[] attacked, int pos)
    {
        if (IsSlidingPiece(board.tiles[pos]))
            getAttackedByThisSquareSliding(board, attacked, pos);
        else if (IsType(board.tiles[pos], KNIGHT))
            getAttackedByThisSquareKnight(board, attacked, pos);
        else if (IsType(board.tiles[pos], PAWN))
            getAttackedByThisSquarePawn(board, attacked, pos);
        else if (IsType(board.tiles[pos], KING))
            getAttackedByThisSquareKing(board, attacked, pos);
        return attacked;
    }
    private static bool[] getAttackedByThisSquareSliding(Board board, bool[] attacked, int pos)
    {
        int type = board.tiles[pos];
        int startDir = (IsType(type, BISHOP)) ? 4 : 0;
        int endDir = (IsType(type, ROOK)) ? 4 : 8;
        for (int direction = startDir; direction < endDir; direction++)
        {
            for (int n = 1; n <= numSquaresToEdge[pos][direction]; n++)
            {
                int target = pos + directionOffsets[direction] * n;
                attacked[target] = true;
                if (board.tiles[target] != 0)
                    break;
            }
        }
        return attacked;
    }
    private static bool[] getAttackedByThisSquareKnight(Board board, bool[] attacked, int pos)
    {
        int[] arr = getKnightMoves(pos);
        int friendlyCol = Colour(board.tiles[pos]);
        foreach (int target in arr)
            attacked[target] = true;
        return attacked;
    }
    private static bool[] getAttackedByThisSquarePawn(Board board, bool[] attacked, int pos)
    {
        int[] attackDir = IsWhite(board.tiles[pos]) ? pawnAttackDirections[0] : pawnAttackDirections[1];
        if (IsOnBoard(pos + attackDir[0]))
            if (((pos - 8 * (pos / 8)) != 0))
                attacked[pos + attackDir[0]] = true;
        if (IsOnBoard(pos + attackDir[1]))
            if (((pos - 8 * (pos / 8)) != 7))
                attacked[pos + attackDir[1]] = true;
        return attacked;
    }
    private static bool[] getAttackedByThisSquareKing(Board board, bool[] attacked, int pos)
    {
        for (int dir = 0; dir < 8; dir++)
            if (IsOnBoard(pos + directionOffsets[dir]))
                if (numSquaresToEdge[pos][dir] != 0)
                    attacked[pos + directionOffsets[dir]] = true;
        return attacked;
    }

    public static bool resolvesCheckByNonKingMove(Board board, int pos, int target)
    {
        //TODO Optimize
        int[] checkInfo = getCheckingPiece(board, Colour(board.tiles[pos]));
        bool isWhite = IsColour(board.tiles[pos], WHITE);
        if (checkInfo[0] == -1)
        {
            Debug.LogError("NONCHECK!");
            Debug.LogError(board.WhiteInCheck);
            Debug.LogError(Colour(board.tiles[pos]));
            return true;
        }
        if (checkInfo[1] == 9)
            return false;
        foreach (int[] pinnedPosDir in (isWhite) ? board.PinnedWhite : board.PinnedBlack)
            if (pinnedPosDir != null && pinnedPosDir[0] == pos)
                return false;

        if (checkInfo[1] == 8)
            return target == checkInfo[0];

        int kingPos = (IsWhite(board.tiles[pos])) ? board.whiteKingPos : board.blackKingPos;
        for (int dist = 1; dist <= checkInfo[2]; dist++)
        {
            int newPos = kingPos + directionOffsets[checkInfo[1]] * dist;
            if (newPos == target)
                return true;
            if (newPos == checkInfo[0])
                return false;
        }
        return false;
    }
    //{pos,dir,dist} dir = 8 = KNIGHT (and has dist =1)
    //{pos,dir,dist} dir = 9 = multiple check (can only move king)
    public static int[] getCheckingPiece(Board board, int color)
    {
        int position = (color == WHITE) ? board.whiteKingPos : board.blackKingPos;
        int opoCol = (color == WHITE) ? BLACK : WHITE;
        if ((color == WHITE) ? !board.WhiteInCheck : !board.BlackInCheck)
            return new int[] { -1, -1, -1 };
        int[] arr = new int[] { -1, -1, -1 }; ;
        for (int dir = 0; dir < 8; dir++)
        {
            //May cause issues idk
            for (int dist = 1; dist <= numSquaresToEdge[position][dir]; dist++)
            {
                int newPos = position + directionOffsets[dir] * dist;
                int piece = board.tiles[newPos];
                if (piece != 0)
                {
                    if (IsColour(piece, color))
                        break;
                    bool canCheck = (dist == 1 && IsType(piece, PAWN) && IsPawnAttackDirection(Colour(piece), dir));
                    canCheck |= (CanSlideInDirection(piece, dir));
                    if (canCheck)
                    {
                        if (arr[0] != -1)
                        {
                            Debug.Log("DoubleCheck");
                            return new int[] { 0, 9, 0 };
                        }
                        arr = new int[] { newPos, dir, dist };
                    }
                }
            }
        }
        foreach (int newPos in getKnightMoves(position))
            if (IsColour(board.tiles[newPos], opoCol) && IsType(board.tiles[newPos], KNIGHT))
            {
                if (arr[0] != -1)
                {
                    Debug.Log("DoubleCheck");
                    return new int[] { 0, 9, 0 };
                }
                return new int[] { newPos, 8, 1 };
            }
        if (arr[0] == -1)
            Debug.LogError("getCheckingPiece FAILED!");
        return arr;

    }
    //{pos,dir,dist,dist to attacker}
    public static List<int[]> checkPinned(Board board, int color)
    {
        List<int[]> pinnedList = new List<int[]>();
        int opoCol = color == WHITE ? BLACK : WHITE;
        int position = color == WHITE ? board.whiteKingPos : board.blackKingPos;
        bool[] canBeCaptured = color == WHITE ? board.BlackCap : board.WhiteCap;
        int pinPos = -1;
        int pinDist = -1;
        for (int dir = 0; dir < 8; dir++)
        {
            for (int dist = 1; dist <= numSquaresToEdge[position][dir]; dist++)
            {

                int newPos = position + directionOffsets[dir] * dist;
                int piece = board.tiles[newPos];
                if (piece != 0)
                {
                    if (pinPos != -1 && IsColour(piece, color))
                    { break; Debug.Log("SAMECOL"); }
                    else if (IsColour(piece, color) && canBeCaptured[newPos])
                    {
                        pinPos = newPos;
                        pinDist = dist;
                    }
                    else if (IsColour(piece, opoCol))
                    {
                        if (pinPos == -1)
                            break;
                        if (CanSlideInDirection(piece, dir))
                        {
                            pinnedList.Add(new int[] { pinPos, dir, pinDist, dist });
                            Debug.Log("PINNED:" + "{" + pinPos + ", " + dir + ", " + pinDist + ", " + dist + "}");
                        }
                        break;
                    }
                    else
                        break;
                }
            }
            pinPos = -1;
            pinDist = -1;
        }
        return pinnedList;

    }


}