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
    [Header("Main Menu")]
    public Button playDefaultStart, playusingFen, resetButton, openNetworkMenuButton, quitButton, forfitButton, openSettingsMenuButton, returnMainMenuButton, inGameDisconnectButton;
    public Text inputText;
    public InputField fenInputFeild;
    public Text inputErrText;
    public GameObject canvas;
    [Header("Network Menu")]
    public GameObject networkMenuCanvas;
    public Text networkStatusText;
    public Button closeNetworkMenuButton;
    [Header("Lobby")]
    public GameObject LobbyCanvas;
    public InputField fenInputFeildLobby;
    public Button playDefaultStartLobby, playUsingFenLobby, disconnectButtonLobby, openSettingsMenuButtonLobby;
    public Toggle playAsWhite;
    public Text inputErrTextLobby;
    [Header("State")]
    public bool showing = true;
    public bool showingNetworkMenu = false;
    public bool isInLobby = false;
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
        inGameDisconnectButton.onClick.AddListener(disconnectLobby);


        closeNetworkMenuButton.onClick.AddListener(hideNetworkMenu);

        inGameDisconnectButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);
        forfitButton.gameObject.SetActive(false);
        returnMainMenuButton.gameObject.SetActive(false);
        showingNetworkMenu = true;
        hideNetworkMenu(false);
        canvas.SetActive(true);

        #region Lobby
        playDefaultStartLobby.onClick.AddListener(defaultStartLobby);
        playUsingFenLobby.onClick.AddListener(usingFenLobby);
        disconnectButtonLobby.onClick.AddListener(disconnectLobby);
        openSettingsMenuButtonLobby.onClick.AddListener(showSettingsMenuLobby);
        fenInputFeild.onEndEdit.AddListener(delegate { checkFENLobby(fenInputFeild); });
        LobbyCanvas.SetActive(false);
        #endregion
        //hideMenu();
        //showMenu();

    }

    public void defaultStart()
    {
        Debug.Log("DEFSTART");
        GameManager.instance.onResetGame();
        hideMainMenu();
    }

    public void usingFen()
    {
        //Debug.Log("FEN");
        string fen = inputText.text;
        int[] valFenSections = FENUtills.validFen(fen);
        if (valFenSections[0] >= 2)
        {
            string[] sections = fen.Split(' ');
            string validFEN = "";
            for (int i = 0; i <= valFenSections[0]; i++)
                validFEN += (i != 0 ? " " : "") + sections[i];

            GameManager.instance.onResetGame(validFEN);
            hideMainMenu();

        }
    }

    public void checkFEN(InputField fenInput)
    {
        //Debug.Log("ENDEDIT");
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
            if (isInLobby)
            {
                inGameDisconnectButton.gameObject.SetActive(true);
                //returnMainMenuButton.gameObject.SetActive(false);
            }
            else
            {
                //returnMainMenuButton.gameObject.SetActive(true);
                inGameDisconnectButton.gameObject.SetActive(false);
            }
            playDefaultStart.gameObject.SetActive(false);
            //resetButton.gameObject.SetActive(true);
            playusingFen.gameObject.SetActive(false);
            forfitButton.gameObject.SetActive(true);
            openNetworkMenuButton.gameObject.SetActive(false);
            openSettingsMenuButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(false);
            fenInputFeild.interactable = true;
            fenInputFeild.text =
            gameManager.board.boardToFEN();
        }
        else
        {
            playusingFen.gameObject.SetActive(true);
            returnMainMenuButton.gameObject.SetActive(false);
            inGameDisconnectButton.gameObject.SetActive(false);
            forfitButton.gameObject.SetActive(false);
            //resetButton.gameObject.SetActive(false);
            openSettingsMenuButton.gameObject.SetActive(true);
            playDefaultStart.gameObject.SetActive(true);
            openNetworkMenuButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(true);
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
        GameManager.instance.onStoppingGame();
        hideMainMenu();
        showMainMenu();
    }

    #region Lobby

    public void showLobby()
    {
        isInLobby = true;
        LobbyCanvas.SetActive(true);
        hideNetworkMenu(false);
    }

    public void hideLobby()
    {
        LobbyCanvas.SetActive(false);
    }

    public void defaultStartLobby()
    {
        NetworkGameManager.instance.sendStandardFEN(playAsWhite.isOn);
    }

    public void usingFenLobby()
    {
        NetworkGameManager.instance.sendFEN(fenInputFeildLobby.text, playAsWhite.isOn);
    }

    public void checkFENLobby(InputField fenInput)
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
        inputErrTextLobby.text = "Check failed fail type: { " + valFenSections[1] + ", " + FENUtills.failTypeDict[valFenSections[1]] + " } \n Unable to start game.";

    }

    public void disconnectLobby()
    {
        if (isInLobby)
        {
            if (showing)
                hideMainMenu();
            NetworkUIManager.instance.Disconnect();
            isInLobby = false;
            hideLobby();
            showNetworkMenu();
            if (GameManager.instance.started)
            {
                gameManager.started = false;
                uiManager.hideBoard();
            }
        }
    }

    public void showSettingsMenuLobby()
    {
        hideLobby();
        settingsManager.showSettingsMenu();
    }
    #endregion
}
