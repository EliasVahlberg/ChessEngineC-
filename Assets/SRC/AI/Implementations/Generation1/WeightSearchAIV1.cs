using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChessAI
{
    /*
    @File WeightSearchAIV1.cs
    @author Elias Vahlberg 
    @Date 2021-07 
    !Deprecated
    */
    [CreateAssetMenu(fileName = "ConnectToServer", menuName = "Utilities/AI/Weight&Search AI V1")]
    public class WeightSearchAIV1 : IAIObject
    {
        public static int CaptureWeight = 20;
        public static int MapPlacementWeight = 1;
        public override Move SelectMove(Board board)
        {
            Move selectedMove = new Move(0);
            if (board.Moves.Count == 0)
            {
                Debug.Log("NO MOVES");
                return selectedMove;
            }
            int[] scores = new int[board.Moves.Count];
            bool allSame = true;
            int maxI = -1;
            int maxV = int.MinValue;
            int val = 0;
            int ii = 0;
            BoardScoreGenerator bsGen = AIUtillsManager.instance.BoardScoreGen;
            foreach (Move move in board.Moves)
            {
                Board newBoard = board.Clone();
                newBoard.useMove(move);
                val = BoardWeightMap.Evaluate(newBoard, board.whiteTurn) * MapPlacementWeight;
                val += -bsGen.V4CaptureScore(newBoard.tiles, newBoard.whiteTurn) * CaptureWeight;
                if (ii != 0)
                    allSame &= maxV == val;
                maxI = maxV > val ? maxI : ii;
                maxV = maxV > val ? maxV : val;
                ii++;
            }
            Debug.Log("maxV: " + maxV);
            selectedMove = board.Moves[maxI];
            return selectedMove;
        }
    }
}