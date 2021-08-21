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

        [SerializeField]
        private AISettings[] aISettings;

        private IAIPlayer[] aIs = new IAIPlayer[0];
        public IAIPlayer[] AIs { get => aIs; }

        //[SerializeField] private IAIObject[] aIs = new IAIObject[0];
        //[HideInInspector] public IAIObject[] AIs { get { return aIs; } }


        public bool showing = false;
        public bool isBlackAIActive = false;
        public bool isWhiteAIActive = false;
        public IAIPlayer activeBlackAI = null;
        public IAIPlayer activeWhiteAI = null;
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
            //List<string> test = UnityUtills.GetAllEntities<IAIPlayer>();
            //
            //foreach (string str in test)
            //{
            //    Debug.Log(str);
            //}
            aIs = UnityUtills.GetAllEntitiesAsClasses<IAIPlayer>().ToArray();
            List<IAIPlayer> aIs2 = new List<IAIPlayer>();
            foreach (IAIPlayer iaiP in aIs)
            {
                aIs2.Add(iaiP);
                aIs2.Add(iaiP);
            }
            aIs = aIs2.ToArray();
            //aISettings = UnityUtills.GetAllInstances<AISettings>();

            int ii = 0;
            foreach (IAIPlayer ai in aIs)
            {
                string str = "AI [" + ii + "] :" + ai.Name();
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

                    activeWhiteAI = aIs[index].GetInstance();
                    if (index >= aISettings.Length)
                        throw new System.IndexOutOfRangeException("No settings available for AI[" + index + "]");
                    activeWhiteAI.Initialize(GameManager.instance.WhiteBoard, aISettings[index]);
                    isWhiteAIActive = true;
                    gameManager.wAI = activeWhiteAI;
                    gameManager.whiteAIPlaying = true;
                    gameManager.aiWaitingToMove = true;
                    wAINameDisplay.text = "<color=cyan><b>White AI: " + gameManager.wAI.Name() + "</b></color>";


                    wAINameDisplay.gameObject.SetActive(true);
                    gameManager.playAIMove();

                }
                else
                {
                    activeBlackAI = aIs[index].GetInstance();
                    activeBlackAI.Initialize(GameManager.instance.BlackBoard, aISettings[index]);
                    isBlackAIActive = true;
                    gameManager.bAI = activeBlackAI;
                    gameManager.blackAIPlaying = true;
                    bAINameDisplay.text = "<color=green><b>Black AI: " + gameManager.bAI.Name() + "</b></color>";
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
        public void RequestMove(IAIPlayer ai)
        {
            //timeID = TimeUtills.Instance.startMeasurement();
            if (!ai.IsSearching())
                ai.RequestMove();
            //long td = TimeUtills.Instance.stopMeasurementMillis(timeID);
            //Debug.Log(ai.Name + " took :" + td + "ms");
            //ConsoleHistory.instance.addLogHistory("\t<color=yellow> " + ai.Name + " took : " + td + "ms</color>");

        }
    }
}