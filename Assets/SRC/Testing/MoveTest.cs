using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Utills;

public readonly struct PerftCheckResult
{
    public readonly long nTotalNodes;
    public readonly long totalTime;
    public readonly long[] result;
    public readonly long[] actual;
    public readonly string fenStartPosition;
    public PerftCheckResult(
    long nTotalNodes,
    long totalTime,
    long[] result,
    long[] actual,
    string fenStartPosition
    )
    {
        this.nTotalNodes = nTotalNodes;
        this.totalTime = totalTime;
        this.result = result;
        this.actual = actual;
        this.fenStartPosition = fenStartPosition;
    }
}

public class MoveTest
{
    //* INPUT HERE TO COMPARE
    public static List<string> stockfishResult = new List<string>(new string[]{
"c6c5: 1", "a7a6: 1", "b7b6: 1", "f7f6: 1", "g7g6: 1", "h7h6: 1", "a7a5: 1",
 "b7b5: 1", "f7f5: 1", "g7g5: 1", "h7h5: 1", "f2d1: 1", "f2h1: 1", "f2d3: 1",
  "f2h3: 1", "f2e4: 1", "f2g4: 1", "b8a6: 1", "b8d7: 1", "e7a3: 1", "e7b4: 1",
   "e7h4: 1", "e7c5: 1", "e7g5: 1", "e7d6: 1", "e7f6: 1", "c8d7: 1", "h8g8: 1",
    "d8a5: 1", "d8b6: 1", "d8c7: 1", "d8d7: 1", "d8e8: 1", "f8g8: 1"         });
    public static string[] PositionRepresentation = {
    "a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1",
    "a2", "b2", "c2", "d2", "e2", "f2", "g2", "h2",
    "a3", "b3", "c3", "d3", "e3", "f3", "g3", "h3",
    "a4", "b4", "c4", "d4", "e4", "f4", "g4", "h4",
    "a5", "b5", "c5", "d5", "e5", "f5", "g5", "h5",
    "a6", "b6", "c6", "d6", "e6", "f6", "g6", "h6",
    "a7", "b7", "c7", "d7", "e7", "f7", "g7", "h7",
    "a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8"};

    public static readonly string[] TestFenKnown = {
        "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
        "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - ",
        "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ",
        "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1",
        "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8  ",
        "r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10 ",
        "r3k2r/Pppp1ppp/1b3nbN/nPB5/B1P1P3/q4N2/Pp1P2PP/R2Q1RK1 b kq - 1 1"         //Custom
    };
    public static readonly long[][] TestNodesKnown =
    {new long[]{ 20, 400, 8902, 197281, 4865609, 119060324, 3195901860, 84998978956},
    new long[]{ 48, 2039, 97862, 4085603, 193690690, 8031647685},
    new long[]{ 14, 191, 2812, 43238, 674624, 11030083, 178633661, 3009794393},
    new long[]{ 6, 264, 9467, 422333, 15833292, 706045033},
    new long[]{ 44, 1486, 62379, 2103487, 89941194},
    new long[]{ 46, 2079, 89890, 3894594, 164075551, 6923051137, 287188994746, 11923589843526, 490154852788714},
    new long[]{ -1, -1, -1, -1, -1, -1, -1, -1, -1}};
    public static string[] TestFen = new string[]{
        "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
        "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8  ",
        "rnbqkbnr/pppppppp/8/8/7P/8/PPPPPPP1/RNBQKBNR b - - 0 1",
        "rnbqkbnr/pppppppp/8/8/8/P7/1PPPPPPP/RNBQKBNR b KQkq - 0 1",
        "rnbqkbnr/ppppppp1/8/7p/8/P7/1PPPPPPP/RNBQKBNR w KQkq h5 0 2",
        "rnbqkbnr/ppppppp1/7p/8/8/P7/RPPPPPPP/1NBQKBNR b Kkq - 1 2",
        "rnbqkbnr/pppppppp/8/8/8/4P3/PPPP1PPP/RNBQKBNR b KQkq - 0 1",
        "r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10 ",
        "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQ1RK1 b KQ - 1 8"
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


    public static PerftCheckResult PerftCheck(int fen, int ply)
    {
        if (fen >= TestFenKnown.Length)
            fen = 0;
        string fenPos = TestFenKnown[fen];

        long nTotalNodes = 0;
        long[] result = new long[ply];
        long[] actual = TestNodesKnown[fen];
        int timeId = TimeUtills.Instance.startMeasurement();
        Board board = new Board(fenPos);

        PerftCheckRecursive(board, 0, ply, result);
        long totalTime = TimeUtills.Instance.stopMeasurementMillis(timeId);
        for (int ii = 0; ii < ply; ii++)
            nTotalNodes += result[ii];
        return new PerftCheckResult(nTotalNodes, totalTime, result, actual, fenPos);
    }
    public static void PerftCheckRecursive(Board board, int currentPly, int plyDepth, long[] nMoves)
    {
        nMoves[currentPly] += board.Moves.Count;
        if (currentPly == plyDepth - 1)
            return;
        foreach (Move nextMove in board.Moves)
        {
            Board newBoard = board.Clone();
            if (!newBoard.useMove(nextMove))
            { Debug.Log("FAIL"); }
            PerftCheckRecursive(newBoard, currentPly + 1, plyDepth, nMoves);

        }
    }


    public static PerftCheckResult PerftCheckV2(int fen, int ply)
    {
        if (fen >= TestFenKnown.Length)
            fen = 0;
        string fenPos = TestFenKnown[fen];

        long nTotalNodes = 0;
        long[] result = new long[ply];
        long[] actual = TestNodesKnown[fen];
        int timeId = TimeUtills.Instance.startMeasurement();
        Board board = new Board(fenPos);

        PerftCheckRecursiveV2(board, 0, ply, result);
        long totalTime = TimeUtills.Instance.stopMeasurementMillis(timeId);
        for (int ii = 0; ii < ply; ii++)
            nTotalNodes += result[ii];
        return new PerftCheckResult(nTotalNodes, totalTime, result, actual, fenPos);
    }
    public static void PerftCheckRecursiveV2(Board board, int currentPly, int plyDepth, long[] nMoves)
    {
        board.generateNewMoves();
        nMoves[currentPly] += board.Moves.Count;
        if (currentPly == plyDepth - 1)
            return;
        foreach (Move nextMove in board.Moves)
        {
            board.generateNewMoves();
            if (!board.useMove(nextMove))
            { Debug.Log("FAIL MAKE"); }
            PerftCheckRecursiveV2(board, currentPly + 1, plyDepth, nMoves);
            if (!board.UnmakeMove())
            { Debug.Log("FAIL UNMAKE"); }

        }
    }

    public static List<string> PerftDebug(int fen, int ply, string[] refResult)
    {
        List<string> oList = new List<string>();
        Board board = new Board(TestFenKnown[fen]);
        long[] nMoves = new long[ply];
        PerftDebugRecursive(board, 0, ply, nMoves, oList, "");
        List<string> refList = refResult.ToList();
        oList.Sort();
        refList.Sort();
        for (int ii = 0; ii < oList.Count; ii++)//Math.Max(outputList.Count, stockfishResult.Count); i++)
            if (refList.Count > ii)
            {
                string str = oList[ii] + " " + refList[ii];
                oList[ii] = str;
            }

        return oList;
    }
    static private void PerftDebugRecursive(Board board, int currentPly, int plyDepth, long[] nMoves, List<string> oList, string move)
    {
        long before = 0;
        nMoves[currentPly] += board.Moves.Count;
        if (currentPly == plyDepth - 1)
        {
            if (currentPly == 1)
                oList.Add(move + ": " + board.Moves.Count + ", SF: ");
            return;
        }
        if (board.Moves.Count == 0)
        {
            Debug.Log("CM: " + move);
            Debug.Log(board.boardToFEN());
        }
        for (int i = currentPly; i < plyDepth; i++)
        {
            before += nMoves[i];
        }

        foreach (Move nextMove in board.Moves)
        {
            Board newBoard = board.Clone();
            newBoard.useMove(nextMove);
            PerftDebugRecursive(newBoard, currentPly + 1, plyDepth, nMoves, oList, move + MoveStringRepresentation(nextMove));

        }
        long delta = 0;
        for (int i = currentPly; i < plyDepth; i++)
        {
            delta += nMoves[i];
        }
        delta -= before;
        if (currentPly == 1)
            oList.Add(move + ": " + delta + ", SF: ");
    }


    public static List<string> PerftDebugV2(int fen, int ply, string[] refResult)
    {
        List<string> oList = new List<string>();
        Board board = new Board(TestFenKnown[fen]);
        long[] nMoves = new long[ply];
        PerftDebugRecursiveV2(board, 0, ply, nMoves, oList, "");
        List<string> refList = refResult.ToList();
        oList.Sort();
        refList.Sort();
        for (int ii = 0; ii < oList.Count; ii++)//Math.Max(outputList.Count, stockfishResult.Count); i++)
        {
            string str = "";
            if (refList.Count > ii)
            {
                str = oList[ii] + " " + refList[ii];
            }
            else
                str = oList[ii];
            oList[ii] = str;
        }

        oList.Add("TOTAL: " + nMoves[ply - 1]);

        return oList;
    }
    static private void PerftDebugRecursiveV2(Board board, int currentPly, int plyDepth, long[] nMoves, List<string> oList, string move)
    {
        long before = 0;
        board.generateNewMoves();
        nMoves[currentPly] += board.Moves.Count;
        if (currentPly == plyDepth - 1)
        {
            if (currentPly <= 1)
            {
                Debug.Log(board.Moves.Count);
                oList.Add(move + ": " + board.Moves.Count + ", SF: ");
            }

            return;
        }
        if (board.Moves.Count == 0)
        {
            Debug.Log("CM: " + move);
            Debug.Log(board.boardToFEN());
        }
        for (int i = currentPly; i < plyDepth; i++)
        {
            before += nMoves[i];
        }

        foreach (Move nextMove in board.Moves)
        {
            board.generateNewMoves();
            if (!board.useMove(nextMove))
                Debug.Log("FAIL MAKE");
            PerftDebugRecursiveV2(board, currentPly + 1, plyDepth, nMoves, oList, move + MoveStringRepresentation(nextMove));
            if (!board.UnmakeMove())
                Debug.Log("FAIL UNMAKE");

        }
        long delta = 0;
        for (int i = currentPly; i < plyDepth; i++)
        {
            delta += nMoves[i];
        }
        delta -= before;
        if (currentPly == 1)
            oList.Add(move + ": " + delta + ", SF: ");
    }
}