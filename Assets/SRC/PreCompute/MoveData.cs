using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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



    static MoveData()
    {
        numSquaresToEdge = new int[64][];
        knightMoves = new byte[64];
        kingMoves = new byte[64];
        knightMovesMap = new int[64][];
        kingMovesMap = new int[64][];
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
        {

            int y = squareIndex / 8;
            int x = squareIndex - y * 8;
            numSquaresToEdge[squareIndex] = genSquaresToEdge(squareIndex);
            knightMoves[squareIndex] = genKnightMoves(squareIndex);
            kingMoves[squareIndex] = genKingMoves(squareIndex);
            knightMovesMap[squareIndex] = genKnightMovesMap(squareIndex);
            kingMovesMap[squareIndex] = genKingMovesMap(squareIndex);


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

