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
        private Move bestMove;
        private int bestVal;

        public AISettings settings;

        private Search search;

        CancellationTokenSource cancelSearchTimer;

        private bool moveFound = false;
        private bool started = false;
        private int[] tilesCopy;
        private GameState gameStateCopy;
        private void Awake()
        {

            moveFound = false;
            started = false;
        }
        public override Move SelectMove(Board board)
        {
            //if (started && moveFound)
            //    Debug.Log("START&MOVE");
            //if (!(started && moveFound))
            //    Debug.Log("NOSTART&MOVE");
            if (started)
            {
                if (moveFound)
                {
                    //if (!AIUtillsManager.instance.BoardIntegrityCheck(GameManager.instance.board, tilesCopy, gameStateCopy))
                    //    throw new InvalidOperationException("Board was mutated during selection of moves");
                    started = false;
                    moveFound = false;
                    return bestMove;
                }
                else
                    return PENDING_SEARCH_MOVE;
            }
            //if (search == null)

            search = new Search(new Board(board.boardToFEN()), settings);


            if (board.Moves.Count == 0)
                throw new ArgumentNullException("board.Moves", "EndgameConditions are handeled by the game manager and not the AI");

            #region Before_ValidCopy

            tilesCopy = new int[64];
            Array.Copy(board.tiles, tilesCopy, 64);
            gameStateCopy = new GameState(board.currGameState.gameStateValue, board.currGameState.PrevMove);
            #endregion

            started = true;

            Task.Factory.StartNew(() => search.SearchRecurr(), TaskCreationOptions.LongRunning);
            cancelSearchTimer = new CancellationTokenSource();
            Task.Delay(settings.searchTimeMillis, cancelSearchTimer.Token).ContinueWith((t) => TimedStoppThreadedSearch());
            //Debug.Log(this.Name + " : " + maxVal);
            return PENDING_SEARCH_MOVE;
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
            }

        }
        void OnSearchComplete(Move move)
        {
            // Cancel search timer in case search finished before timer ran out (can happen when a mate is found)
            cancelSearchTimer?.Cancel();
            this.bestMove = move;
            moveFound = true;
            Debug.LogError("SearchAbort early");
        }

    }

}