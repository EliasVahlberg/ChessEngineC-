using System.Collections.Generic;
using UnityEngine;
namespace ChessAI
{
    public class AIManager : MonoBehaviour
    {

        [HideInInspector] public static AIManager instance;
        [Header("UI")]
        [SerializeField] private IAIObject[] aIs = new IAIObject[0];
        public bool showing = false;
        public bool isBlackAIActive = false;
        public bool isWhiteAIActive = false;
        public IAIObject activeBlackAI = null;
        public IAIObject activeWhiteAI = null;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                instance.gameObject.SetActive(false);
            }
            else if (instance != this)
            {
                Debug.Log("SAMEINSTACE ");
                Destroy(this);
            }
        }
        public void showAIMenu()
        {
            instance.gameObject.SetActive(true);
        }
        public void hideAIMenu()
        {
            instance.gameObject.SetActive(false);
        }
        public void letAITakeOver(int index)
        {

        }
        public void letAITakeOver()
        {
            GameManager gameManager = GameManager.instance;
            if (gameManager.started)
            {
                if (gameManager.board.whiteTurn)
                {
                    activeWhiteAI = aIs[0];
                    isWhiteAIActive = true;
                    gameManager.wAI = activeWhiteAI;
                    gameManager.whiteAIPlaying = true;
                    gameManager.playAIMove();

                }
                else
                {
                    activeBlackAI = aIs[0];
                    isBlackAIActive = true;
                    gameManager.bAI = activeBlackAI;
                    gameManager.blackAIPlaying = true;
                    gameManager.playAIMove();
                }
            }
            else
                Debug.Log("Can't let AI play the game has not started!");
        }
    }
}