using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChessAI
{
    [CreateAssetMenu(fileName = "AI", menuName = "Utilities/AI GEN 2/Simple Search AI")]
    public class SimpleSearchAIGen2V1 : IAIObject
    {
        public override Move SelectMove(Board board)
        {
            Move selectedMove = new Move(0);
            if (board.Moves.Count == 0)
            {
                Debug.Log("NO MOVES");
                int i = 0;
                return selectedMove;
            }
            board.generateNewMoves();
            int[] scores = new int[board.Moves.Count];
            //bool allSame = true;
            int maxI = -1;
            int maxV = int.MinValue;
            int val = 0;
            int ii = 0;
            BoardScoreGenerator bsGen = AIUtillsManager.instance.BoardScoreGen;
            foreach (Move move in board.Moves)
            {

                board.useMove(move);
                val = -bsGen.V4CaptureScore(board.tiles, board.whiteTurn);
                board.UnmakeMove();
                //if (ii != 0)
                //    allSame &= maxV == val;
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