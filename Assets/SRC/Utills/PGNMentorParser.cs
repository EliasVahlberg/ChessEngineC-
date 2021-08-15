using System.Collections;
using System.IO;
using UnityEngine;
/*
@File TranspositionTable.cs
@author Elias Vahlberg & Sebastian Lague 
Original: https://github.com/SebLague/Chess-AI/blob/main/Assets/Scripts/Other/Book/MultiPGNParser.cs

*/
namespace Utills
{


    public class PGNMentorParser : MonoBehaviour
    {
        //public TextAsset[] inputFiles;
        public static PGNMentorParser instance;
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
        public string[] PGNArray;
        public string combinedPGN;
        public TextAsset outputFile;
        public string externalOutputPath;
        public bool append;

        //[ContextMenu("Parse All")]
        void ParseAll()
        {
            string allGames = "";
            if (PGNArray.Length == 0)
                return;
            if (outputFile == null)
                return;
            for (int ii = 0; ii < PGNArray.Length; ii++)
            {
                allGames += Parse(PGNArray[ii]);
            }

            FileWriter.WriteToTextAsset_EditorOnly(outputFile, allGames, append);
        }
        [ContextMenu("Save PGN")]
        void ParseAllRuntime()
        {
            string allGames = "";
            if (PGNArray.Length == 0)
                return;

            for (int ii = 0; ii < PGNArray.Length; ii++)
            {
                allGames += Parse(PGNArray[ii]);
                Debug.Log(ii);
            }
            SavePGNToFile(allGames);


        }
        [ContextMenu("Get Games Files")]
        void GetGamesFiles()
        {
            FileUtills.instance.GetFilesFromFileExplorer("Text files (*.pgn) | *.pgn", str => GetGamesFilesCallback(str));
        }

        void SavePGNToFile(string content)
        {
            Debug.Log("ENTER");
            FileUtills.instance.SaveToFile("Text files (*.pgn) | *.pgn", content, path => StartCoroutine(SavePGNToFileAssyncCallback(path)));
        }
        IEnumerator SavePGNToFileAssyncCallback(string path)
        {
            externalOutputPath = path;
            Debug.Log("DONE");
            yield return null;
        }
        void GetGamesFilesCallback(string[] fileContents)
        {
            PGNArray = fileContents;
        }

        string Parse(string text)
        {

            bool isReadingPGN = false;
            string currentPgn = "";
            string parsedGames = "";

            StringReader reader = new StringReader(text);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains("["))
                {
                    if (isReadingPGN)
                    {
                        isReadingPGN = false;
                        parsedGames += currentPgn.Replace("  ", " ").Trim() + '\n';
                        currentPgn = "";
                    }
                    continue;
                }
                else
                {
                    isReadingPGN = true;
                    string[] moves = line.Split(' ');
                    foreach (string move in moves)
                    {
                        string formattedMove = move;
                        if (formattedMove.Contains("."))
                        {
                            formattedMove = formattedMove.Split('.')[1];
                        }
                        currentPgn += formattedMove.Trim() + " ";
                    }
                }
            }

            return parsedGames;

        }
    }

}