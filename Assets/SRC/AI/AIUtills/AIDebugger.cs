using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ChessAI
{

    public class AIDebugger : MonoBehaviour
    {
        [SerializeField]
        private string outputPath;
        public static AIDebugger instance;
        private List<string> debugList;
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
                debugList = new List<string>();
            string str = search.sD.move + "," +
            search.sD.eval + "," +
            search.sD.lastCompletedDepth + "," +
            search.sD.numPositionsEvaluated + "," +
            search.sD.numPrunes + "," +
            search.sD.numQPositionsEvaluated + "," +
            search.sD.numTPositions + "\n";
            debugList.Add(str);
        }
    }
}