using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utills;

namespace ChessAI
{
    [CreateAssetMenu(fileName = "SearchAIGen2V2", menuName = "Utilities/AI GEN 2/Search AI V2")]
    public class SearchAIGen2V2 : IAIObject
    {
        int negInf = int.MinValue / 2;
        private Move bestMove;
        public int searchDepth = 2;
        private long counter;
        public override Move SelectMove(Board board)
        {
            int measureID = TimeUtills.Instance.startMeasurement();
            #region Before_ValidCopy
            int[] tilesCopy = new int[64];
            Array.Copy(board.tiles, tilesCopy, 64);
            GameState gameStateCopy = new GameState(board.currGameState.gameStateValue, board.currGameState.PrevMove);
            #endregion

            bestMove = new Move(0);
            counter = 0;
            board.generateNewMoves();
            if (board.Moves.Count == 0)
                throw new ArgumentNullException("board.Moves", "EndgameConditions are handeled by the game manager and not the AI");
            int maxVal = Search(searchDepth, searchDepth, board);
            Debug.Log(this.Name + " : " + maxVal);
            Debug.Log("Number of nodes searched:" + counter);
            if (bestMove.MoveValue == 0)
                throw new ArgumentNullException("bestMove", "No move selected");

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

        private int Search(int depth, int maxDepth, Board board)
        {
            counter++;
            if (depth == 0)
                return AIUtillsManager.instance.BoardScoreGen.V4CaptureScore(board.tiles, board.whiteTurn);

            board.generateNewMoves();
            int max = negInf;
            if (board.Moves.Count == 0)
                return board.CurrentInCheck ? negInf : 0;
            if (board.Moves.Count == 0 && board.CurrentInCheck)
                return 0;
            foreach (Move move in board.Moves)
            {

                if (!board.useMove(move))
                    throw new ArgumentException("FAIL MAKE");

                int val = -Search(depth - 1, maxDepth, board);
                if (max <= val)
                {
                    max = max > val ? max : val;
                    if (depth == maxDepth)
                        bestMove = move;
                }
                if (!board.UnmakeMove())
                    throw new ArgumentException("FAIL UNMAKE");
            }


            return max;
        }
    }

}