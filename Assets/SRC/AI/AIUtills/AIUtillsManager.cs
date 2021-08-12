
using System;
using UnityEngine;
namespace ChessAI
{
    /*
    @File AIUtillsManager.cs
    @author Elias Vahlberg 
    @Date 2021-07 
    */
    public class AIUtillsManager : MonoBehaviour
    {

        public static AIUtillsManager instance;
        private BoardScoreGenerator boardScoreGenerator;
        public BoardScoreGenerator BoardScoreGen
        {
            get
            {
                if (boardScoreGenerator == null)
                    boardScoreGenerator = new BoardScoreGenerator();
                return boardScoreGenerator;
            }
        }
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
        public bool BoardIntegrityCheck(Board board, int[] prevTiles, GameState prevGamestate)
        {
            if (board.tiles == prevTiles)
                throw new ArgumentException("Pointer to tiles array is the same as prevTiles... This proves nothing");
            bool mutated = false;
            for (int ii = 0; ii < 64; ii++)
            {
                if (board.tiles[ii] != prevTiles[ii])
                {
                    mutated = true;
                    Debug.LogError("position: " + ii + "is changed");
                    Debug.LogError("Is =" + prevTiles[ii] + " Should be =" + board.tiles[ii]);

                }
            }
            if (board.currGameState.gameStateValue != prevGamestate.gameStateValue)
                return false;
            if (board.currGameState.PrevMove.MoveValue != prevGamestate.PrevMove.MoveValue)
                return false;
            return true;

        }
    }
}