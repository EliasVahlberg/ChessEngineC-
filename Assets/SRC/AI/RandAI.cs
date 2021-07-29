using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChessAI
{
    [CreateAssetMenu(fileName = "ConnectToServer", menuName = "Utilities/AI/RandAI")]
    public class RandAI : IAIObject
    {
        public override Move SelectMove(Board board)
        {
            return board.Moves[Random.Range(0, board.Moves.Count)];
        }
    }
}