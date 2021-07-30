using ChessAI;
using Utills;

namespace Testing
{
    public readonly struct PreformanceTestResult
    {
        public readonly long time;
        public readonly int moves;
        public readonly int wPoints;
        public readonly int bPoints;
        public readonly float avrageTimePerTurn;
        public readonly string lastMoveFen;
        public PreformanceTestResult(
        long time,
        int moves,
        int wPoints,
        int bPoints,
        float avrageTimePerTurn,
        string lastMoveFen)
        {
            this.time = time;
            this.moves = moves;
            this.wPoints = wPoints;
            this.bPoints = bPoints;
            this.avrageTimePerTurn = avrageTimePerTurn;
            this.lastMoveFen = lastMoveFen;
        }
    }
    public static class PreformanceTest
    {

        private static string testFEN = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";//Position 5
        public static PreformanceTestResult startAIMoveTest(IAIObject wAI, IAIObject bAI, string fen, int moves)
        {
            long time = 0;
            int nMoves = 0;
            int wPoints = 0;
            int bPoints = 0;
            float avrageTimePerTurn = 0f;
            string lastMoveFen = "";
            Board board = new Board(fen);
            Move move;
            int ii = 0;
            int measureID = TimeUtills.Instance.startMeasurement();
            for (ii = 0; ii < moves; ii++)
            {
                if (board.Moves.Count == 0)
                    break;
                move = board.whiteTurn ? wAI.SelectMove(board) : bAI.SelectMove(board);
                if (move.StartSquare == 0 && move.TargetSquare == 0)
                    break;
                board.useMove(move);
                if (board.lastMoveWasCapture)
                {
                    if (board.whiteTurn)
                        bPoints += Piece.PieceValueDictionary[Piece.PieceType(board.lastMoveCaptured)];
                    else
                        wPoints += Piece.PieceValueDictionary[Piece.PieceType(board.lastMoveCaptured)];
                }
            }
            time = TimeUtills.Instance.stopMeasurementMillis(measureID);
            nMoves = ii;
            avrageTimePerTurn = (float)time / nMoves;
            lastMoveFen = board.boardToFEN();
            return new PreformanceTestResult(
            time,
            moves,
            wPoints,
            bPoints,
            avrageTimePerTurn,
            lastMoveFen
            );
        }
        public static PreformanceTestResult startAIMoveTest(IAIObject wAI, IAIObject bAI, int moves)
        { return startAIMoveTest(wAI, bAI, testFEN, moves); }

    }
}