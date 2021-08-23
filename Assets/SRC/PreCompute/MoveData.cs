using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    @File MoveData.cs
    @author Elias Vahlberg 
    @Date 2021-07
    @Credit Sebastian Lague 
*/
public static class MoveData
{
    public const int NORTH_I = 0, SOUTH_I = 1, WEST_I = 2, EAST_I = 3, NORTHWEST_I = 4, SOUTHEAST_I = 5, NORTHEAST_I = 6, SOUTHWEST_I = 7;
    public static readonly int[] directionOffsets = { 8, -8, -1, 1, 7, -7, 9, -9 }; //(N, S, W, E, NW, SE, NE, SW)
    public static readonly int[] allKnightJumps = { 15, 17, -17, -15, 10, -6, 6, -10 };
    public static readonly int[][] pawnAttackDirections = {
            new int[] { 7, 9 },
            new int[] {-9 ,-7}
        }; //NE NW, SE, SW
    public static readonly Dictionary<int, int> offsetDirDictionary = new Dictionary<int, int>(){
        {8,NORTH_I},
        {-8,SOUTH_I},
        {-1,WEST_I},
        {1,EAST_I},
        {7,NORTHWEST_I},
        {-7,SOUTHEAST_I},
        {9,NORTHEAST_I},
        {-9,SOUTHWEST_I}
         };
    public static readonly Dictionary<int, int> offsetDirCounterpartDict = new Dictionary<int, int>(){
        {NORTH_I,SOUTH_I},
        {SOUTH_I,NORTH_I},
        {WEST_I,EAST_I},
        {EAST_I,WEST_I},
        {NORTHWEST_I,SOUTHEAST_I},
        {SOUTHEAST_I,NORTHWEST_I},
        {NORTHEAST_I,SOUTHWEST_I},
        {SOUTHWEST_I,NORTHEAST_I}
         };
    public static readonly int[][] numSquaresToEdge;
    public static readonly byte[] knightMoves;
    public static readonly byte[] kingMoves;
    public static readonly int[][] kingMovesMap;
    public static readonly int[][] knightMovesMap;


    public const int PawnDoubleForwardW = 0, PawnAttackLeftW = 1, PawnAttackRightW = 2;
    public const int PawnDoubleForwardB = 3, PawnAttackLeftB = 4, PawnAttackRightB = 5;
    public static readonly int[][] pawnMoveMap;

    public static readonly ulong[] kingAttackBitboards;
    public static readonly ulong[] knightAttackBitboards;
    public static readonly ulong[][] pawnAttackBitboards;

    public static readonly int[] directionLookup;



    static MoveData()
    {
        numSquaresToEdge = new int[64][];
        knightMoves = new byte[64];
        kingMoves = new byte[64];
        knightMovesMap = new int[64][];
        kingMovesMap = new int[64][];
        kingAttackBitboards = new ulong[64];
        knightAttackBitboards = new ulong[64];
        pawnAttackBitboards = new ulong[64][];
        pawnMoveMap = new int[64][];
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
        {

            int y = squareIndex / 8;
            int x = squareIndex - y * 8;
            numSquaresToEdge[squareIndex] = genSquaresToEdge(squareIndex);
            knightMoves[squareIndex] = genKnightMoves(squareIndex);
            kingMoves[squareIndex] = genKingMoves(squareIndex);
            knightMovesMap[squareIndex] = genKnightMovesMap(squareIndex);
            kingMovesMap[squareIndex] = genKingMovesMap(squareIndex);

            #region KnightBitBoard
            var legalKnightJumps = new List<byte>();
            ulong knightBitboard = 0;
            foreach (int knightJumpDelta in allKnightJumps)
            {
                int knightJumpSquare = squareIndex + knightJumpDelta;
                if (knightJumpSquare >= 0 && knightJumpSquare < 64)
                {
                    int knightSquareY = knightJumpSquare / 8;
                    int knightSquareX = knightJumpSquare - knightSquareY * 8;
                    // Ensure knight has moved max of 2 squares on x/y axis (to reject indices that have wrapped around side of board)
                    int maxCoordMoveDst = System.Math.Max(System.Math.Abs(x - knightSquareX), System.Math.Abs(y - knightSquareY));
                    if (maxCoordMoveDst == 2)
                    {
                        legalKnightJumps.Add((byte)knightJumpSquare);
                        knightBitboard |= 1ul << knightJumpSquare;
                    }
                }
            }
            knightAttackBitboards[squareIndex] = knightBitboard;
            #endregion

            #region KingBitBoard
            // Calculate all squares king can move to from current square (not including castling)
            var legalKingMoves = new List<byte>();
            foreach (int kingMoveDelta in directionOffsets)
            {
                int kingMoveSquare = squareIndex + kingMoveDelta;
                if (kingMoveSquare >= 0 && kingMoveSquare < 64)
                {
                    int kingSquareY = kingMoveSquare / 8;
                    int kingSquareX = kingMoveSquare - kingSquareY * 8;
                    // Ensure king has moved max of 1 square on x/y axis (to reject indices that have wrapped around side of board)
                    int maxCoordMoveDst = System.Math.Max(System.Math.Abs(x - kingSquareX), System.Math.Abs(y - kingSquareY));
                    if (maxCoordMoveDst == 1)
                    {
                        legalKingMoves.Add((byte)kingMoveSquare);
                        kingAttackBitboards[squareIndex] |= 1ul << kingMoveSquare;
                    }
                }
            }
            #endregion

            #region PawnBitBoard
            pawnAttackBitboards[squareIndex] = new ulong[2];
            if (x > 0)
            {
                if (y < 7)
                {
                    pawnAttackBitboards[squareIndex][Board.WhiteIndex] |= 1ul << (squareIndex + 7);
                }
                if (y > 0)
                {
                    pawnAttackBitboards[squareIndex][Board.BlackIndex] |= 1ul << (squareIndex - 9);
                }
            }
            if (x < 7)
            {
                if (y < 7)
                {
                    pawnAttackBitboards[squareIndex][Board.WhiteIndex] |= 1ul << (squareIndex + 9);
                }
                if (y > 0)
                {
                    pawnAttackBitboards[squareIndex][Board.BlackIndex] |= 1ul << (squareIndex - 7);
                }
            }
            #endregion

            #region PawnMoveMap
            pawnMoveMap[squareIndex] = new int[6];
            if (y == 1)
                pawnMoveMap[squareIndex][PawnDoubleForwardB] = (squareIndex + 16);
            if (y == 6)
                pawnMoveMap[squareIndex][PawnDoubleForwardB] = (squareIndex - 16);

            if (x > 0)
            {
                if (y < 7)
                    pawnMoveMap[squareIndex][PawnAttackLeftW] = (squareIndex + 7);
                else
                    pawnMoveMap[squareIndex][PawnAttackLeftW] = -1;

                if (y > 0)
                    pawnMoveMap[squareIndex][PawnAttackLeftB] = (squareIndex - 9);
                else
                    pawnMoveMap[squareIndex][PawnAttackLeftB] = -1;
            }
            if (x < 7)
            {
                if (y < 7)
                    pawnMoveMap[squareIndex][PawnAttackRightW] = (squareIndex + 9);
                else
                    pawnMoveMap[squareIndex][PawnAttackRightW] = -1;

                if (y > 0)
                    pawnMoveMap[squareIndex][PawnAttackRightB] = (squareIndex - 7);
                else
                    pawnMoveMap[squareIndex][PawnAttackRightB] = -1;
            }
            #endregion

            #region DirectionLookup
            directionLookup = new int[127];
            for (int i = 0; i < 127; i++)
            {
                int offset = i - 63;
                int absOffset = System.Math.Abs(offset);
                int absDir = 1;
                if (absOffset % 9 == 0)
                {
                    absDir = 9;
                }
                else if (absOffset % 8 == 0)
                {
                    absDir = 8;
                }
                else if (absOffset % 7 == 0)
                {
                    absDir = 7;
                }

                directionLookup[i] = absDir * System.Math.Sign(offset);
            }
            #endregion

        }
    }

    static int[] genSquaresToEdge(int squareIndex)
    {
        int y = squareIndex / 8;
        int x = squareIndex - y * 8;

        int north = 7 - y;
        int south = y;
        int west = x;
        int east = 7 - x;

        int[] squaresToEdge = new int[8];
        squaresToEdge = new int[8];
        squaresToEdge[0] = north;
        squaresToEdge[1] = south;
        squaresToEdge[2] = west;
        squaresToEdge[3] = east;
        squaresToEdge[4] = System.Math.Min(north, west);
        squaresToEdge[5] = System.Math.Min(south, east);
        squaresToEdge[6] = System.Math.Min(north, east);
        squaresToEdge[7] = System.Math.Min(south, west);
        return squaresToEdge;
    }

    static byte genKnightMoves(int squareIndex)
    {
        byte kMoves = 0;
        int y = squareIndex / 8;
        int x = squareIndex - y * 8;
        byte i = 0;
        foreach (int posDelta in allKnightJumps)
        {
            int pos = squareIndex + posDelta;
            if (IsOnBoard(pos))
            {
                int newY = pos / 8;
                int newX = pos - newY * 8;
                int maxDeltaXY = System.Math.Max(System.Math.Abs(x - newX), System.Math.Abs(y - newY));
                if (maxDeltaXY == 2)
                    kMoves += (byte)(1 << i);


            }
            i++;

        }
        byte kMovesByte = System.Convert.ToByte(kMoves);
        return kMoves;
    }

    static int[] genKnightMovesMap(int squareIndex)
    {
        List<int> kMoves = new List<int>();
        int y = squareIndex / 8;
        int x = squareIndex - y * 8;
        foreach (int posDelta in allKnightJumps)
        {
            int pos = squareIndex + posDelta;
            if (IsOnBoard(pos))
            {
                int newY = pos / 8;
                int newX = pos - newY * 8;
                int maxDeltaXY = System.Math.Max(System.Math.Abs(x - newX), System.Math.Abs(y - newY));
                if (maxDeltaXY == 2)
                    kMoves.Add((byte)pos);

            }

        }
        return kMoves.ToArray();
    }

    static byte genKingMoves(int squareIndex)
    {
        int kMoves = 0;
        int y = squareIndex / 8;
        int x = squareIndex - y * 8;
        int i = 0;
        foreach (int posDelta in directionOffsets)
        {
            int pos = squareIndex + posDelta;
            if (IsOnBoard(pos))
            {
                int newY = pos / 8;
                int newX = pos - newY * 8;
                int maxDeltaXY = System.Math.Max(System.Math.Abs(x - newX), System.Math.Abs(y - newY));
                if (maxDeltaXY == 1)
                    kMoves |= (1 << i);
            }
            i++;

        }
        byte kMovesByte = System.Convert.ToByte(kMoves);
        return kMovesByte;
    }

    static int[] genKingMovesMap(int squareIndex)
    {
        List<int> kMoves = new List<int>();
        int y = squareIndex / 8;
        int x = squareIndex - y * 8;
        int i = 0;
        foreach (int posDelta in directionOffsets)
        {
            int pos = squareIndex + posDelta;
            if (IsOnBoard(pos))
            {
                int newY = pos / 8;
                int newX = pos - newY * 8;
                int maxDeltaXY = System.Math.Max(System.Math.Abs(x - newX), System.Math.Abs(y - newY));
                if (maxDeltaXY == 1)
                    kMoves.Add(pos);
            }
            i++;

        }
        return kMoves.ToArray();
    }


    public static int[] getKnightMoves(int squareIndex)
    {
        return knightMovesMap[squareIndex];
    }

    public static int[] getKingMoves(int squareIndex)
    {
        return kingMovesMap[squareIndex];
    }

    public static bool IsOnBoard(int pos)
    { return pos >= 0 && pos < 64; }



}

