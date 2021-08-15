using System.Collections.Generic;

namespace ChessAI
{
    [System.Serializable]
    public class OppeningsBook
    {
        public Dictionary<ulong, PositionPage> pages;

        public PositionPage this[ulong key] => pages[key];

        public OppeningsBook()
        {
            pages = new Dictionary<ulong, PositionPage>();
        }

        public bool Contains(ulong positionHash)
        {
            return pages.ContainsKey(positionHash);
        }

        public Move GetRandomMove(ulong key)
        {
            PositionPage page = this[key];
            ushort[] moves = new List<ushort>(page.timesMovePlayed.Keys).ToArray();
            System.Random rand = new System.Random();
            ushort rndMoveValue = moves[rand.Next(0, moves.Length)];
            return new Move(rndMoveValue);
        }

        public void Add(ulong key, Move move)
        {
            Add(key, move, 1);
        }

        public void Add(ulong key, Move move, int timesPlayed)
        {
            if (!pages.ContainsKey(key))
            {
                pages.Add(key, new PositionPage());
            }

            pages[key].Add(move, timesPlayed);
        }
        //TODO ADD SmoothWeights and GetRandomBookMoveWeighted
    }

    public class PositionPage
    {
        public Dictionary<ushort, int> timesMovePlayed;
        public PositionPage()
        {
            timesMovePlayed = new Dictionary<ushort, int>();
        }
        public void Add(Move move, int timesPlayed = 1)
        {
            ushort moveValue = move.MoveValue;

            if (timesMovePlayed.ContainsKey(moveValue))
            {
                timesMovePlayed[moveValue]++;
            }
            else
            {
                timesMovePlayed.Add(moveValue, timesPlayed);
            }
        }
    }
}