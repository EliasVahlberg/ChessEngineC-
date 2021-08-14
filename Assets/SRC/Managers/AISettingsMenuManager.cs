using System.Collections;
using System.Collections.Generic;
using ChessAI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    private void Start()
    {
        Apply.onClick.AddListener(onApply);
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

    }

}