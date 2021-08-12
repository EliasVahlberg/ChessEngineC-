
using System;
using SimpleChess;
using UnityEngine;

namespace ChessAI
{
    /*
    @File Search.cs
    @author Elias Vahlberg 
    @Date 2021-08
    @Credit Sebastian Lague 
    */
    public class Search
    {
        private const int NEG_INF = int.MinValue / 2;
        private const int POS_INF = int.MaxValue / 2 + 1;
        private const int DIRECT_MATE_SCORE = int.MaxValue / 2;
        private const int MINIMUM_MATE_SCORE = int.MaxValue / 4;
        private const int transpositionTableSize = 64000;
        private static readonly Move INVAL_MOVE = new Move(0);

        private Board board;
        private AISettings settings;
        private Evaluator evaluator;
        private MoveOrderer moveOrderer;
        public event System.Action<Move> searchStopper;

        private Move bestMove;
        private int bestVal;
        Move bestMoveCurrentIteration;
        int bestValCurrentIteration;
        int currentSearchDepth;
        bool abortSearch;
        SearchDiagnostics sD;
        #region SearchInfo


        private int nNodes;
        private int nQNodes;
        private int nPrunes;
        private int nTPos;

        System.Diagnostics.Stopwatch searchStopwatch;

        #endregion


        TranspositionTable tt;
        public Search(Board board, AISettings settings)
        {
            this.board = board;
            this.settings = settings;
            evaluator = new Evaluator();
            tt = new TranspositionTable(board, transpositionTableSize);
            moveOrderer = new MoveOrderer(tt);
            sD = new SearchDiagnostics();
            //invalidMove = Move.InvalidMove;
            //int s = TranspositionTable.Entry.GetSize();
            //Debug.Log ("TT entry: " + s + " bytes. Total size: " + ((s * transpositionTableSize) / 1000f) + " mb.");
        }

        public static bool IsMateScore(int score)
        {
            return Math.Abs(score) > MINIMUM_MATE_SCORE;
        }

        public void SearchRecurr()
        {
            Debug.Log("SearchStart");
            ResetSearchInfo();
            if (settings.clearTTEachMove)
                tt.Clear();

            currentSearchDepth = 0;
            abortSearch = false;
            sD = new SearchDiagnostics();
            if (settings.useIterativeDeepening)
            {
                int targetDepth = (settings.useFixedDepthSearch) ? settings.depth : int.MaxValue;

                for (int searchDepth = 1; searchDepth <= targetDepth; searchDepth++)
                {
                    SearchRecurrRecurr(searchDepth, searchDepth, NEG_INF, POS_INF);

                    if (abortSearch)
                    {
                        LogDebugInfo();
                        Debug.Log("ABORT");
                        break;
                    }
                    else
                    {
                        currentSearchDepth = searchDepth;
                        bestMove = bestMoveCurrentIteration;
                        bestVal = bestValCurrentIteration;

                        // Update diagnostics
                        sD.lastCompletedDepth = searchDepth;
                        sD.move = bestMove.ToString();
                        sD.eval = bestVal;
                        //sD.moveVal = board.boardToFEN();//Chess.PGNCreator.NotationFromMove (FenUtility.CurrentFen (board), bestMove);

                        // Exit search if found a mate
                        if (IsMateScore(bestVal)) //&& !settings.endlessSearchMode) 
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                Debug.Log("NON ITTERATIVE");
                SearchRecurrRecurr(settings.depth, settings.depth, NEG_INF, POS_INF);
                bestMove = bestMoveCurrentIteration;
                bestVal = bestValCurrentIteration;
            }
            searchStopper?.Invoke(bestMove);

        }



        private int SearchRecurrRecurr(int depth, int maxDepth, int alpha, int beta)
        {
            if (abortSearch)
            {
                return 0;
            }
            nNodes++;

            int ttVal = tt.LookupEvaluation(depth, maxDepth - depth, alpha, beta);
            if (ttVal != TranspositionTable.lookupFailed)
            {
                nTPos++;
                if (depth == maxDepth)
                {
                    bestMoveCurrentIteration = tt.GetStoredMove();
                    bestValCurrentIteration = tt.entries[tt.Index].value;
                }
                return ttVal;
            }

            if (depth == 0)
                return evaluator.Evaluate(board);

            moveOrderer.Order(board);

            if (board.Moves.Count == 0)
                return board.CurrentInCheck ? NEG_INF : 0;
            if (board.Moves.Count == 0 && board.CurrentInCheck)
                return 0;

            Move bestMoveThisPos = INVAL_MOVE;

            int evalType = TranspositionTable.UpperBound;


            foreach (Move move in board.Moves)
            {

                if (!board.useMove(move))
                    throw new ArgumentException("FAIL MAKE");

                int val = -SearchRecurrRecurr(depth - 1, maxDepth, -beta, -alpha);
                if (!board.UnmakeMove())
                    throw new ArgumentException("FAIL UNMAKE");
                if (val >= beta)
                {
                    //PRUNE
                    tt.StoreEvaluation(depth, maxDepth - depth, beta, TranspositionTable.LowerBound, move);
                    nPrunes++;
                    return beta;
                }
                if (val > alpha)
                {
                    evalType = TranspositionTable.Exact;
                    bestMoveThisPos = move;
                    alpha = val;
                    if (depth == maxDepth)
                    {
                        bestMoveCurrentIteration = move;
                        bestValCurrentIteration = val;
                    }
                }
            }


            tt.StoreEvaluation(depth, maxDepth - depth, beta, evalType, bestMoveThisPos);
            return alpha;
        }

        void ResetSearchInfo()
        {
            searchStopwatch = System.Diagnostics.Stopwatch.StartNew();
            nNodes = 0;
            nQNodes = 0;
            nPrunes = 0;
            nTPos = 0;
        }

        public void StoppSearch()
        {
            abortSearch = true;
        }

        void LogDebugInfo()
        {
            Debug.Log("str1");
            AnnounceMate();
            Debug.Log("str2");
            string str1 = "Depth reached: " + sD.lastCompletedDepth;
            str1 += "\n Best move: " + sD.move + " Eval: " + sD.eval + "Search time:" + searchStopwatch.ElapsedMilliseconds + " ms.";
            str1 += "\n Num nodes: " + nNodes + "num Qnodes:" + nQNodes + "num cutoffs:" + nPrunes + "num TThits" + nTPos;
            //ConsoleHistory.instance.addLogHistory("<color=yellow>" + str1 + "</color>");
            Debug.Log(str1);
        }

        void AnnounceMate()
        {

            if (IsMateScore(bestValCurrentIteration))
            {
                int numPlyToMate = DIRECT_MATE_SCORE - System.Math.Abs(bestValCurrentIteration);
                //int numPlyToMateAfterThisMove = numPlyToMate - 1;

                int numMovesToMate = (int)Math.Ceiling(numPlyToMate / 2f);

                string sideWithMate = (bestValCurrentIteration * ((board.whiteTurn) ? 1 : -1) < 0) ? "Black" : "White";

                Debug.Log($"{sideWithMate} can mate in {numMovesToMate} move{((numMovesToMate > 1) ? "s" : "")}");
            }
        }

        public (Move move, int eval) GetResult()
        {
            return (bestMove, bestVal);
        }

        [System.Serializable]
        public class SearchDiagnostics
        {
            public int lastCompletedDepth;
            public string moveVal;
            public string move;
            public int eval;
            public bool isBook;
            public int numPositionsEvaluated;
        }

    }

}