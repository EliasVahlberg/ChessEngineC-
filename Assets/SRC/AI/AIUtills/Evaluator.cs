
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
        #region eval/WeightConfig
        int captureWeight;
        int weightMapWeight;
        bool useWeightMap;
        #endregion
        private BoardScoreGenerator bsg;
        private int BSG_W = 100, BWM_W = 1;
        private bool useBWM;

        public Evaluator(int captureWeight, int weightMapWeight, bool useWeightMap)
        {
            bsg = AIUtillsManager.instance.BoardScoreGen;
            BSG_W = captureWeight;
            BWM_W = weightMapWeight;
            useBWM = useWeightMap;

        }

        public int Evaluate(Board board)
        {
            int val = BSG_W * bsg.V4CaptureScore(board.tiles, board.whiteTurn);
            if (useBWM)
                val += BoardWeightMap.Evaluate(board) * BWM_W;
            return val;
        }
    }
}