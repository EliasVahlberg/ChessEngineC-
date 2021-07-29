using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChessAI
{
    public class RandAI : ScriptableObject, IAI
    {
        public Move SelectMove(Board board)
        {
            return board.Moves[Random.Range(0, board.Moves.Count)];
        }
    }
}