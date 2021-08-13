using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Utills;

namespace ChessAI
{
    /*
    @File SearchAIGen2V4.cs
    @author Elias Vahlberg 
    @Date 2021-08 
    */
    [CreateAssetMenu(fileName = "SearchAIGen3V1", menuName = "Utilities/AI GEN 3/Search AI V1")]
    public class SearchAIGen3V1 : IAIObject
    {
        private Board privateBoard;
        private Move bestMove;
        private int bestVal;

        public AISettings settings;

        private Search search;
        private static int repetitionMaxLength = 5;

        CancellationTokenSource cancelSearchTimer;
        [System.NonSerialized]
        private bool moveFound = false;
        [System.NonSerialized]
        private bool started = false;
        private int[] tilesCopy;
        private GameState gameStateCopy;
        private string logInfo;

        public override Move SelectMove(Board board)
        {
            if (privateBoard == null)
                privateBoard = board;
            if (started)
            {
                if (moveFound)
                {
                    if (!AIUtillsManager.instance.BoardIntegrityCheck(privateBoard, tilesCopy, gameStateCopy))
                        throw new InvalidOperationException("Board was mutated during selection of moves");
                    started = false;
                    moveFound = false;
                    if (logInfo != null)
                        ConsoleHistory.instance.addLogHistory(logInfo);
                    return bestMove;
                }
                else
                    return PENDING_SEARCH_MOVE;
            }
            if (search == null)
                search = new Search(privateBoard, settings);//search = new Search(new Board(board.boardToFEN()), settings);

            //search.useCoinToss = hasRepetitions(privateBoard);


            if (privateBoard.Moves.Count == 0)
                throw new ArgumentNullException("board.Moves", "EndgameConditions are handeled by the game manager and not the AI");

            #region Before_ValidCopy

            tilesCopy = new int[64];
            Array.Copy(board.tiles, tilesCopy, 64);
            gameStateCopy = new GameState(privateBoard.currGameState.gameStateValue, privateBoard.currGameState.PrevMove);
            #endregion

            started = true;

            Task.Factory.StartNew(() => search.SearchRecurr(), TaskCreationOptions.LongRunning);
            cancelSearchTimer = new CancellationTokenSource();
            Task.Delay(settings.searchTimeMillis, cancelSearchTimer.Token).ContinueWith((t) => TimedStoppThreadedSearch());
            //Debug.Log(this.Name + " : " + maxVal);
            return PENDING_SEARCH_MOVE;
        }

        private bool hasRepetitions(Board board)
        {
            ulong[] hashHistory = board.HashHistory.ToArray();
            for (int ii = 0; ii < Math.Min(hashHistory.Length, repetitionMaxLength); ii++)
            {
                for (int jj = ii + 1; jj < Math.Min(hashHistory.Length, repetitionMaxLength); jj++)
                {
                    if (hashHistory[ii] == hashHistory[jj])
                        return true;
                }

            }
            return false;
        }

        void StartSearchNonThreaded()
        {
            search.SearchRecurr();
            moveFound = true;
        }

        void TimedStoppThreadedSearch()
        {
            if (cancelSearchTimer == null || !cancelSearchTimer.IsCancellationRequested)
            {
                search.StoppSearch();
                (bestMove, bestVal) = search.GetResult();
                moveFound = true;

                logInfo = "<color=yellow>" + search.LogDebugInfo() + "</color>";
                GameManager.instance.AIPendingComplete = true;
            }
            else
            {
                throw new InvalidOperationException("Someting went wrong when searching for move");
            }

        }

        void OnSearchComplete(Move move)
        {
            // Cancel search timer in case search finished before timer ran out (can happen when a mate is found)
            cancelSearchTimer?.Cancel();
            this.bestMove = move;
            moveFound = true;
            GameManager.instance.AIPendingComplete = true;
            Debug.LogError("SearchAbort early");
        }

    }

}