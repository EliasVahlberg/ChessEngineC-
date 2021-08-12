using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChessAI
{
    /*
    @File RandAI.cs
    @author Elias Vahlberg 
    @Date 2021-07 
    !Deprecated
    */
    [CreateAssetMenu(fileName = "ConnectToServer", menuName = "Utilities/AI/RandAI")]
    public class RandAI : IAIObject
    {
        public override Move SelectMove(Board board)
        {
            Move move = new Move(0);
            int i = 0;
            try
            {
                i = UnityEngine.Random.Range(0, board.Moves.Count);
                move = board.Moves[i];
                return move;
            }
            catch (Exception _ex)
            {
                Debug.LogError("i = " + i + ", Count = " + board.Moves.Count);
                return move;
            }
        }
    }
}