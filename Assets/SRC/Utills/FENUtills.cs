using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FENUtills
{
    public const string DEFAULT_START_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public static Dictionary<char, int> symbolTypeDict = new Dictionary<char, int>()
    {
        ['k'] = Piece.KING,
        ['p'] = Piece.PAWN,
        ['n'] = Piece.KNIGHT,
        ['b'] = Piece.BISHOP,
        ['r'] = Piece.ROOK,
        ['q'] = Piece.QUEEN
    };
    public static Dictionary<int, char> typeSymbolDict = new Dictionary<int, char>()
    {
        [Piece.KING] = 'k',
        [Piece.PAWN] = 'p',
        [Piece.KNIGHT] = 'n',
        [Piece.BISHOP] = 'b',
        [Piece.ROOK] = 'r',
        [Piece.QUEEN] = 'q'
    };
    public static Dictionary<int, string> failTypeDict = new Dictionary<int, string>()
    {
        [0] = "NO FAIL",
        [-1] = "FILE FAIL",
        [-2] = "PIECE DICTIONARY FAIL",
        [-3] = "NUMBER OF SECTIONS FAIL",
        [-4] = "INVALID TURN CHARACTER FAIL",
        [-5] = "INVALID SECTION LENGTH",
        [-6] = "INVALID CASTELING CHARACTER FAIL",
        [-7] = "INT PARSE FAIL",
        [-8] = "FIFTY TURN RULE FAIL",
        [-9] = "NEGATIVE PLY FAIL",
        [-10] = "EXCEPTION THROWN FAIL"
    };
    public static readonly char[] validInSection0 = new char[] { '/', 'k', 'p', 'n', 'b', 'r', 'q' };
    public static readonly char[] validInSection1 = new char[] { 'w', 'b' };
    public static readonly char[] validInSection2 = new char[] { '-', 'k', 'q' };
    public static readonly char[] validInSection3 = new char[] { '-', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
    public static GameStateInfo generatePiecePositions(string fen)
    {
        GameStateInfo stateInfo = new GameStateInfo();
        int[] squareGrid = new int[64];
        string[] sections = fen.Split(' ');
        int file = 0;
        int rank = 7;
        foreach (char symbol in sections[0])
        {
            if (symbol == '/')
            {
                file = 0;
                rank--;
            }
            else
            {
                if (char.IsDigit(symbol))
                {
                    file += (int)char.GetNumericValue(symbol);
                }
                else
                {
                    stateInfo.squareGrid[file + rank * 8] = symbolTypeDict[char.ToLower(symbol)] | (char.IsUpper(symbol) ? Piece.WHITE : Piece.BLACK);

                    file++;
                }
            }

        }
        stateInfo.whiteTurn = (sections[1] == "w");
        string castlingRights = (sections.Length > 2) ? sections[2] : "KQkq";
        stateInfo.whiteCastleKingside = castlingRights.Contains("K");
        stateInfo.whiteCastleQueenside = castlingRights.Contains("Q");
        stateInfo.blackCastleKingside = castlingRights.Contains("k");
        stateInfo.blackCastleQueenside = castlingRights.Contains("q");

        if (sections.Length > 3)
        {
            int epIndex = BoardUtills.IndexFromString(sections[3]);
            if (epIndex != -1)
            {
                stateInfo.epindex = epIndex;
            }
        }
        if (sections.Length > 4)
        {
            int.TryParse(sections[4], out stateInfo.fiftyCount);
        }
        if (sections.Length > 5)
        {
            int.TryParse(sections[5], out stateInfo.plyCount);
        }
        return stateInfo;

    }
    public static string generateFEN(GameStateInfo gameStateInfo)
    {
        string fen = "";
        for (int rank = 7; rank >= 0; rank--)
        {
            int nEmptyFiles = 0;
            for (int file = 0; file < 8; file++)
            {
                int pos = rank * 8 + file;
                int piece = gameStateInfo.squareGrid[pos];
                if (piece == 0)
                {
                    nEmptyFiles++;
                    if (file == 7)
                    {
                        fen += nEmptyFiles;
                        nEmptyFiles = 0;
                    }
                    continue;
                }
                else if (nEmptyFiles != 0)
                {
                    fen += nEmptyFiles;
                    nEmptyFiles = 0;
                }

                if ((!typeSymbolDict.ContainsKey(Piece.PieceType(piece))))
                    Debug.Log("NOT FOUND: " + piece);
                fen += Piece.IsBlack(piece) ? typeSymbolDict[Piece.PieceType(piece)] : char.ToUpper(typeSymbolDict[Piece.PieceType(piece)]);

            }
            if (rank != 0)
            {
                fen += '/';
            }
        }
        fen += ' ';
        fen += (gameStateInfo.whiteTurn) ? 'w' : 'b';
        fen += ' ';
        fen += (gameStateInfo.whiteCastleKingside) ? "K" : "";
        fen += (gameStateInfo.whiteCastleQueenside) ? "Q" : "";
        fen += (gameStateInfo.blackCastleKingside) ? "k" : "";
        fen += (gameStateInfo.blackCastleQueenside) ? "q" : "";
        //fen += ((gameStateInfo.plyCount & 15) == 0) ? "-" : "";

        fen += ' ';
        if (gameStateInfo.epindex == 0)
        {
            fen += '-';
        }
        else
        {
            string epPos = BoardUtills.stringFromIndex(gameStateInfo.epindex);
            fen += epPos;
        }
        fen += ' ';
        fen += gameStateInfo.fiftyCount;
        fen += ' ';
        fen += gameStateInfo.plyCount;
        //TODO
        // Castling
        // En-passant
        // 50 move counter
        // Full-move count (should be one at start, and increase after each move by black)
        return fen;
    }
    public static int[] validFen(string fen)
    {
        int sectionsCleared = 0;
        try
        {
            int[] squareGrid = new int[64];
            string[] sections = fen.Split(' ');
            int file = 0;
            int rank = 7;
            foreach (char symbol in sections[0])
            {
                if (symbol == '/')
                {
                    file = 0;
                    rank--;
                }
                else
                {
                    if (char.IsDigit(symbol))
                    {
                        int empty = (int)char.GetNumericValue(symbol);
                        if (file + empty > 8)
                        {
                            Debug.Log("Rank: " + rank + ", " + file + ", " + empty);
                            return new int[] { 0, -1 };
                        }
                        file += empty;
                    }
                    else
                    {
                        if (!symbolTypeDict.ContainsKey(char.ToLower(symbol)))
                            return new int[] { 0, -2 };
                        squareGrid[file + rank * 8] = symbolTypeDict[char.ToLower(symbol)] | (char.IsUpper(symbol) ? Piece.WHITE : Piece.BLACK);

                        file++;
                    }
                }

            }
            sectionsCleared++;
            if ((sections.Length < 2))
                return new int[] { sectionsCleared, -3 };
            string vs1 = validInSection1.ToString();
            if (char.ToLower(sections[1][0]) != 'w' && char.ToLower(sections[1][0]) != 'b')
                return new int[] { sectionsCleared, -4 };
            sectionsCleared++;

            if ((sections.Length < 3))
                return new int[] { sectionsCleared, -3 };
            string castlingRights = (sections.Length > 2) ? sections[2] : "KQkq";
            if (castlingRights.Length > 4)
                return new int[] { sectionsCleared, -5 };
            bool cas = true;

            Array.Find<char>(sections[2].ToCharArray(), c => (Array.Find<char>(validInSection2, c2 => c2 == c) != ('\0')));
            foreach (char c in sections[2].ToCharArray())
                if (Array.Find<char>(validInSection2, c2 => c2 == char.ToLower(c)) == ('\0'))
                { cas = false; }
            if (!cas)
                return new int[] { sectionsCleared, -6 };
            sectionsCleared++;

            if ((sections.Length <= 4))
                return new int[] { sectionsCleared, -3 };
            int plyCount;
            int fullPly;
            if (!int.TryParse(sections[4], out plyCount))
                return new int[] { sectionsCleared, -7 };
            if (plyCount >= 50)
                return new int[] { sectionsCleared, -8 };

            sectionsCleared++;

            if ((sections.Length <= 4))
                return new int[] { sectionsCleared, -3 };
            if (!int.TryParse(sections[5], out fullPly))
                return new int[] { sectionsCleared, -7 };
            if (fullPly < 0)
                return new int[] { sectionsCleared, -9 };
            sectionsCleared++;
            return new int[] { sectionsCleared, 0 };


        }
        catch (System.Exception)
        {
            return new int[] { sectionsCleared, -10 };
        }
    }
    public class GameStateInfo
    {
        public int[] squareGrid;
        public bool whiteTurn;
        //TODO
        public int fiftyCount;
        public int plyCount;
        public bool whiteCastleKingside;
        public bool whiteCastleQueenside;
        public bool blackCastleKingside;
        public bool blackCastleQueenside;
        public int epindex;

        public GameStateInfo()
        {
            squareGrid = new int[64];
        }
    }
}
