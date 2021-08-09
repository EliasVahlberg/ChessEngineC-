using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChessAI
{
    [CreateAssetMenu(fileName = "SearchAIGen2V4", menuName = "Utilities/AI GEN 2/Search AI V4")]
    public class SearchAIGen2V4 : IAIObject
    {
        int negInf = int.MinValue / 2;
        int posInf = int.MaxValue / 2;
        private Move bestMove;
        public int searchDepth = 2;
        public int CaptureValueMultiplier = 2;
        private long counter;
        private long numPrunes;
        public override Move SelectMove(Board board)
        {
            #region Before_ValidCopy

            int[] tilesCopy = new int[64];
            Array.Copy(board.tiles, tilesCopy, 64);
            GameState gameStateCopy = new GameState(board.currGameState.gameStateValue, board.currGameState.PrevMove);
            #endregion

            counter = 0;
            numPrunes = 0;
            bestMove = new Move(0);
            OrderMoves(board);

            if (board.Moves.Count == 0)
                throw new ArgumentNullException("board.Moves", "EndgameConditions are handeled by the game manager and not the AI");

            int maxVal = Search(searchDepth, searchDepth, negInf, posInf, board);

            Debug.Log(this.Name + " : " + maxVal);

            if (bestMove.MoveValue == 0)
                throw new ArgumentNullException("bestMove", "No move selected");
            Debug.Log("Number of nodes searched:" + counter);
            Debug.Log("Number of Prunes:" + counter);

            #region After_ValidityCheck
            if (!AIUtillsManager.instance.BoardIntegrityCheck(board, tilesCopy, gameStateCopy))
                throw new InvalidOperationException("Board was mutated during selection of moves");
            #endregion
            return bestMove;
        }

        private int Search(int depth, int maxDepth, int alpha, int beta, Board board)
        {
            counter++;
            if (depth == 0)
                return AIUtillsManager.instance.BoardScoreGen.V4CaptureScore(board.tiles, board.whiteTurn);

            board.generateNewMoves();

            if (board.Moves.Count == 0)
                return board.CurrentInCheck ? negInf : 0;
            if (board.Moves.Count == 0 && board.CurrentInCheck)
                return 0;
            foreach (Move move in board.Moves)
            {

                if (!board.useMove(move))
                    throw new ArgumentException("FAIL MAKE");

                int val = -Search(depth - 1, maxDepth, -beta, -alpha, board);
                if (!board.UnmakeMove())
                    throw new ArgumentException("FAIL UNMAKE");
                if (val >= beta)
                {
                    //PRUNE
                    numPrunes++;
                    return beta;
                }
                if (alpha <= val)
                {
                    alpha = alpha > val ? alpha : val;
                    if (depth == maxDepth)
                        bestMove = move;
                }
            }


            return alpha;
        }

        public void OrderMoves(Board board)
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
                    moveScoreEstimate = CaptureValueMultiplier * BoardScoreGenerator.pieceScore[capPieceT] - BoardScoreGenerator.pieceScore[movePieceT];

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