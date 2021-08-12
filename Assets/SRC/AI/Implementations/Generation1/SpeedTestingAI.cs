using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChessAI
{
    /*
    @File SpeedTestingAI.cs
    @author Elias Vahlberg 
    @Date 2021-07 
    !Deprecated
    */
    [CreateAssetMenu(fileName = "ConnectToServer", menuName = "Utilities/AI/Speed testing AI")]
    public class SpeedTestingAI : IAIObject
    {
        static int nRecounts = 1000;
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
                for (int i = 0; i < nRecounts; i++)
                    val = -bsGen.CaptureScore(newBoard.tiles, newBoard.whiteTurn);
                if (ii != 0)
                    allSame &= maxV == val;
                maxI = maxV > val ? maxI : ii;
                maxV = maxV > val ? maxV : val;
                ii++;
            }
            //Debug.Log("MaxV = " + maxV);  Removde while testing
            //selectedMove = allSame ? board.Moves[UnityEngine.Random.Range(0, board.Moves.Count)] : board.Moves[maxI];
            selectedMove = board.Moves[maxI];
            return selectedMove;
        }
    }
}