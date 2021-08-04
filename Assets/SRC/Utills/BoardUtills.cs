using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


}
