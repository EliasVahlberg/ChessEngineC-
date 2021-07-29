using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Command;
using TMPro;
using UnityEngine;

public class DevConsoleBehaviour : MonoBehaviour
{
    [SerializeField] private string prefix = string.Empty;
    [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

    [Header("UI")]
    [SerializeField] public GameObject uiCanvas = null;
    [SerializeField] private TMP_InputField inputField = null;
    [Header("LogHistory")]
    [SerializeField] private int i1 = 0x4100FF;


    [SerializeField] private Color DATE_COL = new Color(0x41, 0x00, 0xFF, 0xFF);
    private string DateCol => ColorUtility.ToHtmlStringRGB(DATE_COL);
    [SerializeField] private Color COMMAND_COL = new Color(0xDE, 0x56, 0x56, 0xFF);
    private string CommandCol => ColorUtility.ToHtmlStringRGB(COMMAND_COL);
    [SerializeField] private Color INPUT_COMMAND_COL = new Color(0xB7, 0x3B, 0xFF, 0xFF);
    private string InputCommandCol => ColorUtility.ToHtmlStringRGB(INPUT_COMMAND_COL);
    [SerializeField] private Color ARGS_COL = new Color(0x53, 0xFF, 0xF3, 0xFF);
    private string ArgsCol => ColorUtility.ToHtmlStringRGB(ARGS_COL);
    [SerializeField] private Color RESPONSE_COL = new Color(0x36, 0xA8, 0x3C, 0xFF);
    private string ResponseCol => ColorUtility.ToHtmlStringRGB(RESPONSE_COL);
    [SerializeField] private Color RETURNED_RESPONSE_COL = new Color(0xFF, 0x32, 0x32, 0xFF);
    private string ReturnedResponseCol => ColorUtility.ToHtmlStringRGB(RETURNED_RESPONSE_COL);
    //0x 4100FF : 0x41, 0x00, 0xFF  :   DateCol 
    //0x DE5656 : 0xDE, 0x56, 0x56  :   CommandCol 
    //0x B73BFF : 0xB7, 0x3B, 0xFF  :   InputCommandCol 
    //0x 53FFF3 : 0x53, 0xFF, 0xF3  :   ArgsCol
    //0x 36A83C : 0x36, 0xA8, 0x3C  :   ResponseCol
    //0x FF3232 : 0xFF, 0x32, 0x32  :   ReturnedResponseCol

    public bool active = false;
    private float pausTimeScale; //USED WITH TIMESCALE
    public static DevConsoleBehaviour instance; //* SINGELTON
    private DevConsole devConsole;
    private DevConsole DConsole
    {
        get
        {
            if (devConsole != null) { return devConsole; }
            return devConsole = new DevConsole(prefix, commands);
        }
    }
    private ConsoleHistory DConsoleHistory => ConsoleHistory.instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        active = uiCanvas.activeSelf;
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void ToggleDevConsole()
    {
        if (uiCanvas.activeSelf)
        {
            //Time.timeScale = pausTimeScale;
            uiCanvas.SetActive(false);
            active = false;
        }
        else
        {
            //pausTimeScale = Time.timeScale;
            //Time.timeScale = 0;
            uiCanvas.SetActive(true);
            inputField.ActivateInputField();
            active = true;
        }

    }
    //*inputValue is always null for some reason

    public void ProcessCommand(string inputValue)
    {
        string actualInput = inputField.text;
        string actualInput2 = (string)actualInput.Clone();
        string response = DConsole.ProcessCommand(actualInput);

        DateTime now = DateTime.Now;
        if (actualInput2 == null || actualInput2.Length < prefix.Length)
        {
            DConsoleHistory.addLogHistory($"  <color=#{DateCol}>[{now.Day}:{now.Hour}:{now.Minute}:{now.Second}]</color> <color=#{CommandCol}>Command:</color>  <color=#{InputCommandCol}>NONE</color> <color=#{ArgsCol}>{{ NONE }}</color>\n\t<color=#{ResponseCol}>Response:</color>  <color=#{ReturnedResponseCol}>{response}</color>");
            inputField.text = string.Empty;
            return;
        }

        actualInput2 = actualInput2.Remove(0, prefix.Length);

        string[] inputSplit = actualInput2.Split(' ');
        string command = inputSplit[0];
        actualInput2 = inputSplit.Length <= 1 ? "NONE" : actualInput2.Remove(0, command.Length + 1);
        string s = $"  <color=#{DateCol}>[{now.Day}:{now.Hour}:{now.Minute}:{now.Second}]</color> <color=#{CommandCol}>Command:</color>  <color=#{InputCommandCol}>{command}</color> <color=#{ArgsCol}>{{ {actualInput} }}</color>\n\t<color=#{ResponseCol}>Response:</color>	<color=#{ReturnedResponseCol}>{response}</color>";
        inputField.text = string.Empty;
        DConsoleHistory.addLogHistory(s);
        inputField.ActivateInputField();

    }
    /*
    {DateCol}
    {CommandCol}
    {InputCommandCol}
    {ArgsCol}
    {ResponseCol}
    {ReturnedResponseCol}
    */


}
