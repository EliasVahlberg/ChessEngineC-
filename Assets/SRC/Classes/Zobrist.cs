using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace SimpleChess
{
    /*
    @File Zobrist.cs
    @author Elias Vahlberg 
    @Date 2021-07
    @Credit Sebastian Lague 
    */
    public static class Zobrist
    {
        const int seed = unchecked((int)0xFEDCCDEF);
        const string randomNumbersFileName = "HashRndNums.txt";

        //* {Type, Color, Index}
        public static readonly ulong[,,] piecesArr = new ulong[8, 2, 64];
        public static readonly ulong[] castlingR = new ulong[16];

        public static readonly ulong[] ePFile = new ulong[9]; // no need for rank info as side to move is included in key
        public static readonly ulong sideToMove;

        static System.Random prng = new System.Random(seed);

        /*
        !Unoptimized should use StringBuilder
        */
        static void WriteRandomNumbers()
        {
            prng = new System.Random(seed);
            string randNumStr = "";
            int numOfNums = 64 * 8 * 2 + castlingR.Length + 9 + 1;

            for (int i = 0; i < numOfNums; i++)
            {
                randNumStr += RandUL();
                if (i != numOfNums - 1)
                {
                    randNumStr += ',';
                }
            }

            StreamWriter writer = new StreamWriter(randomNumbersPath);
            writer.Write(randNumStr);
            writer.Close();
        }

        static Queue<ulong> ReadRandomNumbers()
        {
            if (!File.Exists(randomNumbersPath))
            {
                Debug.Log("Create");
                WriteRandomNumbers();
            }
            Queue<ulong> randomNumbers = new Queue<ulong>();
            StreamReader reader = new StreamReader(randomNumbersPath);
            string numbersString = reader.ReadToEnd();
            reader.Close();
            if (numbersString == null)
            {
                //! Is kindoff an StackOF exception waiting to happen but I ain't scared
                Debug.Log("Create");
                WriteRandomNumbers();
                return ReadRandomNumbers();
            }

            string[] numberStrings = numbersString.Split(',');
            for (int i = 0; i < numberStrings.Length; i++)
            {
                ulong number = ulong.Parse(numberStrings[i]);
                randomNumbers.Enqueue(number);
            }
            return randomNumbers;
        }

        static Zobrist()
        {
            Queue<ulong> randomNumbers = ReadRandomNumbers();
            for (int sI = 0; sI < 64; sI++)
                for (int pI = 0; pI < 8; pI++)
                {
                    piecesArr[pI, 0, sI] = randomNumbers.Dequeue();
                    piecesArr[pI, 1, sI] = randomNumbers.Dequeue();
                }
            for (int i = 0; i < 16; i++)
            {
                castlingR[i] = randomNumbers.Dequeue();
            }

            for (int i = 0; i < ePFile.Length; i++)
            {
                ePFile[i] = randomNumbers.Dequeue();
            }

            sideToMove = randomNumbers.Dequeue();
        }
        /*
        *@OnBoardInit
        !Not to be used to update the hash
        */
        //Calculate zobrist key from current board position. This should only be used after setting board from fen; during search the key should be updated incrementally.
        public static ulong CalculateZobristKey(Board board)
        {
            ulong hash = 0;

            for (int squareIndex = 0; squareIndex < 64; squareIndex++)
            {
                if (board.tiles[squareIndex] != 0)
                {
                    int pieceType = Piece.PieceType(board.tiles[squareIndex]);
                    int pieceColour = Piece.Colour(board.tiles[squareIndex]);

                    hash ^= piecesArr[pieceType, (pieceColour == Piece.WHITE) ? Board.WhiteIndex : Board.BlackIndex, squareIndex];
                }
            }

            int epIndex = board.currGameState.EnPassant() % 8;
            if (board.currGameState.EnPassant() != 0)
            {
                hash ^= ePFile[epIndex];
            }

            if (!board.whiteTurn)
            {
                hash ^= sideToMove;
            }

            hash ^= castlingR[board.currGameState.CastleRights()];

            return hash;
        }

        static string randomNumbersPath
        {
            get
            {
                //*Makes sure that it doesn't land just anywhere
                return Path.Combine(Application.streamingAssetsPath, randomNumbersFileName);
            }
        }

        static ulong RandUL()
        {
            byte[] buffer = new byte[8];
            prng.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}