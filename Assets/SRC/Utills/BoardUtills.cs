using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    @File BoardUtills.cs
    @author Elias Vahlberg 
    @Date 2021-07
*/
public class BoardUtills
{
    public struct Coord : IComparable<Coord>
    {
        public readonly int fileIndex;
        public readonly int rankIndex;

        public Coord(int fileIndex, int rankIndex)
        {
            this.fileIndex = fileIndex;
            this.rankIndex = rankIndex;
        }

        public bool IsLightSquare()
        {
            return (fileIndex + rankIndex) % 2 != 0;
        }

        public int CompareTo(Coord other)
        {
            return (fileIndex == other.fileIndex && rankIndex == other.rankIndex) ? 0 : 1;
        }
    }

    public const string fileNames = "abcdefgh";
    public const string rankNames = "12345678";

    public const int a1 = 0, a2 = 0 + 8, a3 = 0 + 16, a4 = 0 + 24, a5 = 0 + 32, a6 = 0 + 40, a7 = 0 + 48, a8 = 0 + 56;
    public const int b1 = 1, b2 = 1 + 8, b3 = 1 + 16, b4 = 1 + 24, b5 = 1 + 32, b6 = 1 + 40, b7 = 1 + 48, b8 = 1 + 56;
    public const int c1 = 2, c2 = 2 + 8, c3 = 2 + 16, c4 = 2 + 24, c5 = 2 + 32, c6 = 2 + 40, c7 = 2 + 48, c8 = 2 + 56;
    public const int d1 = 3, d2 = 3 + 8, d3 = 3 + 16, d4 = 3 + 24, d5 = 3 + 32, d6 = 3 + 40, d7 = 3 + 48, d8 = 3 + 56;
    public const int e1 = 4, e2 = 4 + 8, e3 = 4 + 16, e4 = 4 + 24, e5 = 4 + 32, e6 = 4 + 40, e7 = 4 + 48, e8 = 4 + 56;
    public const int f1 = 5, f2 = 5 + 8, f3 = 5 + 16, f4 = 5 + 24, f5 = 5 + 32, f6 = 5 + 40, f7 = 5 + 48, f8 = 5 + 56;
    public const int g1 = 6, g2 = 6 + 8, g3 = 6 + 16, g4 = 6 + 24, g5 = 6 + 32, g6 = 6 + 40, g7 = 6 + 48, g8 = 6 + 56;
    public const int h1 = 7, h2 = 7 + 8, h3 = 7 + 16, h4 = 7 + 24, h5 = 7 + 32, h6 = 7 + 40, h7 = 7 + 48, h8 = 7 + 56;

    public static int IndexFromCoord(int fileIndex, int rankIndex)
    {
        return rankIndex * 8 + fileIndex;
    }
    public static int IndexFromString(string s)
    {
        if (s.Length < 2)
            return -1;
        try
        {
            int file = 0;
            foreach (char c in fileNames)
            {
                if (c == s[0])
                    break;
                file++;
            }
            int rank = int.Parse(s[1] + "");
            Debug.Log("EP: " + (file + (rank - 1) * 8));
            return file + (rank - 1) * 8;
        }
        catch (System.Exception)
        {
            Debug.Log("EPFAIL");
            return -1;
        }

    }
    public static string stringFromIndex(int index)
    {
        int rank = index / 8;
        int file = index - rank * 8;
        return "" + fileNames[file] + rankNames[rank];

    }
    public static int Rank(int pos)
    {
        return pos / 8;
    }
    public static int File(int pos)
    {
        return pos % 8;
    }
    public static bool ContainsTile(ulong bitboard, int tile)
    {
        return ((bitboard >> tile) & 1) != 0;
    }
    public static bool[] BitBoardToBoolArray(ulong bitboard)
    {
        bool[] arr = new bool[64];
        for (int ii = 0; ii < 64; ii++)
        {
            arr[ii] = ((bitboard >> ii) & 1) != 0;
        }
        return arr;
    }

}
