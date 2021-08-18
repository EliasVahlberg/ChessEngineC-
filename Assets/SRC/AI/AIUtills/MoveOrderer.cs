
using SimpleChess;

namespace ChessAI
{
    /*
    @File Evaluator.cs
    @author Elias Vahlberg 
    @Date 2021-08
    */
    public class MoveOrderer
    {
        private TranspositionTable tt;
        private const int CAPTURE_VAL_MULTIPLIER = 15;
        private const int UNSAFE_POS_MULTIPLIER = 1;
        private const int CAPTURED_BY_PAWN_MULTIPLIER = 20;

        public MoveOrderer(TranspositionTable tt)
        {
            this.tt = tt;
        }

        public void Order(Board board, bool usePrevSearch = false)
        {
            board.generateNewMoves();
            Move prevBest = Search.INVAL_MOVE;
            if (usePrevSearch)
            {
                prevBest = tt.GetStoredMove();
            }

            int[] moveScoreEstimates = new int[board.Moves.Count];

            for (int ii = 0; ii < board.Moves.Count; ii++)
            {
                Move move = board.Moves[ii];
                int moveScoreEstimate = 0;
                int movePieceT = Piece.PieceType(board.tiles[move.StartSquare]);
                int capPieceT = Piece.PieceType(board.tiles[move.TargetSquare]);
                if (capPieceT != 0)
                {
                    moveScoreEstimate = CAPTURE_VAL_MULTIPLIER * BoardScoreGenerator.pieceScore[capPieceT] - BoardScoreGenerator.pieceScore[movePieceT];

                }
                if (move.moveFlag == Move.Flag.PromoteToQueen)
                    moveScoreEstimate += BoardScoreGenerator.pieceScore[Piece.QUEEN];
                else if (move.moveFlag == Move.Flag.PromoteToKnight)
                    moveScoreEstimate += BoardScoreGenerator.pieceScore[Piece.KNIGHT];
                else if (move.moveFlag == Move.Flag.PromoteToBishop)
                    moveScoreEstimate += BoardScoreGenerator.pieceScore[Piece.BISHOP];
                else if (move.moveFlag == Move.Flag.PromoteToRook)
                    moveScoreEstimate += BoardScoreGenerator.pieceScore[Piece.ROOK];

                if (!board.MoveGenerator.isSafePosition(move.TargetSquare))
                    moveScoreEstimate -= BoardScoreGenerator.pieceScore[movePieceT] * UNSAFE_POS_MULTIPLIER;
                if (BoardUtills.ContainsTile(board.MoveGenerator.currentPawnAttackMap, move.TargetSquare))
                    moveScoreEstimate -= CAPTURED_BY_PAWN_MULTIPLIER * BoardScoreGenerator.pieceScore[movePieceT];
                moveScoreEstimates[ii] = moveScoreEstimate;
                if (move.Equals(prevBest))
                    moveScoreEstimates[ii] += 10000;

            }
            //TODO Implement better sorting
            for (int ii = 0; ii < board.Moves.Count - 1; ii++)
            {
                for (int jj = ii + 1; jj > 0; jj--)
                {
                    int swapIndex = jj - 1;
                    if (moveScoreEstimates[swapIndex] < moveScoreEstimates[jj])
                    {
                        (board.Moves[jj], board.Moves[swapIndex]) = (board.Moves[swapIndex], board.Moves[jj]);
                        (moveScoreEstimates[jj], moveScoreEstimates[swapIndex]) = (moveScoreEstimates[swapIndex], moveScoreEstimates[jj]);
                    }
                }
            }
        }
    }
}