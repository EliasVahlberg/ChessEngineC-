using System.Collections;
using System.Collections.Generic;
using ChessAI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;
/*
@File AISettingsMenuManager.cs
@author Elias Vahlberg 
@Date 2021-07
*/
public class AISettingsMenuManager : MonoBehaviour
{
    public static AISettingsMenuManager instance;
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

    [SerializeField]
    private AISettings targetSettings;
    [SerializeField]
    private InputField SearchTimeMillisInput;
    [SerializeField]
    private InputField MaxNumBookMovesInput;
    [SerializeField]
    private InputField CaptureWeightInput;
    [SerializeField]
    private InputField MapWeightInput;
    [SerializeField]
    private Toggle UseBookToggle;
    [SerializeField]
    private Toggle UseQSearchToggle;
    [SerializeField]
    private Toggle UseMapWeightToggle;
    [SerializeField]
    private Button Apply;

    [SerializeField]
    private InputField statusField;

    [SerializeField]
    private AISettings[] aISettings;
    public AISettings[] AISettings { get => aISettings; set => aISettings = value; }
    [SerializeField]
    private Dropdown AISettingsSelect;


    void Start()
    {
        List<OptionData> options = new List<OptionData>();
        for (int ii = 0; ii < aISettings.Length; ii++)
        {
            options.Add(new OptionData(aISettings[ii].name));
        }
        AISettingsSelect.options = options;
        if (options.Count != 0)
            SelectSettings(0);


        Apply.onClick.AddListener(onApply);
        AISettingsSelect.onValueChanged.AddListener(n => SelectSettings(n));
    }

    private void onApply()
    {
        int searchTimeMillis;
        if (int.TryParse(SearchTimeMillisInput.text, out searchTimeMillis))
            targetSettings.searchTimeMillis = searchTimeMillis;
        int numBookMoves;
        if (int.TryParse(MaxNumBookMovesInput.text, out numBookMoves))
            targetSettings.maxBookPly = numBookMoves;
        int captureWeight;
        if (int.TryParse(CaptureWeightInput.text, out captureWeight))
            targetSettings.captureWeight = captureWeight;
        int mapWeight;
        if (int.TryParse(MapWeightInput.text, out mapWeight))
            targetSettings.weightMapWeight = mapWeight;
        targetSettings.useBook = UseBookToggle.isOn;
        targetSettings.useQuiescenceSearch = UseQSearchToggle.isOn;
        targetSettings.useWeightMap = UseMapWeightToggle.isOn;
        updateStatusText();

    }

    public void SelectSettings(int ii)
    {
        if (ii < aISettings.Length)
        {
            targetSettings = aISettings[ii];
            updateStatusText();
        }
    }

    public void updateStatusText()
    {
        string str = $"Selected : <color=green>{targetSettings.name}</color>\n";

        str += $"Search Time : <color=yellow>{targetSettings.searchTimeMillis}</color>\n";
        str += $"Search Time : <color=yellow>{targetSettings.searchTimeMillis}</color>\n";
        str += $"Using book : <color=yellow>{targetSettings.useBook}</color>\n";
        str += $"Using Map Weight: <color=yellow>{targetSettings.useWeightMap}</color>\n";
        str += $"Using Itterative Deepening: <color=yellow>{targetSettings.useIterativeDeepening}</color>\n";
        str += $"Using Transposition Table: <color=yellow>{targetSettings.useTranspositionTable}</color>\n";
        str += $"Capture Weight : <color=yellow>{targetSettings.weightMapWeight}</color>\n";
        str += $"Map Weight : <color=yellow>{targetSettings.weightMapWeight}</color>\n";
        str += $"Max Book Ply : <color=yellow>{targetSettings.maxBookPly}</color>\n";
        str += "Current Book : <color=yellow>";
        if (targetSettings.useExternalBook)
        {
            string[] path = targetSettings.externalBookPath.Split('\\');
            str += path[path.Length - 1] + "(External)</color>\n";
        }
        else
            str += targetSettings.book.name + "</color>\n";
        statusField.text = str;
    }
}