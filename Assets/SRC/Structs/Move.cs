using System.Collections;
using System.Collections.Generic;

//TODO Optimize move
//https://github.com/SebLague/Chess-AI/blob/main/Assets/Scripts/Core/Move.cs
public readonly struct Move
{

    public readonly struct Flag
    {
        public const int None = 0;
        public const int EnPassantCapture = 1;
        public const int Castling = 2;
        public const int PromoteToQueen = 3;
        public const int PromoteToKnight = 4;
        public const int PromoteToRook = 5;
        public const int PromoteToBishop = 6;
        public const int PawnTwoForward = 7;
    }

    readonly ushort moveValue;
    const ushort startSquareMask = 0b0000000000111111;
    const ushort targetSquareMask = 0b0000111111000000;
    const ushort flagMask = 0b1111000000000000;

    public Move(ushort moveValue)
    {
        this.moveValue = moveValue;

    }
    public Move(int startSquare, int targetSquare)
    {
        moveValue = (ushort)(startSquare | targetSquare << 6);
    }
    public Move(int startSquare, int targetSquare, int flag)
    {
        moveValue = (ushort)(startSquare | targetSquare << 6 | flag << 12);
    }
    public int StartSquare
    {
        get { return moveValue & startSquareMask; }
    }
    public int TargetSquare
    {
        get { return (moveValue & targetSquareMask) >> 6; }
    }
    public int moveFlag
    {
        get { return moveValue >> 12; }
    }

    public override bool Equals(object obj)
    {

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        Move m = (Move)obj;
        return StartSquare == m.StartSquare && TargetSquare == m.TargetSquare && moveFlag == m.moveFlag;
    }
    public bool Equals(Move m)
    {
        return StartSquare == m.StartSquare && TargetSquare == m.TargetSquare && moveFlag == m.moveFlag;
    }

    public bool isPromotion()
    {
        int moveflag = moveFlag;
        return (moveflag == Flag.PromoteToQueen || moveflag == Flag.PromoteToRook || moveflag == Flag.PromoteToBishop || moveflag == Flag.PromoteToKnight);
    }
    public ushort MoveValue { get { return moveValue; } }
    //!DEPRECATED
    public short toShort()
    {
        short _move = ((short)moveValue);
        return _move;
    }

}