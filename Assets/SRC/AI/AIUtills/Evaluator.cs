
using System;

namespace ChessAI
{
    /*
    @File Evaluator.cs
    @author Elias Vahlberg 
    @Date 2021-08
    */
    public class Evaluator
    {
        private BoardScoreGenerator bsg;

        public Evaluator()
        {
            bsg = AIUtillsManager.instance.BoardScoreGen;
        }

        public int Evaluate(Board board)
        {
            return bsg.V4CaptureScore(board.tiles, board.whiteTurn);
        }
    }
}