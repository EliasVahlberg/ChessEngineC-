
using System;
using SimpleChess;
using UnityEngine;

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
        private const int CAPTURE_VAL_MULTIPLIER = 2;

        public MoveOrderer(TranspositionTable tt)
        {
            this.tt = tt;
        }

        public void Order(Board board)
        {
            board.generateNewMoves();
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
                {
                    moveScoreEstimate += BoardScoreGenerator.pieceScore[Piece.QUEEN];
                }
                if (move.moveFlag == Move.Flag.PromoteToKnight)
                {
                    moveScoreEstimate += BoardScoreGenerator.pieceScore[Piece.KNIGHT];
                }
                moveScoreEstimates[ii] = moveScoreEstimate;

            }
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