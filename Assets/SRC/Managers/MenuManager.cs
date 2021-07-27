using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [HideInInspector]
    private UIManager uiManager;
    [HideInInspector]
    private GameManager gameManager;
    [HideInInspector]
    private SettingsManager settingsManager;
    public Button playDefaultStart, playusingFen, resetButton, openNetworkMenuButton, quitButton, forfitButton, openSettingsMenuButton, returnMainMenuButton;
    public Text inputText;
    public InputField fenInputFeild;
    public Text inputErrText;

    public Button closeNetworkMenuButton;

    public Text networkStatusText;

    public GameObject canvas;
    public GameObject networkMenuCanvas;
    public bool showing = true;
    public bool showingNetworkMenu = false;
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
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        gameManager = FindObjectOfType<GameManager>();
        settingsManager = FindObjectOfType<SettingsManager>();

        playDefaultStart.onClick.AddListener(defaultStart);
        playusingFen.onClick.AddListener(usingFen);
        resetButton.onClick.AddListener(resetBoard);
        fenInputFeild.onEndEdit.AddListener(delegate { checkFEN(fenInputFeild); });
        openNetworkMenuButton.onClick.AddListener(showNetworkMenu);
        quitButton.onClick.AddListener(quit);
        forfitButton.onClick.AddListener(forfit);
        openSettingsMenuButton.onClick.AddListener(showSettingsMenu);
        returnMainMenuButton.onClick.AddListener(returnMainMenu);


        closeNetworkMenuButton.onClick.AddListener(hideNetworkMenu);


        resetButton.gameObject.SetActive(false);
        forfitButton.gameObject.SetActive(false);
        returnMainMenuButton.gameObject.SetActive(false);
        showingNetworkMenu = true;
        hideNetworkMenu(false);
        canvas.SetActive(true);
        //hideMenu();
        //showMenu();

    }
    void Update()
    {

    }
    public void defaultStart()
    {
        Debug.Log("DEFSTART");
        gameManager.resetBoard();
        hideMainMenu();
    }
    public void usingFen()
    {
        Debug.Log("FEN");
        string fen = inputText.text;
        int[] valFenSections = FENUtills.validFen(fen);
        if (valFenSections[0] >= 2)
        {
            string[] sections = fen.Split(' ');
            string validFEN = "";
            for (int i = 0; i <= valFenSections[0]; i++)
                validFEN += (i != 0 ? " " : "") + sections[i];

            gameManager.resetBoard(validFEN);
            hideMainMenu();

        }
    }
    public void checkFEN(InputField fenInput)
    {
        Debug.Log("ENDEDIT");
        string fen = inputText.text;
        int[] valFenSections = FENUtills.validFen(fen);
        if (valFenSections[0] >= 2)
        {
            string[] sections = fen.Split(' ');
            string validFEN = "";
            for (int i = 0; i <= valFenSections[0]; i++)
                validFEN += (i != 0 ? " " : "") + sections[i];

            if (valFenSections[1] != 0)
                inputErrText.text = "Check failed fail type: { " + valFenSections[1] + ", " + FENUtills.failTypeDict[valFenSections[1]] + " } \nStill able to start game.";
            else
                inputErrText.text = "";
            return;
        }
        inputErrText.text = "Check failed fail type: { " + valFenSections[1] + ", " + FENUtills.failTypeDict[valFenSections[1]] + " } \n Unable to start game.";
    }
    public void hideMainMenu()
    {
        if (gameManager.started || showingNetworkMenu || settingsManager.showing)
        {
            canvas.SetActive(false);
            resetButton.gameObject.SetActive(false);
            showing = false;
        }
    }
    public void showMainMenu()
    {

        canvas.SetActive(true);
        if (gameManager.started)
        {
            playDefaultStart.gameObject.SetActive(false);
            resetButton.gameObject.SetActive(true);
            forfitButton.gameObject.SetActive(true);
            openNetworkMenuButton.gameObject.SetActive(false);
            openSettingsMenuButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(false);
            returnMainMenuButton.gameObject.SetActive(true);
            fenInputFeild.interactable = true;
            fenInputFeild.text =
            gameManager.board.boardToFEN();
        }
        else
        {
            forfitButton.gameObject.SetActive(false);
            resetButton.gameObject.SetActive(false);
            openSettingsMenuButton.gameObject.SetActive(true);
            playDefaultStart.gameObject.SetActive(true);
            openNetworkMenuButton.gameObject.SetActive(true);
        }
        showing = true;
    }
    public void resetBoard()
    {
        Debug.Log("RESET");
        gameManager.resetBoard();
        hideMainMenu();
    }
    public void onPointerEnter()
    { }
    public void forfit()
    {
        //TODO ADD
        if (!gameManager.isNetworked)
        {
            if (gameManager.board.whiteTurn)
                gameManager.whiteForfit = true;
            else
                gameManager.blackForfit = true;
            hideMainMenu();
            gameManager.forfit();
        }
    }
    public void quit()
    {
        Application.Quit(0);
    }
    public void showNetworkMenu()
    {

        if (!showingNetworkMenu)
        {
            networkMenuCanvas.SetActive(true);
            showingNetworkMenu = true;
            hideMainMenu();
        }
    }
    public void hideNetworkMenu()
    {
        if (showingNetworkMenu)
        {
            networkMenuCanvas.SetActive(false);
            showMainMenu();
            showingNetworkMenu = false;
        }
    }
    public void hideNetworkMenu(bool showMainM)
    {
        if (showingNetworkMenu)
        {
            networkMenuCanvas.SetActive(false);
            if (showMainM)
                showMainMenu();
            showingNetworkMenu = false;
        }
    }




    public void showSettingsMenu()
    {
        if (!settingsManager.showing)
        {
            settingsManager.showSettingsMenu();
            hideMainMenu();
        }
    }
    public void returnMainMenu()
    {
        gameManager.started = false;
        uiManager.hideBoard();
        hideMainMenu();
        showMainMenu();
    }

}
