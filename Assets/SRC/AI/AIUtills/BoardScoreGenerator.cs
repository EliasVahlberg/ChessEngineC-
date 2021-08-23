
using System.Collections.Generic;

namespace ChessAI
{
    /*
    @File BoardScoreGenerator.cs
    @author Elias Vahlberg 
    @Date 2021-07 
    */
    public class BoardScoreGenerator
    {
        //NONE = 0 = 0b000
        //KING = 1 = 0b001
        //PAWN = 2 = 0b010
        //KNIGHT = 3 = 0b011
        //BISHOP = 5 = 0b101
        //ROOK = 6 = 0b110
        //QUEEN = 7 = 0b111
        //WHITE = 8 = 0b01000
        //BLACK = 16 = 0b10000

        //ORDER:{NONE,KING,PAWN,KNIGHT,BISHOP,ROOK,QUEEN,WHITE,BLACK }
        const int WHITE = 0b01000;
        const int BLACK = 0b10000;
        const int COLOR_MASK = 0b11000;
        const int ENDGAME_START_VALUE = 16;      //4 pawns+  1 horse +  2 sliding;

        private static readonly int[] wTableIndexes = { 2, 3, 5, 6, 7 };
        private static readonly int[] bTableIndexes = { 10, 11, 13, 14, 15 };
        public static int[] pieceScoreArr = new int[]{
            0,
            1,//int.MaxValue/2,
            1,
            3,
            1, //NOT SUPPOSED TO BE ACCESSED
            3,
            5,
            9
            };
        public static Dictionary<int, int> pieceScore = new Dictionary<int, int>()
        {
            {0b000,pieceScoreArr[0]},
            {0b001,0},
            {0b010,pieceScoreArr[2]},
            {0b011,pieceScoreArr[3]},
            {0b101,pieceScoreArr[5]},
            {0b110,pieceScoreArr[6]},
            {0b111,pieceScoreArr[7]}
        };
        private static Dictionary<int, int> pieceScoreW = new Dictionary<int, int>()
        {
            {0b00000,pieceScoreArr[0]},
            {0b01001,pieceScoreArr[1]},
            {0b01010,pieceScoreArr[2]},
            {0b01011,pieceScoreArr[3]},
            {0b01101,pieceScoreArr[5]},
            {0b01110,pieceScoreArr[6]},
            {0b01111,pieceScoreArr[7]}
        };
        private static Dictionary<int, int> pieceScoreB = new Dictionary<int, int>()
        {
            {0b00000,pieceScoreArr[0]},
            {0b10001,pieceScoreArr[1]},
            {0b10010,pieceScoreArr[2]},
            {0b10011,pieceScoreArr[3]},
            {0b10101,pieceScoreArr[5]},
            {0b10110,pieceScoreArr[6]},
            {0b10111,pieceScoreArr[7]}
        };
        private static Dictionary<int, int> pieceScoreBW = new Dictionary<int, int>()
        {
            {0b00000,pieceScoreArr[0]},
            {0b01001,pieceScoreArr[1]},
            {0b01010,pieceScoreArr[2]},
            {0b01011,pieceScoreArr[3]},
            {0b01101,pieceScoreArr[5]},
            {0b01110,pieceScoreArr[6]},
            {0b01111,pieceScoreArr[7]},
            {0b10001,-pieceScoreArr[1]},
            {0b10010,-pieceScoreArr[2]},
            {0b10011,-pieceScoreArr[3]},
            {0b10101,-pieceScoreArr[5]},
            {0b10110,-pieceScoreArr[6]},
            {0b10111,-pieceScoreArr[7]}
        };
        public static readonly int[] pieceScoreBWArray =
        {
        0,0,0,0,0,0,0,0,0,
        pieceScoreArr[1],
        pieceScoreArr[2],
        pieceScoreArr[3],
        0,pieceScoreArr[5],
        pieceScoreArr[6],
        pieceScoreArr[7],
        0,-pieceScoreArr[1],
        -pieceScoreArr[2],
        -pieceScoreArr[3],
        0,-pieceScoreArr[5],
        -pieceScoreArr[6],
        -pieceScoreArr[7]
        };

        //! SHOULD ALWAYS USE THE BEST CAPTURE CALC VARIANT
        public int CaptureScore(Board board)
        {
            return V6CaptureScore(board);
        }
        //*Original
        //*Chris 100turn ms:{69, 58, 69, 65, 64, 65, 57, 55}
        //*SPEEDTST 100turn x 1000 ms: {2159, 2112, 2116, 2172, 2113, 2137, 2172}
        public int V1CaptureScore(int[] tiles, bool whiteTurn)
        {
            int score = 0;
            foreach (int tile in tiles)
            {
                score += (whiteTurn ? 1 : -1) * (((tile & COLOR_MASK) == WHITE) ? pieceScoreW[tile] : -pieceScoreB[tile]);

            }
            return score;

        }

        //*for, !foreach
        //*SPEEDTST 100turn x 1000 ms: :{2079, 2085, 2090, 2086, 2179, 2101, 2104}
        public int V2CaptureScore(int[] tiles, bool whiteTurn)
        {
            int score = 0;
            int tile = 0;
            for (int ii = 0; ii < 64; ii++)
            {
                tile = tiles[ii];
                score += (whiteTurn ? 1 : -1) * (((tile & COLOR_MASK) == WHITE) ? pieceScoreW[tile] : -pieceScoreB[tile]);
            }
            return score;

        }
        //*OneDict
        //*SPEEDTST 100turn x 1000 ms: {2050, 2046, 2047, 2056, 2057, 2055, 2051, 2065}
        public int V3CaptureScore(int[] tiles, bool whiteTurn)
        {
            int score = 0;
            for (int ii = 0; ii < 64; ii++)
                score += (whiteTurn ? 1 : -1) * pieceScoreBW[tiles[ii]];
            return score;

        }

        //*Unrolled
        //*SPEEDTST 100turn x 1000 ms: {2003, 1988, 1995, 2005}
        public int V4CaptureScore(int[] tiles, bool whiteTurn)
        {
            int score = 0;
            int factor = (whiteTurn ? 1 : -1);
            score += factor * pieceScoreBW[tiles[0]];
            score += factor * pieceScoreBW[tiles[1]];
            score += factor * pieceScoreBW[tiles[2]];
            score += factor * pieceScoreBW[tiles[3]];
            score += factor * pieceScoreBW[tiles[4]];
            score += factor * pieceScoreBW[tiles[5]];
            score += factor * pieceScoreBW[tiles[6]];
            score += factor * pieceScoreBW[tiles[7]];
            score += factor * pieceScoreBW[tiles[8]];
            score += factor * pieceScoreBW[tiles[9]];
            score += factor * pieceScoreBW[tiles[10]];
            score += factor * pieceScoreBW[tiles[11]];
            score += factor * pieceScoreBW[tiles[12]];
            score += factor * pieceScoreBW[tiles[13]];
            score += factor * pieceScoreBW[tiles[14]];
            score += factor * pieceScoreBW[tiles[15]];
            score += factor * pieceScoreBW[tiles[16]];
            score += factor * pieceScoreBW[tiles[17]];
            score += factor * pieceScoreBW[tiles[18]];
            score += factor * pieceScoreBW[tiles[19]];
            score += factor * pieceScoreBW[tiles[20]];
            score += factor * pieceScoreBW[tiles[21]];
            score += factor * pieceScoreBW[tiles[22]];
            score += factor * pieceScoreBW[tiles[23]];
            score += factor * pieceScoreBW[tiles[24]];
            score += factor * pieceScoreBW[tiles[25]];
            score += factor * pieceScoreBW[tiles[26]];
            score += factor * pieceScoreBW[tiles[27]];
            score += factor * pieceScoreBW[tiles[28]];
            score += factor * pieceScoreBW[tiles[29]];
            score += factor * pieceScoreBW[tiles[30]];
            score += factor * pieceScoreBW[tiles[31]];
            score += factor * pieceScoreBW[tiles[32]];
            score += factor * pieceScoreBW[tiles[33]];
            score += factor * pieceScoreBW[tiles[34]];
            score += factor * pieceScoreBW[tiles[35]];
            score += factor * pieceScoreBW[tiles[36]];
            score += factor * pieceScoreBW[tiles[37]];
            score += factor * pieceScoreBW[tiles[38]];
            score += factor * pieceScoreBW[tiles[39]];
            score += factor * pieceScoreBW[tiles[40]];
            score += factor * pieceScoreBW[tiles[41]];
            score += factor * pieceScoreBW[tiles[42]];
            score += factor * pieceScoreBW[tiles[43]];
            score += factor * pieceScoreBW[tiles[44]];
            score += factor * pieceScoreBW[tiles[45]];
            score += factor * pieceScoreBW[tiles[46]];
            score += factor * pieceScoreBW[tiles[47]];
            score += factor * pieceScoreBW[tiles[48]];
            score += factor * pieceScoreBW[tiles[49]];
            score += factor * pieceScoreBW[tiles[50]];
            score += factor * pieceScoreBW[tiles[51]];
            score += factor * pieceScoreBW[tiles[52]];
            score += factor * pieceScoreBW[tiles[53]];
            score += factor * pieceScoreBW[tiles[54]];
            score += factor * pieceScoreBW[tiles[55]];
            score += factor * pieceScoreBW[tiles[56]];
            score += factor * pieceScoreBW[tiles[57]];
            score += factor * pieceScoreBW[tiles[58]];
            score += factor * pieceScoreBW[tiles[59]];
            score += factor * pieceScoreBW[tiles[60]];
            score += factor * pieceScoreBW[tiles[61]];
            score += factor * pieceScoreBW[tiles[62]];
            score += factor * pieceScoreBW[tiles[63]];
            return score;

        }

        //*Unrolled using ARRAY
        public int V5CaptureScore(int[] tiles, bool whiteTurn)
        {
            int score = 0;
            int factor = (whiteTurn ? 1 : -1);
            score += factor * pieceScoreBWArray[tiles[0]];
            score += factor * pieceScoreBWArray[tiles[1]];
            score += factor * pieceScoreBWArray[tiles[2]];
            score += factor * pieceScoreBWArray[tiles[3]];
            score += factor * pieceScoreBWArray[tiles[4]];
            score += factor * pieceScoreBWArray[tiles[5]];
            score += factor * pieceScoreBWArray[tiles[6]];
            score += factor * pieceScoreBWArray[tiles[7]];
            score += factor * pieceScoreBWArray[tiles[8]];
            score += factor * pieceScoreBWArray[tiles[9]];
            score += factor * pieceScoreBWArray[tiles[10]];
            score += factor * pieceScoreBWArray[tiles[11]];
            score += factor * pieceScoreBWArray[tiles[12]];
            score += factor * pieceScoreBWArray[tiles[13]];
            score += factor * pieceScoreBWArray[tiles[14]];
            score += factor * pieceScoreBWArray[tiles[15]];
            score += factor * pieceScoreBWArray[tiles[16]];
            score += factor * pieceScoreBWArray[tiles[17]];
            score += factor * pieceScoreBWArray[tiles[18]];
            score += factor * pieceScoreBWArray[tiles[19]];
            score += factor * pieceScoreBWArray[tiles[20]];
            score += factor * pieceScoreBWArray[tiles[21]];
            score += factor * pieceScoreBWArray[tiles[22]];
            score += factor * pieceScoreBWArray[tiles[23]];
            score += factor * pieceScoreBWArray[tiles[24]];
            score += factor * pieceScoreBWArray[tiles[25]];
            score += factor * pieceScoreBWArray[tiles[26]];
            score += factor * pieceScoreBWArray[tiles[27]];
            score += factor * pieceScoreBWArray[tiles[28]];
            score += factor * pieceScoreBWArray[tiles[29]];
            score += factor * pieceScoreBWArray[tiles[30]];
            score += factor * pieceScoreBWArray[tiles[31]];
            score += factor * pieceScoreBWArray[tiles[32]];
            score += factor * pieceScoreBWArray[tiles[33]];
            score += factor * pieceScoreBWArray[tiles[34]];
            score += factor * pieceScoreBWArray[tiles[35]];
            score += factor * pieceScoreBWArray[tiles[36]];
            score += factor * pieceScoreBWArray[tiles[37]];
            score += factor * pieceScoreBWArray[tiles[38]];
            score += factor * pieceScoreBWArray[tiles[39]];
            score += factor * pieceScoreBWArray[tiles[40]];
            score += factor * pieceScoreBWArray[tiles[41]];
            score += factor * pieceScoreBWArray[tiles[42]];
            score += factor * pieceScoreBWArray[tiles[43]];
            score += factor * pieceScoreBWArray[tiles[44]];
            score += factor * pieceScoreBWArray[tiles[45]];
            score += factor * pieceScoreBWArray[tiles[46]];
            score += factor * pieceScoreBWArray[tiles[47]];
            score += factor * pieceScoreBWArray[tiles[48]];
            score += factor * pieceScoreBWArray[tiles[49]];
            score += factor * pieceScoreBWArray[tiles[50]];
            score += factor * pieceScoreBWArray[tiles[51]];
            score += factor * pieceScoreBWArray[tiles[52]];
            score += factor * pieceScoreBWArray[tiles[53]];
            score += factor * pieceScoreBWArray[tiles[54]];
            score += factor * pieceScoreBWArray[tiles[55]];
            score += factor * pieceScoreBWArray[tiles[56]];
            score += factor * pieceScoreBWArray[tiles[57]];
            score += factor * pieceScoreBWArray[tiles[58]];
            score += factor * pieceScoreBWArray[tiles[59]];
            score += factor * pieceScoreBWArray[tiles[60]];
            score += factor * pieceScoreBWArray[tiles[61]];
            score += factor * pieceScoreBWArray[tiles[62]];
            score += factor * pieceScoreBWArray[tiles[63]];
            return score;

        }

        //*Unrolled using PieceTables
        public int V6CaptureScore(Board board)
        {
            int eval = 0;
            int factor = board.whiteTurn ? 1 : -1;

            eval += factor * board.allPieceTables[wTableIndexes[0]].Count * pieceScoreArr[wTableIndexes[0]];
            eval += factor * board.allPieceTables[wTableIndexes[1]].Count * pieceScoreArr[wTableIndexes[1]];
            eval += factor * board.allPieceTables[wTableIndexes[2]].Count * pieceScoreArr[wTableIndexes[2]];
            eval += factor * board.allPieceTables[wTableIndexes[3]].Count * pieceScoreArr[wTableIndexes[3]];
            eval += factor * board.allPieceTables[wTableIndexes[4]].Count * pieceScoreArr[wTableIndexes[4]];

            eval -= factor * board.allPieceTables[bTableIndexes[0]].Count * pieceScoreArr[wTableIndexes[0]];
            eval -= factor * board.allPieceTables[bTableIndexes[1]].Count * pieceScoreArr[wTableIndexes[1]];
            eval -= factor * board.allPieceTables[bTableIndexes[2]].Count * pieceScoreArr[wTableIndexes[2]];
            eval -= factor * board.allPieceTables[bTableIndexes[3]].Count * pieceScoreArr[wTableIndexes[3]];
            eval -= factor * board.allPieceTables[bTableIndexes[4]].Count * pieceScoreArr[wTableIndexes[4]];
            return eval;
        }

        public int EndgameValue(int[] tiles, bool whiteTurn)
        {
            int score = 0;
            int factor = (whiteTurn ? 1 : -1);
            int n = 0;
            for (int ii = 0; ii < 64; ii++)
            {
                n = factor * pieceScoreBWArray[tiles[ii]];
                if (n < 0)
                    score -= n;
            }

            return score < ENDGAME_START_VALUE ? ENDGAME_START_VALUE - score : 0;
        }

        //*Unrolled using PieceTables
        public int EndgameValueV2(Board board)
        {
            int eval = 0;
            if (!board.whiteTurn)
            {
                eval += board.allPieceTables[wTableIndexes[0]].Count * pieceScoreArr[wTableIndexes[0]];
                eval += board.allPieceTables[wTableIndexes[1]].Count * pieceScoreArr[wTableIndexes[1]];
                eval += board.allPieceTables[wTableIndexes[2]].Count * pieceScoreArr[wTableIndexes[2]];
                eval += board.allPieceTables[wTableIndexes[3]].Count * pieceScoreArr[wTableIndexes[3]];
                eval += board.allPieceTables[wTableIndexes[4]].Count * pieceScoreArr[wTableIndexes[4]];
            }
            else
            {
                eval += board.allPieceTables[bTableIndexes[0]].Count * pieceScoreArr[wTableIndexes[0]];
                eval += board.allPieceTables[bTableIndexes[1]].Count * pieceScoreArr[wTableIndexes[1]];
                eval += board.allPieceTables[bTableIndexes[2]].Count * pieceScoreArr[wTableIndexes[2]];
                eval += board.allPieceTables[bTableIndexes[3]].Count * pieceScoreArr[wTableIndexes[3]];
                eval += board.allPieceTables[bTableIndexes[4]].Count * pieceScoreArr[wTableIndexes[4]];
            }
            return eval < ENDGAME_START_VALUE ? ENDGAME_START_VALUE - eval : 0;
        }

        public int PawnStructureValueV1(Board board)
        {
            int val = 0;
            int colorIndex = (board.whiteTurn ? 0 : 1);
            int leftAttack = (board.whiteTurn ? MoveData.PawnAttackLeftW : MoveData.PawnAttackLeftB);
            int rightAttack = (board.whiteTurn ? MoveData.PawnAttackRightW : MoveData.PawnAttackRightB);
            PieceTable pawns = board.pawns[colorIndex];
            for (int ii = 0; ii < pawns.Count; ii++)
            {
                int l = MoveData.pawnMoveMap[pawns[ii]][leftAttack];
                int r = MoveData.pawnMoveMap[pawns[ii]][rightAttack];
                if (l != -1)
                    val += pieceScoreArr[Piece.PieceType(board.tiles[l])];
                if (r != -1)
                    val += pieceScoreArr[Piece.PieceType(board.tiles[r])];

            }
            return val;

        }
    }
}