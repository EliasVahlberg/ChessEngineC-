using UnityEngine;

/*
    @File GameState.cs
    @author Elias Vahlberg 
    @Date 2021-07
*/
public struct GameState
{
    //27-32     28      27      22-26   16-22       10-16     4-10        0-4  
    //0000      0       0       0000    000000      000000    000000      0000
    //Unused    EPCap   WTurn   CapT    CapI        FiftyT    EnPassant   Castle
    public const uint whiteCastleKingsideMask = 0b0001U;
    public const uint whiteCastleQueensideMask = 0b0010U;
    public const uint blackCastleKingsideMask = 0b0100U;
    public const uint blackCastleQueensideMask = 0b1000U;
    public const uint castleMask = 0b1111U;
    public const uint enPassantMask = 0b1111110000U;
    public const uint fiftyTurnMask = 0b1111110000000000U;
    public const uint prevCapturedIndexMask = 0b1111110000000000000000U;
    public const uint prevCapturedTypeMask = 0b11110000000000000000000000U;
    public const uint whiteTurnMask = 0b100000000000000000000000000U;

    private uint gameState;
    public uint gameStateValue
    {
        get { return gameState; }
        set { gameState = value; }
    }

    private Move prevMove;
    public Move PrevMove
    {
        get => prevMove; set => prevMove = value;
    }

    public GameState(uint castleRights, int enPassant, int fiftyTurn, int prevCapI, int prevCapT, Move prevMove)
    {
        gameState = 0;
        this.prevMove = prevMove;
        SetCastleRights(castleRights);
        SetEnPassant(enPassant);
        SetFiftyTurnCount(fiftyTurn);
        SetPrevCapturedIndex(prevCapI);
        SetPrevCapturedType(prevCapT);

    }
    public GameState(uint castleRights, int enPassant, int fiftyTurn, int prevCapI, int prevCapT)
    {
        gameState = 0;
        this.prevMove = new Move(0);
        SetCastleRights(castleRights);
        SetEnPassant(enPassant);
        SetFiftyTurnCount(fiftyTurn);
        SetPrevCapturedIndex(prevCapI);
        SetPrevCapturedType(prevCapT);
    }
    public GameState(uint gameState, Move prevMove)
    {
        this.gameState = gameState;
        this.prevMove = prevMove;
    }
    public GameState(uint gameState)
    {
        this.gameState = gameState;
        prevMove = new Move(0);
    }
    #region Castle
    public bool WhiteCastleKingside()
    {
        return (gameState & whiteCastleKingsideMask) == whiteCastleKingsideMask;
    }

    public bool WhiteCastleQueenside()
    {
        return (gameState & whiteCastleQueensideMask) == whiteCastleQueensideMask;
    }

    public bool BlackCastleKingside()
    {
        return (gameState & blackCastleKingsideMask) == blackCastleKingsideMask;
    }

    public bool BlackCastleQueenside()
    {
        return (gameState & blackCastleQueensideMask) == blackCastleQueensideMask;
    }

    public uint CastleRights()
    {
        return gameState & castleMask;
    }

    public void SetWhiteCastleKingside(bool val)
    {
        gameState = val ? gameState | whiteCastleKingsideMask : gameState & (~whiteCastleKingsideMask);

    }

    public void SetWhiteCastleQueenside(bool val)
    {
        gameState = val ? gameState | whiteCastleQueensideMask : gameState & (~whiteCastleQueensideMask);
    }

    public void SetBlackCastleKingside(bool val)
    {
        gameState = val ? gameState | blackCastleKingsideMask : gameState & (~blackCastleKingsideMask);
    }

    public void SetBlackCastleQueenside(bool val)
    {
        gameState = val ? gameState | blackCastleQueensideMask : gameState & (~blackCastleQueensideMask);
    }

    public void SetCastleRights(uint val)
    {
        gameState = (uint)((gameState & (~castleMask)) | (castleMask & val));
    }

    #endregion

    #region EnPassant
    public int EnPassant()
    {
        return checked((int)(gameState & enPassantMask) >> 4);
    }
    public void SetEnPassant(int val)
    {
        gameState = (uint)((gameState & (~enPassantMask)) | (enPassantMask & (val << 4)));
    }
    #endregion

    #region FiftyTurn
    public int FiftyTurnCount()
    {
        return checked((int)(gameState & fiftyTurnMask) >> 10);
    }
    public void SetFiftyTurnCount(int val)
    {
        gameState = (uint)((gameState & (~fiftyTurnMask)) | (fiftyTurnMask & (val << 10)));
    }
    public void IncrementFiftyTurnCount()
    {

        gameState = (uint)((gameState & (~fiftyTurnMask)) | (fiftyTurnMask & (FiftyTurnCount() + 1 << 10)));
    }
    #endregion

    #region PrevCapturedIndex
    public int PrevCapturedIndex()
    {
        return checked((int)(gameState & prevCapturedIndexMask) >> 16);
    }
    public void SetPrevCapturedIndex(int val)
    {
        gameState = (uint)((gameState & (~prevCapturedIndexMask)) | (prevCapturedIndexMask & (val << 16)));
    }
    #endregion

    #region PrevCapturedType
    public int PrevCapturedType()
    {
        return checked((int)(gameState & prevCapturedTypeMask) >> 22);
    }
    public void SetPrevCapturedType(int val)
    {
        gameState = (uint)((gameState & (~prevCapturedTypeMask)) | (prevCapturedTypeMask & (val << 22)));
    }
    #endregion

    #region WhiteTurn
    public bool WhiteTurn()
    {
        return (gameState & whiteTurnMask) == whiteTurnMask;
    }
    public void SetWhiteTurn(bool val)
    {
        gameState = val ? gameState | whiteTurnMask : gameState & (~whiteTurnMask);
    }
    #endregion

    public override string ToString()
    {
        string s = "GameState:{" + gameState;
        s += "\n CastleRights =" + CastleRights();
        s += "\n EnPassant =" + EnPassant();
        s += "\n FiftyTurnCount =" + FiftyTurnCount();
        s += "\n PrevCapturedIndex =" + PrevCapturedIndex();
        s += "\n PrevCapturedType =" + PrevCapturedType();
        s += "\n WhiteTurn =" + WhiteTurn() + " }";
        return s;
    }
}