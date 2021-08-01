using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ChessAI;
using UnityEngine;
using Utills;

namespace Testing
{

    public readonly struct PreformanceTestResult
    {
        //TODO maby fix for mac and Linux
        public readonly long time;
        public readonly int moves;
        public readonly int wPoints;
        public readonly int bPoints;
        public readonly float avrageTimePerTurn;
        public readonly string lastMoveFen;
        public PreformanceTestResult(
        long time,
        int moves,
        int wPoints,
        int bPoints,
        float avrageTimePerTurn,
        string lastMoveFen)
        {
            this.time = time;
            this.moves = moves;
            this.wPoints = wPoints;
            this.bPoints = bPoints;
            this.avrageTimePerTurn = avrageTimePerTurn;
            this.lastMoveFen = lastMoveFen;
        }
    }
    public readonly struct ChessEnginePerftResult
    {
        public readonly int nTotalMoves;
        public readonly int itterations;
        public readonly long totalTime;
        public readonly float avrageTimePerItteration;
        public readonly int memoryUsed;
        public ChessEnginePerftResult(
        int nTotalMoves,
        int itterations,
        long totalTime,
        float avrageTimePerItteration,
        int memoryUsed)
        {
            this.nTotalMoves = nTotalMoves;
            this.itterations = itterations;
            this.totalTime = totalTime;
            this.avrageTimePerItteration = avrageTimePerItteration;
            this.memoryUsed = memoryUsed;
        }
    }
    public static class PreformanceTest
    {
        public static readonly string MoveSequenceFilePath = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\ChessPerftLogs\\MoveSequence.txt";

        private static string testFEN1 = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";//Position 5
        private static string testFEN2 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";//Start position
        public static PreformanceTestResult startAIMoveTest(IAIObject wAI, IAIObject bAI, string fen, int moves)
        {
            long time = 0;
            int nMoves = 0;
            int wPoints = 0;
            int bPoints = 0;
            float avrageTimePerTurn = 0f;
            string lastMoveFen = "";
            Board board = new Board(fen);
            Move move;
            int ii = 0;
            int measureID = TimeUtills.Instance.startMeasurement();
            for (ii = 0; ii < moves; ii++)
            {
                if (board.Moves.Count == 0)
                    break;
                move = board.whiteTurn ? wAI.SelectMove(board) : bAI.SelectMove(board);
                if (move.StartSquare == 0 && move.TargetSquare == 0)
                    break;
                board.useMove(move);
                if (board.lastMoveWasCapture)
                {
                    if (board.whiteTurn)
                        bPoints += Piece.PieceValueDictionary[Piece.PieceType(board.lastMoveCaptured)];
                    else
                        wPoints += Piece.PieceValueDictionary[Piece.PieceType(board.lastMoveCaptured)];
                }
            }
            time = TimeUtills.Instance.stopMeasurementMillis(measureID);
            nMoves = ii;
            avrageTimePerTurn = (float)time / nMoves;
            lastMoveFen = board.boardToFEN();
            return new PreformanceTestResult(
            time,
            nMoves,
            wPoints,
            bPoints,
            avrageTimePerTurn,
            lastMoveFen
            );
        }
        public static PreformanceTestResult startAIMoveTest(IAIObject wAI, IAIObject bAI, int moves)
        { return startAIMoveTest(wAI, bAI, testFEN1, moves); }


        public static ChessEnginePerftResult enginePreformanceTest(int numMoves, int itterations)
        {
            return enginePreformanceTest(numMoves, itterations, testFEN2);
        }

        public static ChessEnginePerftResult enginePreformanceTest(int numMoves, int itterations, string fen)
        {
            long totalTime = 0;
            int nItterations = 0;
            int nTotalMoves = 0;
            float avrageTimePerItteration = 0;
            int memoryUsed = 0;
            //UnityEngine.Debug.Log("Enginetest : {nMoves = " + numMoves + ", nItter = " + itterations + ",fen = " + fen + "}");
            GenerateMoveSequence(numMoves);
            //UnityEngine.Debug.Log("Enginetest : {nMoves = " + numMoves + ", nItter = " + itterations + ",fen = " + fen + "}");
            Move[] moves = GetMoveSequence();
            Board board;
            int measureID = TimeUtills.Instance.startMeasurement();
            Process currentProcess = Process.GetCurrentProcess();
            for (int kk = 0; kk < itterations; kk++)
            {
                board = new Board(fen);
                for (int ii = 0; ii < moves.Length; ii++)
                {
                    if (!board.useMove(moves[ii]) && (ii != moves.Length - 1))
                    {
                        UnityEngine.Debug.LogError("GenerateMoveSequence FAILED to generate any move after, " + ii + " moves. Fen : \n" + board.boardToFEN());
                    }
                    nTotalMoves++;
                }
                nItterations++;
            }
            long usedMemoryBytes = currentProcess.PrivateMemorySize64;
            memoryUsed = (int)usedMemoryBytes; // /1000000;
            totalTime = TimeUtills.Instance.stopMeasurementMillis(measureID);
            return new ChessEnginePerftResult(
            nTotalMoves,
            nItterations,
            totalTime,
            avrageTimePerItteration,
            memoryUsed);
        }

        public static void GenerateMoveSequence(int length)
        {
            GenerateMoveSequence(length, testFEN2);
        }

        public static void GenerateMoveSequence(int length, string fen)
        {
            Board board = new Board(fen);
            Move[] moveSequence = new Move[length];
            for (int ii = 0; ii < length; ii++)
            {
                if (board.Moves.Count != 0)
                {
                    Move move = board.Moves[0];
                    moveSequence[ii] = move;
                    board.useMove(move);
                }
                else
                {
                    UnityEngine.Debug.LogError("GenerateMoveSequence FAILED to generate any move after, " + ii + " moves. Fen : \n" + board.boardToFEN());
                    length = ii;
                    break;
                }
            }
            //UnityEngine.Debug.Log("Final fen: " + board.boardToFEN());
            using (FileStream fs = File.Open(MoveSequenceFilePath, FileMode.Truncate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    for (int ii = 0; ii < length; ii++)
                    {
                        sw.WriteLine(moveSequence[ii].MoveValue.ToString());
                    }
                }
            }
        }

        private static Move[] GetMoveSequence()
        {
            Move[] moveSequence = new Move[0];
            List<Move> moveSequenceList = new List<Move>();
            using (FileStream fs = File.Open(MoveSequenceFilePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sw = new StreamReader(fs))
                {
                    while (!sw.EndOfStream)
                    {
                        ushort val;
                        string str = sw.ReadLine();
                        if (ushort.TryParse(str, out val))
                        {
                            moveSequenceList.Add(new Move(val));
                        }
                        else
                        {
                            UnityEngine.Debug.LogError("FAIL IN GetMoveSequence :PARSEFAIL String={" + str + "}");
                        }
                    }
                }
            }
            return moveSequenceList.ToArray();
        }
    }
}