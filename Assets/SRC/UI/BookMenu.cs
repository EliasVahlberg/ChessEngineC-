using System.Collections;
using System.Collections.Generic;
using ChessAI;
using UnityEngine;
using UnityEngine.UI;
using Utills;
using static UnityEngine.UI.Dropdown;

public class BookMenu : MonoBehaviour
{
    private AISettings[] aISettings;
    [SerializeField]
    private AISettings aISettingsSelected;
    [SerializeField]
    private Dropdown AISettingsSelect;
    [SerializeField]
    private InputField maxPlyToRecordField;
    [SerializeField]
    private InputField minimumOcurrancesField;
    [SerializeField]
    private InputField satusField;

    void Start()
    {
        aISettings = AISettingsMenuManager.instance.AISettings;
        List<OptionData> options = new List<OptionData>();
        for (int ii = 0; ii < aISettings.Length; ii++)
        {
            options.Add(new OptionData(aISettings[ii].name));
        }
        AISettingsSelect.options = options;
        if (options.Count != 0)
            SelectSettings();

    }

    public void SelectSettings()
    {
        int ii = AISettingsSelect.value;
        if (ii < aISettings.Length)
        {
            aISettingsSelected = aISettings[ii];
            updateStatusText();
        }
    }
    public void updateStatusText()
    {
        //<color>{aISettingsSelected.name}</color>
        string str = $"Selected : <color=yellow>{aISettingsSelected.name}</color> \nCurrent Book : <color=yellow>";
        if (aISettingsSelected.useExternalBook)
        {
            string[] path = aISettingsSelected.externalBookPath.Split('\\');
            str += path[path.Length - 1] + "(External) \n";
        }
        else
            str += aISettingsSelected.book.name + " \n";
        str += $"</color>Max Ply : <color=yellow>{aISettingsSelected.maxBookPly}</color> ";
        satusField.text = str;
    }
    public void setMaxPlyToRecord()
    {
        string input = maxPlyToRecordField.text;
        Debug.Log("INPUT = " + input);
        int n;
        if (int.TryParse(input, out n))
            BookBuilder.instance.maxPlyToRecord = n;
    }
    public void setMinimumOcurrances()
    {
        string input = minimumOcurrancesField.text;
        int n;
        if (int.TryParse(input, out n))
            BookBuilder.instance.minMovePlayCount = n;
    }
#if UNITY_STANDALONE_WIN
    public void ParsePGNFiles()
    {
        Debug.Log("LOAD");
        PGNMentorParser.instance.GetGamesFiles(ParsePGNFilesCallback);

    }

    public void ParsePGNFilesCallback()
    {
        Debug.Log("PARSE AND SAVE");
        PGNMentorParser.instance.ParseAllRuntime();
    }

    public void CreateBook()
    {

        BookBuilder.instance.PreBuildAndSaveBookRuntime();

    }

    public void SetBook()
    {
        if (aISettingsSelected == null)
            return;
        FileUtills.instance.GetPathFromFileExplorer("Text files (*.book) | *.book", str => SetBookCallback(str));

    }

    private void SetBookCallback(string path)
    {
        aISettingsSelected.externalBookPath = path;
        aISettingsSelected.useExternalBook = true;
        updateStatusText();
    }

    public void SetDefaultBook()
    {
        aISettingsSelected.externalBookPath = "";
        aISettingsSelected.useExternalBook = false;
    }


#endif
}