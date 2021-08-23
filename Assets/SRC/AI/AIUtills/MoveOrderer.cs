
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

        private const int BASE_MULTIPLIER = 30;
        private const int CAPTURE_VAL_MULTIPLIER = 10;
        private const int Next_Turn_Capture_Multiplier = 5;
        private const int UNSAFE_POS_MULTIPLIER = 1;
        private const int CAPTURED_BY_PAWN_MULTIPLIER = 20;
        private const int WEIGHT_MAP_MULTIPLIER = 1;

        public MoveOrderer(TranspositionTable tt)
        {
            this.tt = tt;
        }

        public void Order(Board board, bool usePrevSearch = false, bool extendedOrdering = false)
        {
            board.generateNewMoves();
            Move prevBest = Search.INVAL_MOVE;
            if (usePrevSearch)
            {
                prevBest = tt.GetStoredMove(); //PV-Move https://www.chessprogramming.org/PV-Move
            }

            int[] moveScoreEstimates = new int[board.Moves.Count];
            Move[] moves = board.Moves.ToArray();
            for (int ii = 0; ii < board.Moves.Count; ii++)
            {
                Move move = moves[ii];

                moveScoreEstimates[ii] = getMoveScoreEstimate(move, board, extendedOrdering);
                if (move.Equals(prevBest))
                    moveScoreEstimates[ii] += 10000 * BASE_MULTIPLIER;
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

        private int getMoveScoreEstimate(Move move, Board board, bool extendedOrdering)
        {
            #region Base

            int moveScoreEstimate = 0;
            int movePieceT = Piece.PieceType(board.tiles[move.StartSquare]);
            int capPieceT = Piece.PieceType(board.tiles[move.TargetSquare]);
            if (capPieceT != 0)
            {
                //MVV-LVA  https://www.chessprogramming.org/MVV-LVA
                moveScoreEstimate = (CAPTURE_VAL_MULTIPLIER * BoardScoreGenerator.pieceScoreArr[capPieceT]) * BASE_MULTIPLIER - BoardScoreGenerator.pieceScoreArr[movePieceT] * BASE_MULTIPLIER;

            }
            if (move.moveFlag == Move.Flag.PromoteToQueen)
                moveScoreEstimate += BoardScoreGenerator.pieceScoreArr[Piece.QUEEN] * BASE_MULTIPLIER;
            else if (move.moveFlag == Move.Flag.PromoteToKnight)
                moveScoreEstimate += BoardScoreGenerator.pieceScoreArr[Piece.KNIGHT] * BASE_MULTIPLIER;
            else if (move.moveFlag == Move.Flag.PromoteToBishop)
                moveScoreEstimate += BoardScoreGenerator.pieceScoreArr[Piece.BISHOP] * BASE_MULTIPLIER;
            else if (move.moveFlag == Move.Flag.PromoteToRook)
                moveScoreEstimate += BoardScoreGenerator.pieceScoreArr[Piece.ROOK] * BASE_MULTIPLIER;

            if (!board.MoveGenerator.isSafePosition(move.TargetSquare))
                moveScoreEstimate -= BoardScoreGenerator.pieceScoreArr[movePieceT] * UNSAFE_POS_MULTIPLIER * BASE_MULTIPLIER;
            if (BoardUtills.ContainsTile(board.MoveGenerator.currentPawnAttackMap, move.TargetSquare))
                moveScoreEstimate -= CAPTURED_BY_PAWN_MULTIPLIER * BoardScoreGenerator.pieceScore[movePieceT] * BASE_MULTIPLIER;

            #endregion

            #region Additional

            moveScoreEstimate += BoardWeightMap.MoveImpact(Piece.PieceType(board.tiles[move.StartSquare]), move.StartSquare, move.TargetSquare, board.whiteTurn);

            if (extendedOrdering)
            {
                //if (!board.useMove(move, isSearchMove: true))
                //    throw new System.ArgumentException("FAIL MAKE");
                //board.generateNonQuietMoves();
                //int maxCap = 0;
                //for (int ii = 0; ii < board.Moves.Count; ii++)
                //{
                //    int cap = Piece.PieceType(board.tiles[board.Moves[ii].TargetSquare]);
                //    int type = Piece.PieceType(board.tiles[board.Moves[ii].StartSquare]);
                //    maxCap = System.Math.Max(maxCap, (CAPTURE_VAL_MULTIPLIER * BoardScoreGenerator.pieceScoreArr[cap]) * BASE_MULTIPLIER - BoardScoreGenerator.pieceScore[type] * BASE_MULTIPLIER);
                //}
                //moveScoreEstimate -= maxCap * Next_Turn_Capture_Multiplier;
                //if (!board.UnmakeMove(isSearchMove: true))
                //    throw new System.ArgumentException("FAIL UNMAKE");
                //board.generateNewMoves();
            }
            #endregion


            return moveScoreEstimate;
        }
    }
}