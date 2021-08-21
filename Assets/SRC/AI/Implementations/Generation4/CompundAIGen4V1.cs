
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChessAI
{
    public class CompundAIGen4V1 : IAIPlayer
    {
        private static readonly string AI_NAME = "Ivan";
        #region State

        private bool isSearching = false;
        private bool isInitialized = false;

        private Move bestMove;
        private int bestVal;
        private string searchLogText;
        #endregion

        #region Private

        private Board board;
        private AISettings settings;
        private OppeningsBook oppeningsBook;
        private Search search;
        private CancellationTokenSource cancelSearchTimer;
        #endregion

        #region Public
        public AISettings Settings { get => settings; }
        public Move BestMove { get => bestMove; }
        public bool IsInitialized { get => isInitialized; }
        public string SearchLogText { get => searchLogText; }
        #endregion

        #region Validation

        private int[] tilesCopy;
        private GameState gameStateCopy;
        #endregion

        public IAIPlayer GetInstance()
        {
            return new CompundAIGen4V1();
        }
        public string Name()
        {
            return AI_NAME;
        }
        public void Initialize(Board board, AISettings settings)
        {
            this.board = board;
            this.settings = settings;
            search = new Search(board, settings);
            isInitialized = true;

        }

        public void RequestMove()
        {
            if (!isInitialized)
                throw new InvalidOperationException("Search can't be requested before it has been initialized");

            #region Before_ValidCopy
            tilesCopy = new int[64];
            Array.Copy(board.tiles, tilesCopy, 64);
            gameStateCopy = new GameState(board.currGameState.gameStateValue, board.currGameState.PrevMove);
            #endregion

            //search.useCoinToss = hasRepetitions(privateBoard);

            if (settings.useBook && board.Turn < settings.maxBookPly)
            {
                if (GetBookMove(board))
                {
                    SubmitMove(bestMove);
                    return;
                }
            }
            if (board.Moves.Count == 0 && board.HasGeneratedMoves)
                throw new ArgumentNullException("board.Moves", "EndgameConditions are handeled by the game manager and not the AI");
            else if (!settings.useIterativeDeepening)
            {
                isSearching = true;
                StartSearchNonThreaded();
                SubmitMove(bestMove);
                isSearching = false;
                return;
            }
            else
            {

                Task.Factory.StartNew(() => search.SearchRecurr(), TaskCreationOptions.LongRunning);
                cancelSearchTimer = new CancellationTokenSource();
                Task.Delay(settings.searchTimeMillis, cancelSearchTimer.Token).ContinueWith((t) => SearchTimeEnded());
                isSearching = true;
            }
        }

        public void SearchCompleted(Move move)
        {
            cancelSearchTimer?.Cancel();
            this.bestMove = move;
            //TODO REIMPLEMENT
            //validate();
            searchLogText = "<color=yellow>" + search.LogDebugInfo() + "</color>";
            isSearching = false;
            SubmitMove(bestMove);
        }

        public void SearchTimeEnded()
        {
            if (cancelSearchTimer == null || !cancelSearchTimer.IsCancellationRequested)
            {
                search.StoppSearch();
                (bestMove, bestVal) = search.GetResult();
                //TODO REIMPLEMENT
                //validate();
                searchLogText = "<color=yellow>" + search.LogDebugInfo() + "</color>";
                isSearching = false;
                AIDebugger.instance.AddDebugInfo(search);
                SubmitMove(bestMove);
            }
            else
            {
                throw new InvalidOperationException("Someting went wrong when searching for move");
            }
        }

        public void SubmitMove(Move move)
        {

            isSearching = false;
            GameManager.instance.RecivePendingMove(move);
        }

        bool GetBookMove(Board board)
        {
            if (oppeningsBook == null)
                oppeningsBook = settings.GetOppeningsBook();
            if (oppeningsBook.Contains(board.ZobristKey))
            {
                bestMove = oppeningsBook.GetRandomMove(board.ZobristKey);
                bestVal = 0;
                searchLogText = "<color=yellow> Book Move: " + bestMove.ToString() + "</color>";
                return true;
            }
            return false;
        }

        void StartSearchNonThreaded()
        {
            isSearching = true;
            search.SearchRecurr();
            (bestMove, bestVal) = search.GetResult();
            searchLogText = "<color=yellow>" + search.LogDebugInfo() + "</color>";
            //TODO REIMPLEMENT
            //validate();
            isSearching = false;
        }
        public bool IsSearching()
        {
            return isSearching;
        }
    }
}