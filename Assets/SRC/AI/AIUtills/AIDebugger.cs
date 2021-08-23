using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ChessAI
{

    public class AIDebugger : MonoBehaviour
    {
        [SerializeField]
        private int numReCalculations = 10;
        [SerializeField]
        private AISettings settings;
        [SerializeField]
        private string[] testingFenPossitions;
        private TestingAIGen4V1 player;
        [SerializeField]
        private string outputPath;
        [SerializeField]
        private string DebugNoteTime = "100ms";
        [SerializeField]
        private string DebugNoteDepth = "...";
        [SerializeField]
        private string DebugNoteEvalFunc = "CapV6, Map1";
        [SerializeField]
        private string DebugNoteMoveOrder = "Standard";
        [SerializeField]
        private string DebugNoteOther = "...";
        public static AIDebugger instance;
        private List<string> debugList;
        private List<int[]> valList;
        private static readonly string format = "{0,-20} {1,-20} {2,-20} {3,-20} {4,-20} {5,-20} {6,-20}";

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

        [ContextMenu("Save DebugInfo")]
        public void WriteDebugData()
        {
            if (debugList == null)
            {
                Debug.LogError("NULL LIST");
                return;
            }
            StreamWriter sw = null;
            try
            {
                debugList.Add(getMean() + "\n");
                sw = new StreamWriter(outputPath, false);
                foreach (string str in debugList)
                {
                    sw.Write(str);
                }

                sw.Close();

            }
            catch (System.Exception _ex)
            {
                Debug.LogError("FAILED TO WRITE");
                if (sw != null)
                    sw.Close();
                throw _ex;
            }
        }

        public void AddDebugInfo(Search search)
        {
            if (debugList == null)
            {
                debugList = new List<string>();
                valList = new List<int[]>();
                debugList.Add(DebugNoteTime + " | " + DebugNoteDepth + " | " + DebugNoteEvalFunc + " | " + DebugNoteMoveOrder + " | " + DebugNoteOther + "\n" + "\n");
                debugList.Add(string.Format(format, "Move", "Num Evaluations", "Completed deapth", "Num eval positions", "Num Prunes", "Num Quiet Evals", "Num Transpossitions") + "\n");
            }
            string str = string.Format(format, search.sD.move,
            search.sD.eval,
            search.sD.lastCompletedDepth,
            search.sD.numPositionsEvaluated,
            search.sD.numPrunes,
            search.sD.numQPositionsEvaluated,
            search.sD.numTPositions);

            valList.Add(new int[6]{search.sD.eval,
            search.sD.lastCompletedDepth,
            search.sD.numPositionsEvaluated,
            search.sD.numPrunes,
            search.sD.numQPositionsEvaluated,
            search.sD.numTPositions});

            //string str = search.sD.move + "," +
            //search.sD.eval + "," +
            //search.sD.lastCompletedDepth + "," +
            //search.sD.numPositionsEvaluated + "," +
            //search.sD.numPrunes + "," +
            //search.sD.numQPositionsEvaluated + "," +
            //search.sD.numTPositions + "\n";
            debugList.Add(str + "\n");
        }
        public void AddDebugInfo(Move move)
        {
            string str = string.Format(format, move.ToString(),
            "BOOK",
            "BOOK",
            "BOOK",
            "BOOK",
            "BOOK",
            "BOOK");
        }
        public string getMean()
        {
            float[] valsMean = new float[6];
            foreach (int[] arr in valList)
            {
                valsMean[0] += arr[0];
                valsMean[1] += arr[1];
                valsMean[2] += arr[2];
                valsMean[3] += arr[3];
                valsMean[4] += arr[4];
                valsMean[5] += arr[5];
            }
            valsMean[0] /= valList.Count;
            valsMean[1] /= valList.Count;
            valsMean[2] /= valList.Count;
            valsMean[3] /= valList.Count;
            valsMean[4] /= valList.Count;
            valsMean[5] /= valList.Count;

            return string.Format(format, "MEAN : ",
        valsMean[0],
        valsMean[1],
        valsMean[2],
        valsMean[3],
        valsMean[4],
        valsMean[5]);
        }
        [ContextMenu("Run Basic test")]
        public void basicTest()
        {
            debugList = null;
            valList = null;
            player = new TestingAIGen4V1();
            Debug.Log("Test Started");
            int id = Utills.TimeUtills.Instance.startMeasurement();
            if (!settings.useIterativeDeepening || settings.useFixedDepthSearch)
            {

                for (int ii = 0; ii < testingFenPossitions.Length; ii++)
                {
                    int id2 = Utills.TimeUtills.Instance.startMeasurement();
                    for (int jj = 0; jj < numReCalculations; jj++)
                    {

                        Board board = new Board(testingFenPossitions[ii]);
                        board.generateNewMoves();
                        player.Initialize(board, settings);
                        player.RequestMove();

                    }

                    AddDebugInfo(player.search);
                    long millis2 = Utills.TimeUtills.Instance.stopMeasurementMillis(id2);
                    debugList.Add("Time: " + millis2 + "ms + \n");

                }
            }
            Debug.Log("Test Ended");
            long millis = Utills.TimeUtills.Instance.stopMeasurementMillis(id);
            Debug.Log("Time:" + millis + "ms");
            debugList.Add("Total Time: " + millis + "ms + \n");
            WriteDebugData();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                //basicTest();
            }
        }
    }
}