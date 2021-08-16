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
        public string externalBookPath = null;
        public AISettings targetSettings;


        public static BookBuilder instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("SAMEINSTACE ");
                Destroy(this);
            }
        }
        [ContextMenu("Build Book")]
        void BuildBook()
        {
            string bookString = BuildBookRuntime(gamesFile.text, maxPlyToRecord, minMovePlayCount);
            FileWriter.WriteToTextAsset_EditorOnly(bookFile, bookString, append);

        }

        #region Build_RUNTIME
#if UNITY_STANDALONE_WIN
        
        [ContextMenu("Build Book From External PGN")]
        public void PreBuildAndSaveBookRuntime()
        {
            FileUtills.instance.GetFilesFromFileExplorer("Text files (*.pgn) | *.pgn", str => BuildAndSaveBookRuntime(str));
        }

        public void BuildAndSaveBookRuntime(string[] PGNGames)
        {

            string bookString = BuildBookRuntime(PGNGames[0], maxPlyToRecord, minMovePlayCount);
            FileUtills.instance.SaveToFile("Text files (*.book) | *.book", bookString, str => BuildAndSaveBookRuntimeCallback(str));

        }

        public void BuildAndSaveBookRuntimeCallback(string path)
        {
            externalBookPath = path;
        }
#endif

        public static string BuildBookRuntime(string gamesString, int maxPlyToRecord, int minMovePlayCount)
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            OppeningsBook book = new OppeningsBook();
            StringReader reader = new StringReader(gamesString);
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
            Debug.Log("Created book: " + sw.ElapsedMilliseconds + " ms.");
            return bookString;
        }
        #endregion

        #region LoadRuntime

        //!DEPRECATED
        [ContextMenu("Load External Book PGN")]
#if UNITY_STANDALONE_WIN
        public void GetExternalBookPath()
        {
            FileUtills.instance.GetFilesFromFileExplorer("Text files (*.book) | *.book", str => LoadExternalBookRuntimeCallback(str));
        }

        //!DEPRECATED
        public void LoadExternalBookRuntimeCallback(string[] data) //TODO make it do something
        {
            OppeningsBook externalbook = LoadExternalOppeningsBook(data[0]);
            if (externalbook != null)
                Debug.Log("SUCSESS");
            if (targetSettings != null)
            {

            }
        }

        public static OppeningsBook LoadExternalBookRuntime(string path)
        {
            StreamReader streamReader = null;
            try
            {
                streamReader = new StreamReader(path);
                return LoadExternalOppeningsBook(streamReader.ReadToEnd());
            }
            catch (System.Exception)
            {
                if (streamReader != null)
                    streamReader.Close();
                throw;
            }
        }
#endif

        public static OppeningsBook LoadExternalOppeningsBook(string bookString)
        {
            OppeningsBook book = new OppeningsBook();
            var reader = new StringReader(bookString);
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

        #endregion
        public static OppeningsBook LoadOppeningsBookFromFile(TextAsset bookFile)
        {
            return LoadExternalOppeningsBook(bookFile.text);
        }

    }

}