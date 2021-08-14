using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utills;
using static UnityEngine.UI.Dropdown;

namespace ChessAI
{
    /*
    @File IAManager.cs
    @author Elias Vahlberg 
    @Date 2021-07 
    */
    public class AIManager : MonoBehaviour
    {

        [HideInInspector] public static AIManager instance;
        [SerializeField] private GameObject canvas;
        [Header("UI")]
        [SerializeField] private IAIObject[] aIs = new IAIObject[0];
        [HideInInspector] public IAIObject[] AIs { get { return aIs; } }
        public bool showing = false;
        public bool isBlackAIActive = false;
        public bool isWhiteAIActive = false;
        public IAIObject activeBlackAI = null;
        public IAIObject activeWhiteAI = null;
        public Button letAIPlayButton;
        public Button toggleAIPausButton;
        public Text wAINameDisplay;
        public Text bAINameDisplay;
        public Dropdown aiSelect;
        private List<OptionData> aiOptions = new List<OptionData>();
        private int timeID = 0;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                //instance.gameObject.SetActive(false);
            }
            else if (instance != this)
            {
                Debug.Log("SAMEINSTACE ");
                Destroy(this);
            }
        }
        private void Start()
        {
            aIs = UnityUtills.GetAllInstances<IAIObject>();

            int ii = 0;
            foreach (IAIObject ai in aIs)
            {
                string str = "AI [" + ii + "] :" + ai.Name;
                aiOptions.Add(new OptionData(str));
                ii++;
            }
            Debug.Log("Num ais: " + ii);
            aiSelect.options = aiOptions;
            aiSelect.value = aiOptions.Count - 1;
            canvas.SetActive(false);
        }
        public void showAIMenu()
        {
            canvas.SetActive(true);
        }
        public void hideAIMenu()
        {
            canvas.SetActive(false);
        }
        public void letAITakeOver(int index)
        {
            GameManager gameManager = GameManager.instance;
            if (gameManager.started)
            {
                if (gameManager.board.whiteTurn)
                {
                    activeWhiteAI = Instantiate(aIs[index]);
                    isWhiteAIActive = true;
                    gameManager.wAI = activeWhiteAI;
                    gameManager.whiteAIPlaying = true;
                    gameManager.aiWaitingToMove = true;
                    wAINameDisplay.text = "<color=cyan><b>White AI: " + gameManager.wAI.Name + "</b></color>";


                    wAINameDisplay.gameObject.SetActive(true);
                    gameManager.playAIMove();

                }
                else
                {
                    activeBlackAI = Instantiate(aIs[index]);
                    isBlackAIActive = true;
                    gameManager.bAI = activeBlackAI;
                    gameManager.blackAIPlaying = true;
                    bAINameDisplay.text = "<color=green><b>Black AI: " + gameManager.bAI.Name + "</b></color>";
                    bAINameDisplay.gameObject.SetActive(true);
                    gameManager.playAIMove();
                }
            }
            else
                Debug.Log("Can't let AI play the game has not started!");
        }
        public void letAITakeOver()
        {
            letAITakeOver(aiSelect.value);
        }
        public void resetAIManager()
        {
            isBlackAIActive = false;
            isWhiteAIActive = false;
            activeBlackAI = null;
            activeWhiteAI = null;
            wAINameDisplay.text = "";
            bAINameDisplay.text = "";
            toggleAIPausButton.gameObject.SetActive(false);
            aiSelect.gameObject.SetActive(true);
            letAIPlayButton.gameObject.SetActive(true);
        }
        public void toggleAIPaus()
        {
            GameManager.instance.toggleAIPaus();
            if (GameManager.instance.isAIPaused)
                toggleAIPausButton.GetComponentInChildren<Text>().text = "<color=blue><b>Resume AI</b></color>";
            else
                toggleAIPausButton.GetComponentInChildren<Text>().text = "<color=blue><b>Paus AI</b></color>";

        }
        public Move SelectMove(IAIObject ai, Board board)
        {
            //timeID = TimeUtills.Instance.startMeasurement();
            Move move = ai.SelectMove(board);
            //long td = TimeUtills.Instance.stopMeasurementMillis(timeID);
            //Debug.Log(ai.Name + " took :" + td + "ms");
            //ConsoleHistory.instance.addLogHistory("\t<color=yellow> " + ai.Name + " took : " + td + "ms</color>");
            return move;
        }
    }
}