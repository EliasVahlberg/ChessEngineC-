
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
        public static readonly Move INVAL_MOVE = new Move(0);
        public bool useCoinToss = false;

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
        public SearchDiagnostics sD;

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

            evaluator = new Evaluator(settings.captureWeight, settings.weightMapWeight, settings.useWeightMap);
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
            //Debug.Log("SearchStart");
            ResetSearchInfo();

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
                        sD.numPositionsEvaluated = nNodes;
                        sD.numQPositionsEvaluated = nQNodes;
                        sD.numPrunes = nPrunes;
                        sD.numTPositions = nTPos;
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

                SearchRecurrRecurr(settings.depth, settings.depth, NEG_INF, POS_INF);
                bestMove = bestMoveCurrentIteration;
                bestVal = bestValCurrentIteration;
                sD.lastCompletedDepth = settings.depth;
                sD.move = bestMove.ToString();
                sD.eval = bestVal;
                sD.numPositionsEvaluated = nNodes;
                sD.numQPositionsEvaluated = nQNodes;
                sD.numPrunes = nPrunes;
                sD.numTPositions = nTPos;
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
            if (depth < maxDepth)
            {
                //TODO FIX SO IT DOESN't USE REPEAT
                if (board.HashHistory.Contains(board.ZobristKey))
                {
                    return 0;
                }

                alpha = Math.Max(alpha, -DIRECT_MATE_SCORE + (maxDepth - depth));
                beta = Math.Min(beta, DIRECT_MATE_SCORE - (maxDepth - depth));
                if (alpha >= beta)
                {
                    //Debug.Log("BetterMateFound");
                    return alpha;
                }
            }
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
                return settings.useQuiescenceSearch ? QuiescenceSearch(alpha, beta) : evaluator.Evaluate(board, CoinToss());

            moveOrderer.Order(board, usePrevSearch: settings.useTranspositionTable, extendedOrdering: (maxDepth >= 4 && 2 > (maxDepth - depth)));

            if (board.Moves.Count == 0)
                return board.CurrentInCheck ? -(DIRECT_MATE_SCORE - (maxDepth - depth)) : 0;

            Move bestMoveThisPos = INVAL_MOVE;


            int evalType = TranspositionTable.UpperBound;


            foreach (Move move in board.Moves)
            {

                if (!board.useMove(move, isSearchMove: true))
                    throw new ArgumentException("FAIL MAKE");
                int val = -SearchRecurrRecurr(depth - 1, maxDepth, -beta, -alpha);
                if (!board.UnmakeMove(isSearchMove: true))
                    throw new ArgumentException("FAIL UNMAKE");
                if (val >= beta)
                {
                    //PRUNE
                    tt.StoreEvaluation(depth, maxDepth - depth, beta, TranspositionTable.LowerBound, move);
                    nPrunes++;
                    return beta;
                }
                if (val == alpha && useCoinToss)
                    val += (CoinToss() ? 1 : 0);

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

            //Store best eval (for deeper search)
            tt.StoreEvaluation(depth, maxDepth - depth, beta, evalType, bestMoveThisPos);
            return alpha;
        }
        /*
        * This prevents bad captures at max search depth
        */
        int QuiescenceSearch(int alpha, int beta)
        {
            int val = evaluator.Evaluate(board);
            nNodes++;
            if (val >= beta)
                return beta;
            if (val > alpha)
                alpha = val;
            board.generateNonQuietMoves();
            moveOrderer.Order(board, usePrevSearch: false);

            foreach (Move move in board.Moves)
            {
                if (!board.useMove(move, isSearchMove: true))
                    throw new ArgumentException("FAIL MAKE");
                val = -QuiescenceSearch(-beta, -alpha); //+ evalModifier;
                if (!board.UnmakeMove(isSearchMove: true))
                    throw new ArgumentException("FAIL UNMAKE");
                nQNodes++;
                if (val >= beta)
                {
                    nPrunes++;
                    return beta;
                }
                if (val > alpha)
                {
                    alpha = val;
                }
            }

            return alpha;
        }
        private int getEvalModifier()
        {
            int val = 0;
            val += (settings.useCheckValue && board.CurPlayerInCheck) ? -settings.checkValue : 0;
            val += (settings.useNumMovesWeight) ? board.Moves.Count * settings.numMovesWeight : 0;
            return val;
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

        public string LogDebugInfo()
        {
            AnnounceMate();
            sD.numPositionsEvaluated = nNodes;
            sD.numQPositionsEvaluated = nQNodes;
            sD.numPrunes = nPrunes;
            sD.numTPositions = nTPos;

            string str1 = "Depth reached: " + sD.lastCompletedDepth;
            str1 += "\n Best move: " + sD.move + " Eval: " + sD.eval + "Search time:" + searchStopwatch.ElapsedMilliseconds + " ms.";
            str1 += "\n Num nodes: " + sD.numPositionsEvaluated +
             "num Qnodes:" + sD.numQPositionsEvaluated +
             " num cutoffs:" + sD.numPrunes +
             "num TThits" + sD.numTPositions;
            //ConsoleHistory.instance.addLogHistory("<color=yellow>" + str1 + "</color>");
            Debug.Log(str1);
            return str1;
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


        public bool CoinToss()
        {
            return (((board.ZobristKey + ((ulong)board.Moves.Count) + ((ulong)searchStopwatch.ElapsedTicks)) >> ((int)(board.ZobristKey % 64))) & 0b01) == 1;
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
            public int numQPositionsEvaluated;
            public int numTPositions;
            public int numPrunes;
        }

    }

}