using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MoveData;
using static Piece;

public class MoveUtills
{
    #region New
    public List<Move> Moves;
    public Board board;
    public bool genNonCaptures;

    private int[][] pinnedMapBlack;
    private int[][] pinnedMapWhite;
    private bool whiteTurn;
    private int color;
    private int opponentColor;
    private int colorIndex;
    private int opponentColorIndex;
    private int kingSquare;

    private ulong currentAttackMap;
    private ulong currentAttackMapNoPawns;
    private ulong currentSlidingAttackMap;
    private ulong currentKnightAttackMap;
    private ulong currentPawnAttackMap;
    private ulong currentKingAttackMap;
    private ulong checkRayMap;
    private ulong pinRayMap;

    private bool inCheck;
    private bool inDoubleCheck;
    private bool pinsExist;

    public bool InCheck
    { get { return inCheck; } }
    public ulong CurrentAttackMap
    { get { return currentAttackMap; } }



    public void Generate(Board board)
    {
        this.board = board;
        Moves = new List<Move>();
        whiteTurn = board.whiteTurn;
        color = whiteTurn ? WHITE : BLACK;
        opponentColor = whiteTurn ? BLACK : WHITE;
        colorIndex = whiteTurn ? 0 : 1;
        opponentColorIndex = whiteTurn ? 1 : 0;
        kingSquare = board.KingSquares[colorIndex];
        inCheck = false;
        inDoubleCheck = false;
        genNonCaptures = true;
        pinsExist = false;
        generateAttackMaps();
        GenerateMoves();

    }

    public void GenerateCaptureMoves(Board board)
    {
        this.board = board;
        Moves = new List<Move>();
        whiteTurn = board.whiteTurn;
        color = whiteTurn ? WHITE : BLACK;
        opponentColor = whiteTurn ? BLACK : WHITE;
        colorIndex = whiteTurn ? 0 : 1;
        opponentColorIndex = whiteTurn ? 1 : 0;
        kingSquare = board.KingSquares[colorIndex];
        inCheck = false;
        inDoubleCheck = false;
        genNonCaptures = false;
        generateAttackMaps();
        GenerateMoves();

    }

    private void GenerateMoves()
    {
        //if (inCheck)
        //    Debug.Log("IN CHECK");
        //if (pinsExist)
        //    Debug.Log("PIN EXISTS");
        //if (inDoubleCheck)
        //    Debug.Log("IN DOUBLE CHECK");

        GenerateKingMoves();
        if (inDoubleCheck)
            return;
        GenerateSlidingMoves();
        GenerateKnightMoves();
        GeneratePawnMoves();


    }

    private void GenerateSlidingMoves()
    {
        PieceTable rooks = board.rooks[colorIndex];
        for (int i = 0; i < rooks.Count; i++)
        {
            GenerateSlidingPieceMoves(rooks[i], 0, 4);
        }

        PieceTable bishops = board.bishops[colorIndex];
        for (int i = 0; i < bishops.Count; i++)
        {
            GenerateSlidingPieceMoves(bishops[i], 4, 8);
        }

        PieceTable queens = board.queens[colorIndex];
        for (int i = 0; i < queens.Count; i++)
        {
            GenerateSlidingPieceMoves(queens[i], 0, 8);
        }

    }

    private void GenerateKnightMoves()
    {
        PieceTable knights = board.knights[colorIndex];

        for (int i = 0; i < knights.Count; i++)
        {
            int position = knights[i];

            // Knight cannot move if it is pinned
            if (IsPinned(position))
            {
                continue;
            }
            for (int ii = 0; ii < knightMovesMap[position].Length; ii++)
            {
                int target = knightMovesMap[position][ii];
                int targetPiece = board.tiles[target];
                bool isCapture = Piece.IsColour(targetPiece, opponentColor);
                if (genNonCaptures || isCapture)
                {
                    // Skip if square contains friendly piece, or if in check and knight is not interposing/capturing checking piece
                    if (Piece.IsColour(targetPiece, color) || (inCheck && !InCheckRay(target)))
                    {
                        continue;
                    }
                    Moves.Add(new Move(position, target));
                }
            }
        }
    }

    private void GeneratePawnMoves()
    {
        PieceTable myPawns = board.pawns[colorIndex];

        for (int ii = 0; ii < myPawns.Count; ii++)
            GeneratePawnPieceMoves(myPawns[ii]);

    }

    public void GenerateKingMoves()
    {
        for (int i = 0; i < kingMovesMap[kingSquare].Length; i++)
        {
            int target = kingMovesMap[kingSquare][i];
            int pieceOnTarget = board.tiles[target];

            // Skip squares occupied by friendly pieces
            if (Piece.IsColour(pieceOnTarget, color))
            {
                continue;
            }

            bool isCapture = Piece.IsColour(pieceOnTarget, opponentColor);
            if (!isCapture)
            {
                // King can't move to square marked as under enemy control, unless he is capturing that piece
                // Also skip if not generating quiet moves
                if (!genNonCaptures || InCheckRay(target))
                {
                    continue;
                }
            }

            // Safe for king to move to this square
            if (isSafePosition(target))
            {
                Moves.Add(new Move(kingSquare, target));

                // Castling:

            }
        }
        if (!inCheck)
            GenerateCastelingMoves();
    }

    #region MapGenerators
    private void generateAttackMaps()
    {
        currentAttackMap = 0;
        currentAttackMapNoPawns = 0;
        pinRayMap = 0;
        generateSlidingAttackMap();
        //generateCheckingRayMap();
        generateKnightAttackMap();
        generatePawnAttackMap();
        generateKingAttackMap();

        currentAttackMapNoPawns = currentSlidingAttackMap | currentKnightAttackMap | currentKingAttackMap;
        currentAttackMap = currentAttackMapNoPawns | currentPawnAttackMap;
    }

    private void generateSlidingAttackMap()
    {
        currentSlidingAttackMap = 0;
        PieceTable enemyRooks = board.rooks[opponentColorIndex];
        for (int i = 0; i < enemyRooks.Count; i++)
        {
            AddSlidingPieceToAttackMap(enemyRooks[i], 0, 4);
        }
        PieceTable enemyQueens = board.queens[opponentColorIndex];
        for (int i = 0; i < enemyQueens.Count; i++)
        {
            AddSlidingPieceToAttackMap(enemyQueens[i], 0, 8);
        }
        PieceTable enemyBishops = board.bishops[opponentColorIndex];
        for (int i = 0; i < enemyBishops.Count; i++)
        {
            AddSlidingPieceToAttackMap(enemyBishops[i], 4, 8);
        }

        //*Dirty OPTIMIZATION
        int startDir = 0;
        int endDir = 8;
        checkRayMap = 0;
        if (board.queens[opponentColorIndex].Count == 0)
        {
            startDir = (board.rooks[opponentColorIndex].Count > 0) ? 0 : 4;
            endDir = (board.bishops[opponentColorIndex].Count > 0) ? 8 : 4;
        }
        for (int dir = startDir; dir < endDir; dir++)
        {
            bool isDiagonal = dir > 3;
            int n = numSquaresToEdge[kingSquare][dir];
            int dirOffset = directionOffsets[dir];
            bool firendlyOnRay = false;
            ulong rayMask = 0;
            for (int i = 0; i < n; i++)
            {
                int squareIndex = kingSquare + dirOffset * (i + 1);
                rayMask |= 1ul << squareIndex;
                int piece = board.tiles[squareIndex];

                if (piece != NONE)
                {
                    if (Piece.IsColour(piece, color))
                    {
                        if (!firendlyOnRay)
                        {
                            firendlyOnRay = true;
                        }
                        else
                        {
                            //NOPIN
                            break;
                        }
                    }
                    else
                    {
                        int pieceType = Piece.PieceType(piece);

                        if (isDiagonal && Piece.IsBishopOrQueen(pieceType) || !isDiagonal && Piece.IsRookOrQueen(pieceType))
                        {
                            if (firendlyOnRay)
                            {
                                pinsExist = true;
                                pinRayMap |= rayMask;
                                //PINNED
                            }
                            else
                            {
                                checkRayMap |= rayMask;
                                inDoubleCheck = inCheck;
                                inCheck = true;
                            }
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (inDoubleCheck)
                {
                    break;
                }
            }
        }
    }

    private void AddSlidingPieceToAttackMap(int startSquare, int startDirIndex, int endDirIndex)
    {

        for (int directionIndex = startDirIndex; directionIndex < endDirIndex; directionIndex++)
        {
            int currentDirOffset = directionOffsets[directionIndex];
            for (int n = 0; n < numSquaresToEdge[startSquare][directionIndex]; n++)
            {
                int targetSquare = startSquare + currentDirOffset * (n + 1);
                int targetSquarePiece = board.tiles[targetSquare];
                currentSlidingAttackMap |= 1ul << targetSquare;
                if (targetSquare != kingSquare)
                {
                    if (targetSquarePiece != NONE)
                    {
                        break;
                    }
                }
            }
        }
    }

    private void generateKnightAttackMap()
    {
        PieceTable opponentKnights = board.knights[opponentColorIndex];
        currentKnightAttackMap = 0;
        bool isKnightCheck = false;

        for (int knightIndex = 0; knightIndex < opponentKnights.Count; knightIndex++)
        {
            int startSquare = opponentKnights[knightIndex];
            currentKnightAttackMap |= knightAttackBitboards[startSquare];

            if (!isKnightCheck && BoardUtills.ContainsTile(currentKnightAttackMap, kingSquare))
            {
                isKnightCheck = true;
                inDoubleCheck = inCheck; // if already in check, then this is double check
                inCheck = true;
                checkRayMap |= 1ul << startSquare;
            }

        }
    }

    //!DEPRECATED
    private void generateCheckingRayMap()
    {
        int startDirIndex = 0;
        int endDirIndex = 8;
        checkRayMap = 0;

        if (board.queens[opponentColorIndex].Count == 0)
        {
            startDirIndex = (board.rooks[opponentColorIndex].Count > 0) ? 0 : 4;
            endDirIndex = (board.bishops[opponentColorIndex].Count > 0) ? 8 : 4;
        }
        for (int dir = startDirIndex; dir < endDirIndex; dir++)
        {
            bool isDiagonal = dir > 3;

            int n = numSquaresToEdge[kingSquare][dir];
            int directionOffset = directionOffsets[dir];
            bool isFriendlyPieceAlongRay = false;
            ulong rayMask = 0;

            for (int i = 0; i < n; i++)
            {
                int squareIndex = kingSquare + directionOffset * (i + 1);
                rayMask |= 1ul << squareIndex;
                int piece = board.tiles[squareIndex];

                // This square contains a piece
                if (piece != NONE)
                {
                    if (Piece.IsColour(piece, color))
                    {
                        // First friendly piece we have come across in this direction, so it might be pinned
                        if (!isFriendlyPieceAlongRay)
                        {
                            isFriendlyPieceAlongRay = true;
                        }
                        // This is the second friendly piece we've found in this direction, therefore pin is not possible
                        else
                        {
                            break;
                        }
                    }
                    // This square contains an enemy piece
                    else
                    {
                        int pieceType = Piece.PieceType(piece);

                        // Check if piece is in bitmask of pieces able to move in current direction
                        if (isDiagonal && Piece.IsBishopOrQueen(pieceType) || !isDiagonal && Piece.IsRookOrQueen(pieceType))
                        {
                            // Friendly piece blocks the check, so this is a pin
                            if (isFriendlyPieceAlongRay)
                            {
                                pinsExist = true;
                                pinRayMap |= rayMask;
                            }
                            // No friendly piece blocking the attack, so this is a check
                            else
                            {
                                pinRayMap |= rayMask;
                                inDoubleCheck = inCheck; // if already in check, then this is double check
                                inCheck = true;
                            }
                            break;
                        }
                        else
                        {
                            // This enemy piece is not able to move in the current direction, and so is blocking any checks/pins
                            break;
                        }
                    }
                }
            }
            // Stop searching for pins if in double check, as the king is the only piece able to move in that case anyway
            if (inDoubleCheck)
            {
                break;
            }

        }
    }

    private void generatePawnAttackMap()
    {
        PieceTable opponentPawns = board.pawns[opponentColorIndex];
        currentPawnAttackMap = 0;
        bool isPawnCheck = false;

        for (int pawnIndex = 0; pawnIndex < opponentPawns.Count; pawnIndex++)
        {
            int pawnSquare = opponentPawns[pawnIndex];
            ulong pawnAttacks = pawnAttackBitboards[pawnSquare][opponentColorIndex];
            currentPawnAttackMap |= pawnAttacks;

            if (!isPawnCheck && BoardUtills.ContainsTile(pawnAttacks, kingSquare))
            {
                isPawnCheck = true;
                inDoubleCheck = inCheck; // if already in check, then this is double check
                inCheck = true;
                checkRayMap |= 1ul << pawnSquare;
            }
        }
    }

    private void generateKingAttackMap()
    {
        currentKingAttackMap = kingAttackBitboards[board.KingSquares[opponentColorIndex]];
    }
    #endregion

    #region PositionInfo
    public bool isSafePosition(int pos)
    {
        return !BoardUtills.ContainsTile(currentAttackMap, pos);
    }

    public bool IsPinned(int pos)
    {
        return pinsExist && ((pinRayMap >> pos) & 1) != 0;
    }

    bool IsCheckingPiece(int pos)
    {
        return inCheck && ((checkRayMap >> pos) & 1) != 0;
    }

    bool IsMovinginDirection(int dir, int startPos, int targetPos)
    {
        int moveDir = directionLookup[targetPos - startPos + 63];
        return (dir == moveDir || -dir == moveDir);
    }

    bool InCheckRay(int pos)
    {
        return inCheck && ((checkRayMap >> pos) & 1) != 0;
    }

    bool IsMovingAlongPinnedAxis(int pos, int offset)
    {
        return !IsPinned(pos) || IsMovinginDirection(offset, pos, kingSquare);
    }

    bool ResolvesCheckRay(int pos)
    {
        return !inCheck || InCheckRay(pos);
    }
    #endregion

    #endregion

    #region GeneratePieceMoves


    private void GenerateSlidingPieceMoves(int position, int startDir, int endDir)
    {
        bool isPinned = IsPinned(position);

        // If this piece is pinned, and the king is in check, this piece cannot move
        if (inCheck && isPinned)
        {
            Debug.Log("IN CHECK BUT PINNED:" + BoardUtills.stringFromIndex(position));
            return;
        }
        int opoColor = board.whiteTurn ? BLACK : WHITE;
        for (int direction = startDir; direction < endDir; direction++)
        {
            int dirOffset = directionOffsets[direction];
            if (isPinned && !IsMovinginDirection(dirOffset, kingSquare, position))
            {
                continue;
            }
            for (int n = 0; n < numSquaresToEdge[position][direction]; n++) //TODO Optimize save pice pos
            {
                int target = position + directionOffsets[direction] * (n + 1);
                int pieceOnTarget = board.tiles[target];

                if (IsColour(pieceOnTarget, color))
                    break;
                bool isCapture = pieceOnTarget != NONE;
                bool movePreventsCheck = InCheckRay(target);
                if (movePreventsCheck || !inCheck)
                {
                    if (genNonCaptures || isCapture)
                        Moves.Add(new Move(position, target));
                }
                if (isCapture || movePreventsCheck)
                {
                    break;
                }
            }
        }
    }

    private void GeneratePawnPieceMoves(int position)
    {
        //Debug.Log("PAWN: " + BoardUtills.stringFromIndex(position));
        int moveDir = (whiteTurn) ? directionOffsets[NORTH_I] : directionOffsets[SOUTH_I];
        int[] attackDir = (whiteTurn) ? pawnAttackDirections[0] : pawnAttackDirections[1];
        bool isInStarting = ((whiteTurn && (position / 8 == 1)) || ((!whiteTurn) && (position / 8 == 6)));
        int enPassantPos = board.currGameState.EnPassant();

        int target = position + moveDir;
        bool willUpgrade = (target / 8 == 7 || target / 8 == 0);
        //resolvesCheckByNonKingMove(board, pos, target);
        //pinnedPosDir
        bool isLegal = false;

        if (genNonCaptures)
        {
            if (target > 63 || target < 0)
            {
                Debug.Log(target + "," + position + (whiteTurn ? ", WHITE" : ", BLACK"));
                string str = "";
                for (int ii = 0; ii < board.pawns[colorIndex].Count; ii++)
                {
                    str += board.pawns[colorIndex][ii] + ", ";
                }
                Debug.Log("::" + str);
            }

            if ((board.tiles[target] == 0) && willUpgrade)
            {
                if (IsMovingAlongPinnedAxis(position, moveDir) && ResolvesCheckRay(target))
                    GeneratePawnUpgrades(position, target);
            }
            else if (board.tiles[target] == 0)
            {
                //if (IsPinned(position))
                //    Debug.Log("PINNED: " + position);
                //if (!IsMovingAlongPinnedAxis(position, moveDir))
                //    Debug.Log("NOT MOVING PINNED AXIS: " + position);
                //if (!ResolvesCheckRay(target))
                //    Debug.Log("NOT RESOLVING CHECK RAY: " + position);

                isLegal = IsMovingAlongPinnedAxis(position, moveDir) && ResolvesCheckRay(target);
                if (isLegal)
                {
                    Moves.Add(new Move(position, target));
                }
                target += moveDir;
                isLegal = IsMovingAlongPinnedAxis(position, moveDir) && ResolvesCheckRay(target);
                if (isInStarting && board.tiles[target] == 0)
                    if (isLegal)
                        Moves.Add(new Move(position, target, Move.Flag.PawnTwoForward));

            }
        }

        target = position + attackDir[0];
        //if (!IsColour(board.tiles[target], opponentColor))
        //    Debug.Log("NOATK LEFT: " + BoardUtills.stringFromIndex(position) + ", " + BoardUtills.stringFromIndex(target));
        //if (IsColour(board.tiles[target], opponentColor))
        //    Debug.Log("!!ATK LEFT: " + BoardUtills.stringFromIndex(position) + ", " + BoardUtills.stringFromIndex(target));
        if (IsOnBoard(target) && IsColour(board.tiles[target], opponentColor) && (position - (8 * (position / 8))) != 0)
        {
            isLegal = (IsMovingAlongPinnedAxis(position, attackDir[0]) && ResolvesCheckRay(target));//|| IsCheckingPiece(target);
            if (isLegal)
            {
                if (willUpgrade)
                    GeneratePawnUpgrades(position, target);
                else
                    Moves.Add(new Move(position, target));
            }
        }

        target = position + attackDir[1];
        if (IsOnBoard(target) && Colour(board.tiles[target]) == ((board.whiteTurn) ? BLACK : WHITE) && (position - 8 * (position / 8)) != 7)
        {
            isLegal = IsMovingAlongPinnedAxis(position, attackDir[1]) && ResolvesCheckRay(target);//|| IsCheckingPiece(target);
            if (isLegal)
            {
                if (willUpgrade)
                    GeneratePawnUpgrades(position, target);
                else
                    Moves.Add(new Move(position, target));
            }
        }
        if ((enPassantPos == position - 1 && numSquaresToEdge[position][WEST_I] != 0) || (board.enPassantAble == position + 1 && numSquaresToEdge[position][EAST_I] != 0))
        {
            int offset = enPassantPos - position;
            isLegal = IsMovingAlongPinnedAxis(position, offset) && ResolvesCheckRay(enPassantPos);
            if (isLegal)
            {
                //TODO FIX
                //!FUCK THIS COMMENT
                //*Fixed now but
                //*Was previously :(board.enPassantAble == position - 1  || board.enPassantAble == position + 1)
                //! *MASSIVE BLUNDER*

                //*Oh shit here we go again...
                //*Enpassant bypasses Pin kinda (Position 3 ->e2e4->g4e3->Illegal CheckMate)
                //*This was previously just one line below this.... Fucking chess

                //* FOURTH time is the charm right?
                //*Back at it again
                int col = board.whiteTurn ? WHITE : BLACK;
                int oCol = board.whiteTurn ? BLACK : WHITE;
                bool kingOnRank = false;
                bool rookOrQueenLeft = false;
                bool rookOrQueenRight = false;
                int nPiecesOnLeft = 0;
                int nPiecesOnRight = 0;

                for (int ii = 1; ii <= numSquaresToEdge[position][WEST_I]; ii++)
                {
                    if (board.tiles[position - ii] != 0)
                    {
                        int piece = board.tiles[position - ii];
                        if (IsType(piece, KING) && IsColour(piece, color))
                            kingOnRank = true;
                        else if (IsRookOrQueen(piece) && IsColour(piece, opponentColor))
                        { rookOrQueenLeft = true; break; }
                        else if (!rookOrQueenLeft)
                            nPiecesOnLeft++;

                    }
                }
                for (int ii = 1; ii <= numSquaresToEdge[position][EAST_I]; ii++)
                {
                    if (board.tiles[position + ii] != 0)
                    {
                        int piece = board.tiles[position + ii];
                        if (IsType(piece, KING) && IsColour(piece, color))
                            kingOnRank = true;
                        else if (IsRookOrQueen(piece) && IsColour(piece, opponentColor))
                        { rookOrQueenRight = true; break; }
                        else if (!rookOrQueenRight)
                            nPiecesOnRight++;

                    }
                }
                //Debug.Log(kingOnRank
                //+ ", " + rookOrQueenLeft
                //+ ", " + rookOrQueenRight
                //+ ", " + nPiecesOnLeft
                //+ ", " + nPiecesOnRight);
                if (kingOnRank && ((rookOrQueenLeft && (nPiecesOnLeft <= 1) || (rookOrQueenRight && (nPiecesOnRight <= 1)))))
                    return;
                else
                {
                    //Debug.Log("EP:" + board.enPassantAble);
                    Moves.Add(new Move(position, board.enPassantAble + moveDir, Move.Flag.EnPassantCapture));
                }

            }
        }
    }

    private void GeneratePawnUpgrades(int position, int target)
    {
        //Debug.Log("PAWN UPGRADE");
        Moves.Add(new Move(position, target, Move.Flag.PromoteToQueen));
        Moves.Add(new Move(position, target, Move.Flag.PromoteToKnight));
        Moves.Add(new Move(position, target, Move.Flag.PromoteToRook));
        Moves.Add(new Move(position, target, Move.Flag.PromoteToBishop));
    }

    private void GenerateCastelingMoves()
    {
        bool[] castleRights = whiteTurn ?
            new bool[] { board.whiteCastleKingside, board.whiteCastleQueenside } :
            new bool[] { board.blackCastleKingside, board.blackCastleQueenside };
        int[][] squaresBetween = whiteTurn ?
            new int[][] { new int[] { 5, 6 }, new int[] { 1, 2, 3 } } :
            new int[][] { new int[] { 61, 62 }, new int[] { 57, 58, 59 } };
        int[] rookPos = whiteTurn ?
            new int[] { 7, 0 } :
            new int[] { 63, 56 };

        bool cas1 =
        castleRights[0] &&
        board.tiles[squaresBetween[0][0]] == 0 &&
        board.tiles[squaresBetween[0][1]] == 0 &&
        (isSafePosition(squaresBetween[0][0])) &&
        (isSafePosition(squaresBetween[0][1])) &&
        (IsType(board.tiles[rookPos[0]], ROOK)) &&
        (IsColour(board.tiles[rookPos[0]], color)) &&
        (!(whiteTurn ? board.WhiteInCheck : board.BlackInCheck));
        //* NOTE "The king does not pass through a square that is attacked by an enemy piece."
        //* since the he does not pass through the third square it is ok
        bool cas2 =
        castleRights[1] &&
        board.tiles[squaresBetween[1][0]] == 0 &&
        board.tiles[squaresBetween[1][1]] == 0 &&
        board.tiles[squaresBetween[1][2]] == 0 &&
        (isSafePosition(squaresBetween[1][1])) &&
        (isSafePosition(squaresBetween[1][2])) &&
        (IsType(board.tiles[rookPos[1]], ROOK)) &&
        (IsColour(board.tiles[rookPos[1]], color)) &&
        (!(whiteTurn ? board.WhiteInCheck : board.BlackInCheck));
        //*DEBUG
        //Debug.Log(
        //castleRights[1] + ",\n" +
        //(board.tiles[squaresBetween[1][0]] == 0) + ",\n" +
        //(board.tiles[squaresBetween[1][1]] == 0) + ",\n" +
        //(board.tiles[squaresBetween[1][2]] == 0) + ",\n" +
        //((isSafePosition(squaresBetween[1][1]))) + ",\n" +
        //((isSafePosition(squaresBetween[1][2]))) + ",\n" +
        //((IsType(board.tiles[rookPos[1]], ROOK))) + ",\n" +
        //((IsColour(board.tiles[rookPos[1]], color))) + ",\n" +
        //(!(whiteTurn ? board.WhiteInCheck : board.BlackInCheck)));

        if (cas1)
        {
            Moves.Add(new Move(kingSquare, squaresBetween[0][1], Move.Flag.Castling));
            //Debug.Log("CAS K");
        }
        if (cas2)
        {
            Moves.Add(new Move(kingSquare, squaresBetween[1][1], Move.Flag.Castling));
            //Debug.Log("CAS Q");
        }
    }

    #endregion

    #region Old

    public List<Move>[] sortMovesBasedOnPosition()
    {
        List<Move>[] moveGrid = new List<Move>[64];
        foreach (Move move in Moves)
        {
            if (moveGrid[move.StartSquare] == null)
                moveGrid[move.StartSquare] = new List<Move>();
            moveGrid[move.StartSquare].Add(move);
        }
        return moveGrid;
    }

    #region DEPRECATED


    //TODO OPTIMIZE 10%,
    //*


    //TODO OPTIMIZE 5.4%, SELF = 0.7%
    //*
    //public static void generateKingMoves(int position, Board board, List<Move> moveList, MoveUtills generator)
    //{
    //    int[] arr = getKingMoves(position);
    //    for (int ii = 0; ii < arr.Length; ii++)
    //    {
    //        int target = arr[ii];
    //        if (!IsColour(board.tiles[target], board.ColorTurn))
    //            //if (MoveLegalityUtills.IsLegal(position, target, board)) //TODO OPTIMIZE 2.1%
    //            moveList.Add(new Move(position, target));
    //    }
    //    generateCastelingMoves(position, board, moveList, generator);
    //}

    //TODO OPTIMIZE 1.6% (Is kinda optimzed)
    //*
    // public static void generateCastelingMoves(int position, Board board, List<Move> moveList, MoveUtills generator)
    // {
    //     bool isWhite = Colour(board.tiles[position]) == WHITE;
    //     int color = isWhite ? WHITE : BLACK;
    //     bool[] castleRights = isWhite ?
    //         new bool[] { board.whiteCastleKingside, board.whiteCastleQueenside } :
    //         new bool[] { board.blackCastleKingside, board.blackCastleQueenside };
    //     int[][] squaresBetween = isWhite ?
    //         new int[][] { new int[] { 5, 6 }, new int[] { 1, 2, 3 } } :
    //         new int[][] { new int[] { 61, 62 }, new int[] { 57, 58, 59 } };
    //     int[] rookPos = isWhite ?
    //         new int[] { 7, 0 } :
    //         new int[] { 63, 56 };

    //     bool cas1 =
    //     castleRights[0] &&
    //     board.tiles[squaresBetween[0][0]] == 0 &&
    //     board.tiles[squaresBetween[0][1]] == 0 &&
    //     (generator.isSafePosition(squaresBetween[0][0])) &&
    //     (generator.isSafePosition(squaresBetween[0][1])) &&
    //     (IsType(board.tiles[rookPos[0]], ROOK)) &&
    //     (IsColour(board.tiles[rookPos[0]], color)) &&
    //     (!(isWhite ? board.WhiteInCheck : board.BlackInCheck));
    //     //* NOTE "The king does not pass through a square that is attacked by an enemy piece."
    //     //* since the he does not pass through the third square it is ok
    //     bool cas2 =
    //     castleRights[1] &&
    //     board.tiles[squaresBetween[1][0]] == 0 &&
    //     board.tiles[squaresBetween[1][1]] == 0 &&
    //     board.tiles[squaresBetween[1][2]] == 0 &&
    //     (generator.isSafePosition(squaresBetween[1][1])) &&
    //     (generator.isSafePosition(squaresBetween[1][2])) &&
    //     (IsType(board.tiles[rookPos[1]], ROOK)) &&
    //     (IsColour(board.tiles[rookPos[1]], color)) &&
    //     (!(isWhite ? board.WhiteInCheck : board.BlackInCheck));

    //     if (cas1)
    //     {
    //         moveList.Add(new Move(position, squaresBetween[0][1], Move.Flag.Castling));
    //     }
    //     if (cas2)
    //     {
    //         moveList.Add(new Move(position, squaresBetween[1][1], Move.Flag.Castling));
    //     }

    // }

    // TODO OPTIMIZE 17.6%, SELF = 1.8%
    //*
    // TODO OPTIMIZE 18.8%, SELF = 1.6%

    // private static List<Move> generatePawnUpgrade(int position, int target)
    // {
    //     List<Move> upgradeMoves = new List<Move>();
    //     upgradeMoves.Add(new Move(position, target, Move.Flag.PromoteToQueen));
    //     upgradeMoves.Add(new Move(position, target, Move.Flag.PromoteToRook));
    //     upgradeMoves.Add(new Move(position, target, Move.Flag.PromoteToBishop));
    //     upgradeMoves.Add(new Move(position, target, Move.Flag.PromoteToKnight));
    //     return upgradeMoves;
    // }

    //TODO Make this //!DEPRECATED 
    // public static List<Move> generateMovesForThisSquare(int pos, Board board)
    // {
    //     List<Move> moveL = new List<Move>();

    //     int piece = board.tiles[pos];
    //     if (IsColour(piece, board.ColorTurn))
    //     {
    //         if (IsSlidingPiece(piece))
    //         {
    //             generateSlidingMoves(pos, piece, board, moveL);
    //         }
    //         else
    //         {
    //             switch (PieceType(piece))
    //             {
    //                 case PAWN:
    //                     generatePawnMoves(pos, board, moveL);
    //                     break;
    //                 case KNIGHT:
    //                     generateKnightMoves(pos, board, moveL);
    //                     break;
    //                 case KING:
    //                     generateKingMoves(pos, board, moveL);
    //                     break;

    //                 default:
    //                     moveL = new List<Move>();
    //                     break;
    //             }
    //         }
    //     }
    //     else
    //     {
    //         moveL = new List<Move>();
    //     }
    //     return moveL;
    // }



    //TODO OPTIMIZE 0.4%, SELF = 0.2%
    //public static List<int> getPieces(Board board, int color)
    //{
    //    return (color == WHITE) ? board.WhitePieces : board.BlackPieces;
    //    //TODO Optimize
    //    //List<int> pieces = new List<int>();
    //    //for (int pos = 0; pos < 64; pos++)
    //    //    if (IsColour(board.tiles[pos], color)) //TODO OPTIMIZE 0.1%
    //    //        pieces.Add(pos);                    ////TODO OPTIMIZE 0.1%
    //    //return pieces;
    //}

    //public static List<int> getSlidingPieces(Board board, int color)
    //{
    //    //TODO Optimize
    //    List<int> allpieces = (color == WHITE) ? board.WhitePieces : board.BlackPieces;
    //    return allpieces.Where(n => IsSlidingPiece(board.tiles[n])).ToList<int>();
    //    //for (int pos = 0; pos < 64; pos++)
    //    //    if (IsColour(board.tiles[pos], color) && IsSlidingPiece(board.tiles[pos]))
    //    //        pieces.Add(pos);
    //    //return pieces;
    //}
    #endregion

    #endregion
}
