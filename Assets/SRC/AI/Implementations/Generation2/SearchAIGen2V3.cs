using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utills;

namespace ChessAI
{
    [CreateAssetMenu(fileName = "SearchAIGen2V3", menuName = "Utilities/AI GEN 2/Search AI V3")]
    public class SearchAIGen2V3 : IAIObject
    {
        int negInf = int.MinValue / 2;
        int posInf = int.MaxValue / 2;
        private Move bestMove;
        public int searchDepth = 2;
        private long counter;
        private long numPrunes;
        public override Move SelectMove(Board board)
        {
            #region Before_ValidCopy

            int[] tilesCopy = new int[64];
            Array.Copy(board.tiles, tilesCopy, 64);
            GameState gameStateCopy = new GameState(board.currGameState.gameStateValue, board.currGameState.PrevMove);
            #endregion
            int measureID = TimeUtills.Instance.startMeasurement();
            counter = 0;
            numPrunes = 0;
            bestMove = new Move(0);
            board.generateNewMoves();
            if (board.Moves.Count == 0)
                throw new ArgumentNullException("board.Moves", "EndgameConditions are handeled by the game manager and not the AI");
            int maxVal = Search(searchDepth, searchDepth, negInf, posInf, board);
            Debug.Log(this.Name + " : " + maxVal);
            if (bestMove.MoveValue == 0)
                throw new ArgumentNullException("bestMove", "No move selected");
            Debug.Log("Number of nodes searched:" + counter);
            Debug.Log("Number of Prunes:" + counter);

            #region After_ValidityCheck
            if (!AIUtillsManager.instance.BoardIntegrityCheck(board, tilesCopy, gameStateCopy))
                throw new InvalidOperationException("Board was mutated during selection of moves");
            #endregion
            long deltaT = TimeUtills.Instance.stopMeasurementMillis(measureID);
            ConsoleHistory.instance.addLogHistory("\t<color=orange> " + this.Name + ", Time :" + deltaT + "ms </color>");
            ConsoleHistory.instance.addLogHistory("\t<color=orange> \t Positions evaluated:" + counter + "</color>");
            ConsoleHistory.instance.addLogHistory("\t<color=orange> \t Move selected:" + bestMove.ToString() + "Score: " + maxVal + "</color>");
            return bestMove;
        }

        private int Search(int depth, int maxDepth, int alpha, int beta, Board board)
        {
            counter++;
            if (depth == 0)
                return AIUtillsManager.instance.BoardScoreGen.V4CaptureScore(board.tiles, board.whiteTurn);

            board.generateNewMoves();

            if (board.Moves.Count == 0)
                return board.CurrentInCheck ? negInf : 0;
            if (board.Moves.Count == 0 && board.CurrentInCheck)
                return 0;
            foreach (Move move in board.Moves)
            {

                if (!board.useMove(move))
                    throw new ArgumentException("FAIL MAKE");

                int val = -Search(depth - 1, maxDepth, -beta, -alpha, board);
                if (!board.UnmakeMove())
                    throw new ArgumentException("FAIL UNMAKE");
                if (val >= beta)
                {
                    //PRUNE
                    numPrunes++;
                    return beta;
                }
                if (val > alpha)
                {
                    alpha = val;
                    if (depth == maxDepth)
                        bestMove = move;
                }
            }


            return alpha;
        }
    }

}