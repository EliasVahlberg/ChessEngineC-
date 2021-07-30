using UnityEngine;
using UnityEngine.UI;
using ChessAI;
using System.Collections.Generic;
using static UnityEngine.UI.Dropdown;

namespace Testing
{


    public class TestingMenu : MonoBehaviour
    {
        [HideInInspector] static public TestingMenu instance;
        public GameObject canvas;
        public bool showing = false;

        #region UI
        [Header("UI")]
        [SerializeField] private Dropdown wAISelect;
        [SerializeField] private Dropdown bAISelect;
        [SerializeField] private InputField startingFenInput;
        [SerializeField] private Toggle useStartingFen;
        [SerializeField] private Dropdown selectNumberOfMoves;
        [SerializeField] private Button startTestButton;
        [SerializeField] private InputField resultField;
        private List<OptionData> aiOptions = new List<OptionData>();
        private List<OptionData> nMovesOptions = new List<OptionData>(){
            new OptionData("10"),
            new OptionData("20"),
            new OptionData("30"),
            new OptionData("40"),
            new OptionData("50"),
            new OptionData("60"),
            new OptionData("70"),
            new OptionData("80"),
            new OptionData("90"),
            new OptionData("100")
            };
        #endregion

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
        private void Start()
        {
            int ii = 0;
            foreach (IAIObject ai in AIManager.instance.AIs)
            {
                string str = "AI [" + ii + "] :" + ai.Name;
                aiOptions.Add(new OptionData(str));
                ii++;
            }
            wAISelect.options = aiOptions;
            bAISelect.options = aiOptions;
            selectNumberOfMoves.options = nMovesOptions;
            resultField.readOnly = true;
            startTestButton.onClick.AddListener(runAIPerfTest);
        }

        public void Activate()
        {
            if (MenuManager.instance.showing)
            {
                MenuManager.instance.hideMainMenu();
                canvas.SetActive(true);
                showing = true;
            }
        }

        public void Deactivate()
        {

            MenuManager.instance.showMainMenu();
            canvas.SetActive(false);
            showing = false;
        }

        public void runAIPerfTest()
        {
            IAIObject wAI = AIManager.instance.AIs[wAISelect.value];
            IAIObject bAI = AIManager.instance.AIs[bAISelect.value];
            int moves = (selectNumberOfMoves.value + 1) * 10;
            PreformanceTestResult result;
            if (useStartingFen.isOn)
            {
                string fen = startingFenInput.text;
                result = PreformanceTest.startAIMoveTest(wAI, bAI, fen, moves);
            }
            else
            {
                result = PreformanceTest.startAIMoveTest(wAI, bAI, moves);
            }
            PrintResult(result);
        }

        private void PrintResult(PreformanceTestResult result)
        {
            string restxt =
            "RESULTS: \n" +
            "Total time: " + result.time + "ms \n" +
            "Avrage time per turn: " + result.avrageTimePerTurn + "ms \n" +
            "Number of moves played: " + result.moves + "\n" +
            "White AI points: " + result.wPoints + "\n" +
            "Black AI points: " + result.bPoints + "\n" +
            "Fen after the last move: \n" + result.lastMoveFen;
            resultField.readOnly = false;
            resultField.text = restxt;
            resultField.readOnly = true;
        }
    }
}