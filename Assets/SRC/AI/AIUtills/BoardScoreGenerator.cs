
using System.Collections.Generic;

namespace ChessAI
{
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
        public static int captureWeight = 1;
        public static int[] pieceScoreArr = new int[]{
            0*captureWeight,
            int.MaxValue/2,
            1*captureWeight,
            3*captureWeight,
            3*captureWeight,
            5*captureWeight,
            9*captureWeight
            };
        private static Dictionary<int, int> pieceScoreW = new Dictionary<int, int>()
        {
            {0b00000,pieceScoreArr[0]},
            {0b01001,pieceScoreArr[1]},
            {0b01010,pieceScoreArr[2]},
            {0b01011,pieceScoreArr[3]},
            {0b01101,pieceScoreArr[4]},
            {0b01110,pieceScoreArr[5]},
            {0b01111,pieceScoreArr[6]}
        };
        private static Dictionary<int, int> pieceScoreB = new Dictionary<int, int>()
        {
            {0b00000,pieceScoreArr[0]},
            {0b10001,pieceScoreArr[1]},
            {0b10010,pieceScoreArr[2]},
            {0b10011,pieceScoreArr[3]},
            {0b10101,pieceScoreArr[4]},
            {0b10110,pieceScoreArr[5]},
            {0b10111,pieceScoreArr[6]}
        };
        public int CaptureScore(int[] tiles, bool whiteTurn)
        {
            int score = 0;

            foreach (int tile in tiles)
                score += (whiteTurn ? 1 : -1) *
                (((tile & COLOR_MASK) == WHITE) ?
                    pieceScoreW[tile] : -pieceScoreB[tile]);
            return score;

        }
    }
}