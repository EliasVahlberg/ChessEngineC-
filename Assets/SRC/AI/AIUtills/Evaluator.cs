
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
        private int BSG_W = 100, BWM_W = 1, PS_W = 5;
        private bool useBWM;
        private bool usePSE;

        public Evaluator(int captureWeight, int weightMapWeight, bool useWeightMap, bool usePawnStructureEval, int pawnStructureWeight)
        {
            bsg = AIUtillsManager.instance.BoardScoreGen;
            BSG_W = captureWeight;
            BWM_W = weightMapWeight;
            useBWM = useWeightMap;
            usePSE = usePawnStructureEval;
            PS_W = pawnStructureWeight;


        }
        public int Evaluate(Board board, bool coin)
        {
            return Evaluate(board) + (coin ? 1 : -1);
        }
        public int Evaluate(Board board)
        {
            int val = BSG_W * bsg.V6CaptureScore(board);

            if (useBWM)
            {
                int endgameVal = bsg.EndgameValueV2(board);
                val += BoardWeightMap.EvaluateV3(board, endgameVal) * BWM_W;
            }
            if (usePSE)
            {
                val += PS_W * bsg.PawnStructureValueV1(board);
            }
            return val;
        }
    }
}