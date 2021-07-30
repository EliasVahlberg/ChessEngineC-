using System.Collections.Generic;

public static class Piece
{
    public static string[] PositionRepresentation = {
    "a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1",
    "a2", "b2", "c2", "d2", "e2", "f2", "g2", "h2",
    "a3", "b3", "c3", "d3", "e3", "f3", "g3", "h3",
    "a4", "b4", "c4", "d4", "e4", "f4", "g4", "h4",
    "a5", "b5", "c5", "d5", "e5", "f5", "g5", "h5",
    "a6", "b6", "c6", "d6", "e6", "f6", "g6", "h6",
    "a7", "b7", "c7", "d7", "e7", "f7", "g7", "h7",
    "a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8"};

    public static Dictionary<int, int> PieceValueDictionary = new Dictionary<int, int>(){
            {NONE,0},
            {KING,0},
            {PAWN,1},
            {KNIGHT, 3},
            {BISHOP, 3},
            {ROOK, 5},
            {QUEEN, 9}
            };

    public const int NONE = 0;
    public const int KING = 1;
    public const int PAWN = 2;
    public const int KNIGHT = 3;
    public const int BISHOP = 5;
    public const int ROOK = 6;
    public const int QUEEN = 7;

    public const int WHITE = 8;
    public const int BLACK = 16;

    const int typeMask = 0b00111;
    const int blackMask = 0b10000;
    const int whiteMask = 0b01000;
    const int colourMask = whiteMask | blackMask;

    public static bool IsColour(int piece, int colour)
    {
        return (piece & colourMask) == colour;
    }
    public static bool IsBlack(int piece)
    {
        return IsColour(piece, BLACK);
    }
    public static bool IsWhite(int piece)
    {
        return IsColour(piece, WHITE);
    }
    public static bool IsType(int piece, int type)
    {
        return PieceType(piece) == type;
    }

    public static int Colour(int piece)
    {
        return piece & colourMask;
    }

    public static int PieceType(int piece)
    {
        return piece & typeMask;
    }

    public static bool IsRookOrQueen(int piece)
    {
        return (piece & 0b110) == 0b110;
    }

    public static bool IsBishopOrQueen(int piece)
    {
        return (piece & 0b101) == 0b101;
    }

    public static bool IsSlidingPiece(int piece)
    {
        return (piece & 0b100) != 0;
    }
    public static bool CanSlideInDirection(int piece, int dir)
    {
        return (IsSlidingPiece(piece) && ((IsBishopOrQueen(piece) && dir >= 4) || (IsSlidingPiece(piece) && IsRookOrQueen(piece) && dir < 4)));
    }
    //Seen from oposite side
    public static bool IsPawnAttackDirection(int color, int dir)
    {
        return (color != WHITE ?
        (dir == MoveData.NORTHEAST_I || dir == MoveData.NORTHWEST_I)
        : dir == MoveData.SOUTHEAST_I || dir == MoveData.SOUTHWEST_I);
    }
    public static int File(int position)
    {
        return position % 8;
    }
    public static int Rank(int position)
    {
        return position / 8;
    }
}
