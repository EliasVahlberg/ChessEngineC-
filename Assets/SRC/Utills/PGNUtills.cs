using System.Collections.Generic;
using UnityEngine;
using static Piece;
namespace Utills
{
    public static class PGNUtills
    {
        #region Creation


        public static string GetPGN(Move[] moves)
        {
            string pgn = "";
            Board board = Board.DefaultBoard();

            for (int plyCount = 0; plyCount < moves.Length; plyCount++)
            {
                string moveString = PGNMoveToString(board, moves[plyCount]);
                board.useMove(moves[plyCount]);

                if (plyCount % 2 == 0)
                {
                    pgn += ((plyCount / 2) + 1) + ". ";
                }
                pgn += moveString + " ";
            }

            return pgn;
        }

        private static string PGNMoveToString(Board board, Move move)
        {
            int mpt = Piece.PieceType(board.tiles[move.StartSquare]);
            int cpt = Piece.PieceType(board.tiles[move.TargetSquare]);
            if (move.moveFlag == Move.Flag.Castling)
            {
                int delta = move.TargetSquare - move.StartSquare;
                if (delta == 2)
                {
                    return "O-O";
                }
                else if (delta == -2)
                {
                    return "O-O-O";
                }
            }

            string moveNotation = GetSymbolFromType(mpt);

            if (mpt != PAWN && mpt != KING)
            {
                board.generateNewMoves();
                foreach (Move altMove in board.Moves)
                {
                    if (Piece.PieceType(board.tiles[altMove.StartSquare]) == mpt)
                    {
                        int ffi = BoardUtills.File(move.StartSquare);
                        int affi = BoardUtills.File(altMove.StartSquare);
                        int fri = BoardUtills.Rank(move.StartSquare);
                        int afri = BoardUtills.Rank(altMove.StartSquare);
                        if (ffi != affi)
                        { // pieces on different files, thus ambiguity can be resolved by specifying file
                            moveNotation += BoardUtills.fileNames[ffi];
                            break; // ambiguity resolved
                        }
                        else if (fri != afri)
                        {
                            moveNotation += BoardUtills.rankNames[fri];
                            break; // ambiguity resolved
                        }
                    }
                }

            }

            if (cpt != 0)
            {
                if (mpt == PAWN)
                {
                    moveNotation += BoardUtills.fileNames[BoardUtills.File(move.StartSquare)];
                }
                moveNotation += "x";
            }
            else
            {
                if (move.moveFlag == Move.Flag.EnPassantCapture)
                {
                    moveNotation += BoardUtills.fileNames[BoardUtills.File(move.StartSquare)] + "x";
                }
            }

            moveNotation += BoardUtills.stringFromIndex(move.TargetSquare);

            if (move.isPromotion())
                moveNotation += "=" + GetSymbolFromType(PromotionPieceType(move.moveFlag));


            board.useMove(move, isSearchMove: true);
            board.generateNewMoves();

            if (board.CurPlayerInCheck)
            {
                if (board.Moves.Count == 0)
                {
                    moveNotation += "#";
                }
                else
                {
                    moveNotation += "+";
                }
            }

            board.UnmakeMove(isSearchMove: true);

            return moveNotation;
        }

        private static string GetSymbolFromType(int pieceType)
        {
            switch (pieceType)
            {
                case ROOK:
                    return "R";
                case KNIGHT:
                    return "N";
                case BISHOP:
                    return "B";
                case QUEEN:
                    return "Q";
                case KING:
                    return "K";
                default:
                    return "";
            }
        }

        private static int PromotionPieceType(int flag)
        {
            switch (flag)
            {
                case Move.Flag.PromoteToBishop:
                    return BISHOP;
                case Move.Flag.PromoteToKnight:
                    return KNIGHT;
                case Move.Flag.PromoteToRook:
                    return ROOK;
                case Move.Flag.PromoteToQueen:
                    return QUEEN;
                default:
                    return 0;
            }
        }
        #endregion
        #region Retrieval
        public static Move[] MovesFromPGN(string pgn, int maxPlyCount = int.MaxValue)
        {
            List<string> algebraicMoves = new List<string>();
            string[] pages = pgn.Replace("\n", " ").Split(' ');
            for (int ii = 0; ii < pages.Length; ii++)
            {
                if (algebraicMoves.Count == maxPlyCount)
                    break;
                string page = pages[ii].Trim();
                if (page.Contains(".") || page == "1/2-1/2" || page == "1-0" || page == "0-1")
                { continue; }
                if (!string.IsNullOrEmpty(page))
                {
                    algebraicMoves.Add(page);
                }
            }

            return MovesFromAlgebraic(algebraicMoves.ToArray());
        }

        static Move[] MovesFromAlgebraic(string[] algebraicMoves)
        {
            Board board = Board.DefaultBoard();
            List<Move> moves = new List<Move>();

            for (int i = 0; i < algebraicMoves.Length; i++)
            {
                Move move = MoveFromAlgebraic(board, algebraicMoves[i].Trim());
                if (move.MoveValue == 0)
                {

                    UnityEngine.Debug.Log("illegal move in supplied pgn: " + algebraicMoves[i] + " move index: " + i);
                    string pgn = "";
                    foreach (string s in algebraicMoves)
                    {
                        pgn += s + " ";
                    }
                    Debug.Log("problematic pgn: " + pgn);
                    board.Moves.ToArray();
                }
                else
                {
                    moves.Add(move);
                }
                board.useMove(move);
            }
            return moves.ToArray();
        }

        //* Who ever came up with the PGN system is an absolute amature
        //? Honestly did they want it to take the most ammount of time to parse ?
        static Move MoveFromAlgebraic(Board board, string algebraicMove)
        {

            // Remove unrequired info from move string
            algebraicMove = algebraicMove.Replace("+", "").Replace("#", "").Replace("x", "").Replace("-", "");
            board.generateNewMoves();
            List<Move> allMoves = board.Moves;

            Move move = new Move();

            foreach (Move moveToTest in allMoves)
            {
                move = moveToTest;

                int moveFromIndex = move.StartSquare;
                int moveToIndex = move.TargetSquare;
                int movePieceType = Piece.PieceType(board.tiles[moveFromIndex]);
                int f1 = BoardUtills.File(moveFromIndex),
                    f2 = BoardUtills.File(moveToIndex),
                    r1 = BoardUtills.Rank(moveFromIndex),
                    r2 = BoardUtills.Rank(moveToIndex);
                if (algebraicMove == "OO")
                { // castle kingside
                    if (movePieceType == KING && moveToIndex - moveFromIndex == 2)
                    {
                        return move;
                    }
                }
                else if (algebraicMove == "OOO")
                { // castle queenside
                    if (movePieceType == KING && moveToIndex - moveFromIndex == -2)
                    {
                        return move;
                    }
                }
                // Is pawn move if starts with any file indicator (e.g. 'e'4. Note that uppercase B is used for bishops) 
                else if (BoardUtills.fileNames.Contains(algebraicMove[0].ToString()))
                {
                    if (movePieceType != PAWN)
                    {
                        continue;
                    }
                    if (BoardUtills.fileNames.IndexOf(algebraicMove[0]) == f1)
                    { // correct starting file
                        if (algebraicMove.Contains("="))
                        { // is promotion
                            if (r2 == 0 || r2 == 7)
                            {

                                if (algebraicMove.Length == 5) // pawn is capturing to promote
                                {
                                    char targetFile = algebraicMove[1];
                                    if (BoardUtills.fileNames.IndexOf(targetFile) != f2)
                                    {
                                        // Skip if not moving to correct file
                                        continue;
                                    }
                                }
                                char promotionChar = algebraicMove[algebraicMove.Length - 1];

                                if (PromotionPieceType(move.moveFlag) != GetTypeFromSymbol(promotionChar))
                                {
                                    continue; // skip this move, incorrect promotion type
                                }

                                return move;
                            }
                        }
                        else
                        {

                            char targetFile = algebraicMove[algebraicMove.Length - 2];
                            char targetRank = algebraicMove[algebraicMove.Length - 1];

                            if (BoardUtills.fileNames.IndexOf(targetFile) == f2)
                            { // correct ending file
                                if (targetRank.ToString() == (r2 + 1).ToString())
                                { // correct ending rank
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                { // regular piece move

                    char movePieceChar = algebraicMove[0];
                    if (GetTypeFromSymbol(movePieceChar) != movePieceType)
                    {
                        continue; // skip this move, incorrect move piece type
                    }

                    char targetFile = algebraicMove[algebraicMove.Length - 2];
                    char targetRank = algebraicMove[algebraicMove.Length - 1];
                    if (BoardUtills.fileNames.IndexOf(targetFile) == f2)
                    { // correct ending file
                        if (targetRank.ToString() == (r2 + 1).ToString())
                        { // correct ending rank

                            if (algebraicMove.Length == 4)
                            { // addition char present for disambiguation (e.g. Nbd7 or R7e2)
                                char disambiguationChar = algebraicMove[1];

                                if (BoardUtills.fileNames.Contains(disambiguationChar.ToString()))
                                { // is file disambiguation
                                    if (BoardUtills.fileNames.IndexOf(disambiguationChar) != f1)
                                    { // incorrect starting file
                                        continue;
                                    }
                                }
                                else
                                { // is rank disambiguation
                                    if (disambiguationChar.ToString() != (r1 + 1).ToString())
                                    { // incorrect starting rank
                                        continue;
                                    }

                                }
                            }
                            break;
                        }
                    }
                }
            }
            return move;
        }

        private static int GetTypeFromSymbol(char symbol)
        {
            switch (symbol)
            {
                case 'R': return ROOK;
                case 'N': return KNIGHT;
                case 'B': return BISHOP;
                case 'Q': return QUEEN;
                case 'K': return KING;
                default:
                    return 0;
            }
        }
        #endregion

    }
}
