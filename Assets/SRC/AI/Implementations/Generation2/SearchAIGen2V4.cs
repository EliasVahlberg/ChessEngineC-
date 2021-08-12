using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utills;

namespace ChessAI
{
    /*
    @File SearchAIGen2V4.cs
    @author Elias Vahlberg 
    @Date 2021-08 
    */
    [CreateAssetMenu(fileName = "SearchAIGen2V4", menuName = "Utilities/AI GEN 2/Search AI V4")]
    public class SearchAIGen2V4 : IAIObject
    {
        int negInf = int.MinValue / 2;
        int posInf = int.MaxValue / 2;
        private Move bestMove;
        public int searchDepth = 2;
        public int CaptureValueMultiplier = 2;
        private long counter;
        private long numPrunes;
        public override Move SelectMove(Board board)
        {
            int measureID = TimeUtills.Instance.startMeasurement();
            #region Before_ValidCopy

            int[] tilesCopy = new int[64];
            Array.Copy(board.tiles, tilesCopy, 64);
            GameState gameStateCopy = new GameState(board.currGameState.gameStateValue, board.currGameState.PrevMove);
            #endregion

            counter = 0;
            numPrunes = 0;
            bestMove = new Move(0);

            if (board.Moves.Count == 0)
                throw new ArgumentNullException("board.Moves", "EndgameConditions are handeled by the game manager and not the AI");

            int maxVal = 0;//Search(searchDepth, searchDepth, negInf, posInf, board);

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




    }

}