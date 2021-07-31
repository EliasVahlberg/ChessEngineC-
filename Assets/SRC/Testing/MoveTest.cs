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
"b2b3: 46",
"g2g3: 46",
"h2h3: 46",
"a3a4: 46",
"d3d4: 45",
"b2b4: 45",
"h2h4: 46",
"c3b1: 46",
"c3d1: 46",
"c3a2: 46",
"c3a4: 46",
"c3b5: 46",
"c3d5: 45",
"f3e1: 47",
"f3d2: 47",
"f3d4: 46",
"f3h4: 47",
"f3e5: 50",
"c4a2: 46",
"c4b3: 46",
"c4b5: 46",
"c4d5: 45",
"c4a6: 47",
"c4e6: 45",
"c4f7: 4",
"g5c1: 47",
"g5d2: 47",
"g5e3: 46",
"g5f4: 48",
"g5h4: 47",
"g5f6: 44",
"g5h6: 46",
"a1b1: 46",
"a1c1: 46",
"a1d1: 46",
"a1e1: 46",
"a1a2: 46",
"f1b1: 46",
"f1c1: 46",
"f1d1: 46",
"f1e1: 46",
"e2d1: 46",
"e2e1: 46",
"e2d2: 46",
"e2e3: 45",
"g1h1: 46"
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
        "rnbqkbnr/pppppppp/8/8/7P/8/PPPPPPP1/RNBQKBNR b - - 0 1",
        "rnbqkbnr/pppppppp/8/8/8/P7/1PPPPPPP/RNBQKBNR b KQkq - 0 1",
        "rnbqkbnr/ppppppp1/8/7p/8/P7/1PPPPPPP/RNBQKBNR w KQkq h5 0 2",
        "rnbqkbnr/ppppppp1/7p/8/8/P7/RPPPPPPP/1NBQKBNR b Kkq - 1 2",
        "rnbqkbnr/pppppppp/8/8/8/4P3/PPPP1PPP/RNBQKBNR b KQkq - 0 1",
        "r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10 "
        };
    static List<List<string>> leafList = new List<List<string>>();
    static List<string> outputList;
    static int leafListCounter;
    public static int STANDARD_PLY = 4;
    static FileStream fs;
    static StreamWriter sw;
    //TODO maby fix for mac and Linux
    public static string logFilePath = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\ChessPerftLogs\\MoveTestLog.txt";
    public static long[] StandardMoveTest(int fen)
    {
        return StandardMoveTest(fen, STANDARD_PLY, false);
    }
    public static long[] StandardMoveTest(int fen, int ply, bool listMoves)
    {
        try
        {

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
        if (board.Moves.Count == 0)
        {
            Debug.Log("CM: " + move);
            Debug.Log(board.boardToFEN());
        }
        long before = 0;
        if (currentPly == plyDepth - 1)
        {
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
            Debug.Log(move + " -> " + MoveStringRepresentation(nextMove));
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
        if (currentPly == 1)
            writeResultSync(delta, move, outputList);

    }


    public static void writeResultSync(long nMoves, string move, List<string> outputList)
    {
        string str = (move + ": " + nMoves);
        outputList.Add(str);
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






    public static long[] StandardMoveTestNoLog(int fen)
    {
        return StandardMoveTest(fen, STANDARD_PLY, false);
    }
    public static long[] StandardMoveTestNoLog(int fen, int ply)
    {
        Board board = new Board(TestFen[fen]);
        long[] nMoves = new long[ply];
        standardMoveTestRecursiveNoLog(board, 0, ply, nMoves);
        return nMoves;
    }
    public static void standardMoveTestRecursiveNoLog(Board board, int currentPly, int plyDepth, long[] nMoves)
    {
        nMoves[currentPly] += board.Moves.Count;
        if (currentPly == plyDepth - 1)
            return;
        foreach (Move nextMove in board.Moves)
        {
            Board newBoard = board.Clone();
            newBoard.useMove(nextMove);

            standardMoveTestRecursiveNoLog(newBoard, currentPly + 1, plyDepth, nMoves);

        }
    }
}