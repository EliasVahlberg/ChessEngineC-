using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utills;

namespace ChessAI
{
    public class BookBuilder : MonoBehaviour
    {
        public int maxPlyToRecord;

        public int minMovePlayCount = 10;

        public TextAsset gamesFile;
        public TextAsset bookFile;
        public bool append = false;
        [ContextMenu("Build Book")]
        void BuildBook()
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            OppeningsBook book = new OppeningsBook();
            StringReader reader = new StringReader(gamesFile.text);
            string pgn;
            Board board;

            while (!string.IsNullOrEmpty(pgn = reader.ReadLine()))
            {
                Move[] moves = PGNUtills.MovesFromPGN(pgn, maxPlyCount: maxPlyToRecord);
                board = Board.DefaultBoard();
                //*Adds the Zobrist keys to the dictionary
                //*Also Makes sure that there are no illegal moves in there
                for (int i = 0; i < moves.Length; i++)
                {
                    book.Add(board.ZobristKey, moves[i]);
                    board.useMove(moves[i]);
                }
            }

            string bookString = "";
            foreach (KeyValuePair<ulong, PositionPage> pagePair in book.pages)
            {
                ulong key = pagePair.Key;
                PositionPage page = pagePair.Value;
                string line = key + ":";
                bool isFirstMoveEntry = true;
                foreach (var moveCountByMove in page.timesMovePlayed)
                {
                    ushort moveValue = moveCountByMove.Key;
                    int moveCount = moveCountByMove.Value;
                    if (moveCount >= minMovePlayCount)
                    {
                        if (isFirstMoveEntry)
                        {
                            isFirstMoveEntry = false;
                        }
                        else
                        {
                            line += ",";
                        }
                        line += $" {moveValue} ({moveCount})";
                    }
                }

                bool hasRecordedAnyMoves = !isFirstMoveEntry;
                if (hasRecordedAnyMoves)
                {
                    bookString += line + System.Environment.NewLine;
                }
            }
            Debug.Log("PAGES LOADED: " + book.pages.Count);
            FileWriter.WriteToTextAsset_EditorOnly(bookFile, bookString, append);
            Debug.Log("Created book: " + sw.ElapsedMilliseconds + " ms.");
        }


        public static OppeningsBook LoadOppeningsBookFromFile(TextAsset bookFile)
        {
            OppeningsBook book = new OppeningsBook();
            var reader = new StringReader(bookFile.text);

            string line;
            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                ulong positionKey = ulong.Parse(line.Split(':')[0]);
                string[] moveInfoStrings = line.Split(':')[1].Trim().Split(',');

                for (int i = 0; i < moveInfoStrings.Length; i++)
                {
                    string moveInfoString = moveInfoStrings[i].Trim();
                    if (!string.IsNullOrEmpty(moveInfoString))
                    {

                        ushort moveValue = ushort.Parse(moveInfoString.Split(' ')[0]);
                        string numTimesPlayedString = moveInfoString.Split(' ')[1].Replace("(", "").Replace(")", "");
                        int numTimesPlayed = int.Parse(numTimesPlayedString);
                        book.Add(positionKey, new Move(moveValue), numTimesPlayed);

                    }
                }
            }

            return book;
        }
    }

}