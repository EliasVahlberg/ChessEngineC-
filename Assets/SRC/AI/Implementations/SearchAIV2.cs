using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChessAI
{
    [CreateAssetMenu(fileName = "ConnectToServer", menuName = "Utilities/AI/Search AI V2")]
    public class SearchAIV2 : IAIObject
    {
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
                val = -bsGen.CaptureScore(newBoard.tiles, newBoard.whiteTurn);
                if (newBoard.Moves.Count == 0)
                    return move; //IF can CheckMate
                val += GetBoardBestValueMove(newBoard, true, bsGen)[0];
                if (ii != 0)
                    allSame &= maxV == val;
                maxI = maxV > val ? maxI : ii;
                maxV = maxV > val ? maxV : val;
                ii++;
            }
            Debug.Log("MaxV = " + maxV);
            selectedMove = allSame ? board.Moves[UnityEngine.Random.Range(0, board.Moves.Count)] : board.Moves[maxI];
            return selectedMove;
        }
        //{value, index}
        public int[] GetBoardBestValueMove(Board board, bool otherPlayer, BoardScoreGenerator bsGen)
        {
            int[] valIndex = new int[2];
            int maxI = -1;
            int maxV = int.MinValue;
            int val = 0;
            int ii = 0;
            foreach (Move move in board.Moves)
            {
                Board newBoard = board.Clone();
                newBoard.useMove(move);
                val = bsGen.CaptureScore(newBoard.tiles, newBoard.whiteTurn) * (otherPlayer ? 1 : -1);
                maxI = maxV > val ? maxI : ii;
                maxV = maxV > val ? maxV : val;
                ii++;
            }
            valIndex[0] = maxV;
            valIndex[1] = maxI;
            return valIndex;
        }
    }
}