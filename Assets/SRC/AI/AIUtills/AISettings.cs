using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace ChessAI
{

    [CreateAssetMenu(menuName = "AI/Settings")]
    public class AISettings : ScriptableObject
    {

        public event System.Action requestAbortSearch;

        public int depth;
        public bool useIterativeDeepening;
        public bool useTranspositionTable;

        //public bool useThreading;
        public bool useFixedDepthSearch;
        public int searchTimeMillis = 1000;
        //public bool endlessSearchMode;
        public bool clearTTEachMove;

        public bool useQuiescenceSearch = true;

        public bool useBook;
        public bool useExternalBook;
        public TextAsset book;
        public string externalBookPath;
        public int maxBookPly = 10;

        public bool useWeightMap = true;
        public int weightMapWeight = 1;

        public int captureWeight = 100;

        //public MoveGenerator.PromotionMode promotionsToSearch;

        //public Search.SearchDiagnostics diagnostics;
        public OppeningsBook GetOppeningsBook()
        {
            return useExternalBook ? BookBuilder.LoadExternalBookRuntime(externalBookPath) : BookBuilder.LoadOppeningsBookFromFile(book);
        }

        public void RequestAbortSearch()
        {
            requestAbortSearch?.Invoke();
        }
    }
}