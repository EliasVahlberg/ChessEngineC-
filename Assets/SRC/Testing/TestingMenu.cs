using UnityEngine;
using UnityEngine.UI;
using ChessAI;
using System.Collections.Generic;
using static UnityEngine.UI.Dropdown;
using System.Text;

namespace Testing
{


    public class TestingMenu : MonoBehaviour
    {
        [HideInInspector] static public TestingMenu instance;
        public GameObject canvas;
        public bool showing = false;

        #region AITEST UI
        [Header("AI PERFT UI")]
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
        #region ENGINETEST UI
        [Header("ENGINE PERFT UI")]
        [SerializeField] private InputField startingFenEngineTest;
        [SerializeField] private Toggle useStartingFenEngineTest;
        [SerializeField] private Dropdown selectNumberOfItterationsEngineTest;
        [SerializeField] private Dropdown selectNumberOfMovesEngineTest;
        [SerializeField] private Button startEngineTestButton;
        [SerializeField] private InputField resultFieldEngineTest;

        private List<OptionData> nItterationsOptions = new List<OptionData>(){
            new OptionData("100"),
            new OptionData("200"),
            new OptionData("300"),
            new OptionData("400"),
            new OptionData("500"),
            new OptionData("600"),
            new OptionData("700"),
            new OptionData("800"),
            new OptionData("900"),
            new OptionData("1000")
            };
        #endregion
        #region PerftCheck
        [SerializeField] private Dropdown selectPlyPerftCheck;
        [SerializeField] private Dropdown selectFenPerftCheck;
        [SerializeField] private Button startPerftCheckButton;
        [SerializeField] private InputField resultFieldPerftCheck;
        private List<OptionData> plyOptionsPerftCheck = new List<OptionData>(){
            new OptionData("1 ply"),
            new OptionData("2 ply"),
            new OptionData("3 ply"),
            new OptionData("4 ply"),
            new OptionData("5 ply"),
            new OptionData("6 ply "),
            new OptionData("7 ply"),
            };
        private List<OptionData> fenOptionsPerftCheck = new List<OptionData>(){
            new OptionData("Initial Position"),
            new OptionData("Position 2"),
            new OptionData("Position 3"),
            new OptionData("Position 4"),
            new OptionData("Position 5"),
            new OptionData("Position 6 "),
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
            #region AIPERFT

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
            #endregion
            #region EnginePerft
            selectNumberOfMovesEngineTest.options = nMovesOptions;
            selectNumberOfItterationsEngineTest.options = nItterationsOptions;
            resultFieldEngineTest.readOnly = true;
            startEngineTestButton.onClick.AddListener(runEnginePerfTest);

            #endregion
            #region PerftCheck
            selectPlyPerftCheck.options = plyOptionsPerftCheck;
            selectFenPerftCheck.options = fenOptionsPerftCheck;
            resultFieldPerftCheck.readOnly = true;
            startPerftCheckButton.onClick.AddListener(runPerftCheck);
            #endregion
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
            if (useStartingFenEngineTest.isOn)
            {
                string fen = startingFenEngineTest.text;
                result = PreformanceTest.startAIMoveTest(wAI, bAI, fen, moves);
            }
            else
            {
                result = PreformanceTest.startAIMoveTest(wAI, bAI, moves);
            }
            PrintResultAIPerft(result);
        }

        private void PrintResultAIPerft(PreformanceTestResult result)
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
        public void runEnginePerfTest()
        {
            int itterations = (selectNumberOfItterationsEngineTest.value + 1) * 100;
            int moves = (selectNumberOfMovesEngineTest.value + 1) * 10;
            ChessEnginePerftResult result;
            if (useStartingFen.isOn)
            {
                string fen = startingFenInput.text;
                result = PreformanceTest.enginePreformanceTest(moves, itterations, fen);
            }
            else
            {
                result = PreformanceTest.enginePreformanceTest(moves, itterations);
            }
            PrintResultEnginePerft(result);

        }

        private void PrintResultEnginePerft(ChessEnginePerftResult result)
        {
            string restxt =
            "RESULTS: \n" +
            "Number of total moves played: " + result.nTotalMoves + "\n" +
            "Number of itterations : " + result.itterations + "\n" +
            "Total time: " + result.totalTime + "ms \n" +
            "Avrage time per itteration: " + result.avrageTimePerItteration + "ms \n";
            //"Memory used : " + result.memoryUsed;
            resultFieldEngineTest.readOnly = false;
            resultFieldEngineTest.text = restxt;
            resultFieldEngineTest.readOnly = true;
        }

        public void runPerftCheck()
        {
            int ply = selectPlyPerftCheck.value + 1;
            int fen = selectFenPerftCheck.value;
            PrintResultPerftCheck(MoveTest.PerftCheck(fen, ply));

        }
        private void PrintResultPerftCheck(PerftCheckResult result)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<b>RESULTS:</b> \n");
            stringBuilder.Append("<b>Number of total nodes:</b> " + result.nTotalNodes + "\n");
            for (int ii = 0; ii < result.result.Length; ii++)
            {
                stringBuilder.Append($"Ply <b>{ii + 1}</b>: {{ result = ");
                stringBuilder.Append("<color=cyan>" + result.result[ii] + "</color>");
                stringBuilder.Append(", <b>accurate</b> = ");
                stringBuilder.Append((result.result[ii] == result.actual[ii]) ? "<color=green><b> YES </b></color>" : "<color=red><b> NO </b></color>");
                stringBuilder.Append(", <b>actual</b> = ");
                stringBuilder.Append("<color=yellow>" + result.actual[ii] + "</color>");
                stringBuilder.Append(" }\n");
            }
            stringBuilder.Append("<b>Fen starting position:</b>\n");
            stringBuilder.Append(result.fenStartPosition + "\n");

            resultFieldPerftCheck.readOnly = false;
            resultFieldPerftCheck.text = stringBuilder.ToString();
            resultFieldPerftCheck.readOnly = true;
        }
    }
}