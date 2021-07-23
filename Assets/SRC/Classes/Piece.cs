public static class Piece
{
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
