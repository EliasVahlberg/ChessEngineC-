using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MoveTest
{
    //* INPUT HERE TO COMPARE
    public static List<string> stockfishResult = new List<string>(new string[]{
        "a7a6: 21",
        "b7b6: 21",
        "c7c6: 21",
        "d7d6: 21",
        "e7e6: 21",
        "f7f6: 21",
        "g7g6: 21",
        "h7h6: 21",
        "a7a5: 21",
        "b7b5: 21",
        "c7c5: 21",
        "d7d5: 21",
        "e7e5: 21",
        "f7f5: 21",
        "g7g5: 22",
        "h7h5: 20",
        "b8a6: 21",
        "b8c6: 21",
        "g8f6: 21",
        "g8h6: 21"
        });
    public static string[] PositionRepresentation = {
    "a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1",
    "a2", "b2", "c2", "d2", "e2", "f2", "g2", "h2",
    "a3", "b3", "c3", "d3", "e3", "f3", "g3", "h3",
    "a4", "b4", "c4", "d4", "e4", "f4", "g4", "h4",
    "a5", "b5", "c5", "d5", "e5", "f5", "g5", "h5",
    "a6", "b6", "c6", "d6", "e6", "f6", "g6", "h6",
    "a7", "b7", "c7", "d7", "e7", "f7", "g7", "h7",
    "a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8"};
    public static string[] TestFen = new string[]{
        "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
        "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8  ",
        "rnbqkbnr/pppppppp/8/8/7P/8/PPPPPPP1/RNBQKBNR b - - 0 1",""
        };
    static List<List<string>> leafList = new List<List<string>>();
    static List<string> outputList;
    static int leafListCounter;
    public static int STANDARD_PLY = 4;
    static FileStream fs;
    static StreamWriter sw;

    public static string logFilePath = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\ChessPerftLogs\\MoveTestLog.txt";
    public static long[] StandardMoveTest(int fen)
    {
        return StandardMoveTest(fen, STANDARD_PLY, false);
    }
    public static long[] StandardMoveTest(int fen, int ply, bool listMoves)
    {
        try
        {

            Debug.Log(ply);
            //File.Create(logFilePath).Close();
            outputList = new List<string>();
            leafList = new List<List<string>>();
            Board board = new Board(TestFen[fen]);
            long[] nMoves = new long[ply];
            standardMoveTestRecursive(board, 0, ply, nMoves, "", outputList, listMoves);
            outputList.Sort();
            stockfishResult.Sort();
            fs = File.Open(logFilePath, FileMode.Truncate, FileAccess.Write);
            sw = new StreamWriter(fs);
            Debug.Log("COUNT :" + outputList.Count + ", " + stockfishResult.Count + ", " + leafList.Count);
            string str = " ";
            for (int i = 0; i < outputList.Count; i++)//Math.Max(outputList.Count, stockfishResult.Count); i++)
            {
                if (outputList.Count > i)
                {
                    str += outputList[i];
                    if (listMoves && leafList.Count > i)
                    {
                        for (int j = 0; j < leafList[i].Count; j++)
                        {
                            string leafStr = leafList[i][j];
                            sw.WriteLine(leafStr);
                        }
                    }
                }
                if (stockfishResult.Count > i)
                {
                    str += "," + stockfishResult[i];
                }
                Debug.Log(str);
                sw.WriteLine(str);
                str = " ";
            }
            sw.Close();
            fs.Close();
            return nMoves;

        }
        catch (System.Exception)
        {
            if (sw != null)
                sw.Close();
            if (fs != null && fs.CanWrite)
                fs.Close();
            throw;
        }

    }
    public static void standardMoveTestRecursive(Board board, int currentPly, int plyDepth, long[] nMoves, string move, List<string> outputList, bool listMoves)
    {
        nMoves[currentPly] += board.Moves.Count;
        long before = 0;
        if (currentPly == plyDepth - 1)
        {
            Debug.Log("Leaf :" + currentPly);
            if (listMoves)
            {
                foreach (Move nextMove in board.Moves)
                {
                    writeLeafNodesSync(move + "->" + MoveStringRepresentation(nextMove));
                }
            }
            if (currentPly == 1)
            {
                writeResultSync(board.Moves.Count, move, outputList);
            }
            return;
        }
        else
        {
            for (int i = currentPly; i < plyDepth; i++)
            {
                before += nMoves[i];
            }
        }
        foreach (Move nextMove in board.Moves)
        {
            Board newBoard = board.Clone();
            newBoard.useMove(nextMove);

            standardMoveTestRecursive(newBoard, currentPly + 1, plyDepth, nMoves, move + " -> " + MoveStringRepresentation(nextMove), outputList, listMoves);

        }
        if (currentPly == 1)
            leafListCounter++;
        long delta = 0;
        for (int i = currentPly; i < plyDepth; i++)
        {
            delta += nMoves[i];
        }
        delta -= before;
        if (currentPly != 0)
            writeResultSync(delta, move, outputList);

    }


    public static void writeResultSync(long nMoves, string move, List<string> outputList)
    {
        string str = (move + ": " + nMoves);
        outputList.Add(str);
        Debug.Log(str);
    }
    public static string MoveStringRepresentation(Move move)
    {
        return (PositionRepresentation[move.StartSquare] + PositionRepresentation[move.TargetSquare]);
    }
    public static void writeLeafNodesSync(string move)
    {
        string str = ("\t" + move + ": " + "LEAF");
        if (leafListCounter == leafList.Count)
            leafList.Add(new List<string>());
        if (leafList[leafListCounter] == null)
            leafList[leafListCounter] = new List<string>();
        leafList[leafListCounter].Add(str);
    }
}