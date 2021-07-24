using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [HideInInspector]
    private UIManager uiManager;
    [HideInInspector]
    private GameManager gameManager;
    [HideInInspector]
    private SettingsManager settingsManager;
    public Button playDefaultStart, playusingFen, resetButton, openNetworkMenuButton, quitButton, forfitButton, openSettingsMenuButton;
    public Text inputText;
    public InputField fenInputFeild;
    public Text inputErrText;

    public Button startClientButton, startHostButton, startServerButton, closeNetworkMenuButton;

    public Text networkStatusText;

    public GameObject canvas;
    public GameObject networkMenuCanvas;
    public bool showing = true;
    public bool showingNetworkMenu = false;
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

        startClientButton.onClick.AddListener(startClient);
        startHostButton.onClick.AddListener(startHost);
        startServerButton.onClick.AddListener(startServer);
        closeNetworkMenuButton.onClick.AddListener(hideNetworkMenu);
        openSettingsMenuButton.onClick.AddListener(showSettingsMenu);


        resetButton.gameObject.SetActive(false);
        forfitButton.gameObject.SetActive(false);
        networkMenuCanvas.SetActive(false);
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
        if (true)// (gameManager.started)
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
            hideMainMenu();
            showingNetworkMenu = true;
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
    public void startClient()
    {
        NetworkManager.Singleton.StartClient();
        StatusLabels();
        gameManager.isNetworked = true;
        gameManager.networkEntityType = GameManager.CLIENT;
        hideNetworkMenu(false);
        gameManager.startNetwork();
    }
    public void startHost()
    {
        NetworkManager.Singleton.StartHost();
        StatusLabels();
        gameManager.isNetworked = true;
        gameManager.networkEntityType = GameManager.HOST;
        hideNetworkMenu(false);
        gameManager.startNetwork();
    }
    public void startServer()
    {
        NetworkManager.Singleton.StartServer();
        StatusLabels();
        gameManager.isNetworked = true;
        gameManager.networkEntityType = GameManager.SERVER;
        hideNetworkMenu(false);
        gameManager.startNetwork();
    }
    public void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        networkStatusText.text = ("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name + "\n" + mode);
        foreach (var v in NetworkManager.Singleton.ConnectedClients.Keys)
            networkStatusText.text += v.ToString() + "\n";
        foreach (var v in NetworkManager.Singleton.ConnectedClients.Values)
            networkStatusText.text += v.ClientId.ToString() + "\n";

    }
    public void showSettingsMenu()
    {
        if (!settingsManager.showing)
        {
            settingsManager.showSettingsMenu();
            hideMainMenu();
        }
    }

}
