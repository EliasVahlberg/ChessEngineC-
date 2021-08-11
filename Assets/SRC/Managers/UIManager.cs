using System.Collections;
using System.Collections.Generic;
using ChessAI;
using Testing;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Constants

    static readonly char[] ranksIndecies = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
    #endregion

    #region publicVars
    [HideInInspector]
    public GameManager gameManager;
    [HideInInspector]
    public MenuManager menuManager;
    [HideInInspector]
    public Tile[] tiles;
    [HideInInspector]
    public PieceUI[] pieceUI = new PieceUI[64];
    public Stack<PieceUI> destroyedPieceUI = new Stack<PieceUI>();

    public GameObject tilePref;
    public GameObject boardContainer;
    public Vector2 startPos;
    public float squareSize = 1f;
    public float pieceImgScale = 1f;
    public string tileTag = "Tile";
    public string pieceTag = "Piece";
    public string menuTag = "Menu";
    public string pieceSortingLayer = "Piece";
    public Text gameText;
    public Text winText;
    public Color lightColor = Color.white;
    public Color darkColor = Color.black;
    public Color tintOffset = new Color(0, 100, 0);
    public Color dangerTintOffset = new Color(100, 0, 0);
    public Color lastMoveTintOffset = new Color(100, 0, 100);

    [Header("Promotion Menu")]
    public GameObject promotionMenu;
    public GameObject promotionMenuExit;
    public GameObject promotionMenuQueen;
    public GameObject promotionMenuKnight;
    public GameObject promotionMenuRook;
    public GameObject promotionMenuBishop;

    [Header("Background")]
    [SerializeField]
    private GameObject background1;
    [SerializeField]
    private GameObject background2;


    public Dictionary<int, int> pieceTypeToSprite = new Dictionary<int, int>()
    {
        [Piece.KING | Piece.WHITE] = 0,
        [Piece.PAWN | Piece.WHITE] = 1,
        [Piece.KNIGHT | Piece.WHITE] = 2,
        [Piece.BISHOP | Piece.WHITE] = 3,
        [Piece.ROOK | Piece.WHITE] = 4,
        [Piece.QUEEN | Piece.WHITE] = 5,
        [Piece.KING | Piece.BLACK] = 6,
        [Piece.PAWN | Piece.BLACK] = 7,
        [Piece.KNIGHT | Piece.BLACK] = 8,
        [Piece.BISHOP | Piece.BLACK] = 9,
        [Piece.ROOK | Piece.BLACK] = 10,
        [Piece.QUEEN | Piece.BLACK] = 11
    };
    #endregion

    #region Assets
    public Sprite[] piceSprites = new Sprite[12];
    public AudioClip movePieceSound1; //http://freesoundeffect.net/sound/game-piece-slide-1-sound-effect
    public AudioClip movePieceSound2; //http://freesoundeffect.net/sound/game-piece-slide-2-sound-effect
    public AudioClip destroyPieceSound1; //http://freesoundeffect.net/sound/game-piece-fall-1-sound-effect
    public AudioClip destroyPieceSound2; //http://freesoundeffect.net/sound/game-piece-fall-2-sound-effect
    public AudioClip OnStartSound; //https://freesound.org/people/jalastram/sounds/362227/
    public AudioClip OnUndoSound; //https://freesound.org/people/jalastram/sounds/348493/
    public AudioSource audioSource;
    private int internalAudioCounter1 = 0;
    private int internalAudioCounter2 = 0;
    #endregion

    #region StateVariables

    private int whitePointCounter = 0;
    public Text whitePointText;
    private int blackPointCounter = 0;
    public Text blackPointText;
    public GameObject pointsCanvas;
    private bool selectingPromotion = false;

    private List<int> tintedSquares;
    #endregion

    public static UIManager instance;

    #region INIT

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
        gameManager = GameManager.instance;
        tiles = new Tile[64];
        startPos = new Vector2(-squareSize * 4, -squareSize * 4);
        tintedSquares = new List<int>();
        generateBoardUI();
        menuManager = MenuManager.instance;
        background1.SetActive(true);
        background2.SetActive(false);
        audioSource.PlayOneShot(OnStartSound, 0.2f);
        audioSource.SetScheduledEndTime(AudioSettings.dspTime + (1f));

    }

    private void generateBoardUI()
    {
        for (int i = 0; i < 64; i++)
        {
            Vector2 pos = startPos + new Vector2((i % 8) * squareSize, (i / 8) * squareSize);
            GameObject obj = GameObject.Instantiate(tilePref, pos, Quaternion.identity, boardContainer.transform);
            obj.transform.localScale *= squareSize;
            obj.name = "Tile_" + ranksIndecies[i / 8] + (i % 8 + 1);
            obj.tag = tileTag;
            tiles[i] = obj.AddComponent<Tile>();
            tiles[i].uiManager = this;
            tiles[i].setPosition(i);
            //tiles[i].gameObject.SetActive(false);
        }
        boardContainer.SetActive(false);
    }

    public void generatePieceUI()
    {
        int[] pices = gameManager.board.tiles;
        int[] nameCounter = new int[12];
        boardContainer.SetActive(true);
        for (int i = 0; i < 64; i++)
        {
            if (pieceUI[i] != null)
            {
                pieceUI[i].Destroy();
                pieceUI[i] = null;
            }
            int type = gameManager.board.tiles[i];
            if (type != 0)
            {
                bool isWhite = Piece.IsWhite(type);
                Vector2 pos = startPos + new Vector2((i % 8) * squareSize, (i / 8) * squareSize);
                GameObject obj = GameObject.Instantiate(tilePref, pos, Quaternion.identity, boardContainer.transform);
                obj.transform.localScale *= pieceImgScale;
                obj.name = "Piece_"
                + (isWhite ?
                    char.ToUpper(FENUtills.typeSymbolDict[Piece.PieceType(type)]) :
                    FENUtills.typeSymbolDict[Piece.PieceType(type)])
                 + "_" + (nameCounter[pieceTypeToSprite[type]] + 1);
                nameCounter[pieceTypeToSprite[type]]++;
                obj.tag = pieceTag;
                pieceUI[i] = obj.AddComponent<PieceUI>();
                pieceUI[i].uiManager = this;
                pieceUI[i].setSprite(piceSprites[pieceTypeToSprite[type]]);
                pieceUI[i].piece = type;
                pieceUI[i].position = i;
            }
        }
    }
    #endregion

    void Update()
    {
        checkMouseInput();
        checkKeyInput();
    }

    #region InputHandeling

    [HideInInspector]
    public bool dragging = false;
    private PieceUI currPieceUi;
    private bool displaying = false;

    private void checkMouseInput()
    {

        if (menuManager.showing)
        {
            return;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, new Vector2(0, 0), 0.2f);
            if (hits.Length != 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    Debug.Log(hits[i].transform.name);
                    if (hits[i].transform.tag == menuTag)
                    {
                        Debug.Log(hits[i].transform.name);
                    }

                }
            }
        }
        else
        {
            if (selectingPromotion && Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, new Vector2(0, 0), 0.2f);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.tag == "Promotion")
                    {
                        GameObject obj = hits[i].collider.gameObject;
                        if (obj == promotionMenuExit)
                            onPromotionSelectExit(0);
                        else if (obj == promotionMenuQueen)
                            onPromotionSelectExit(Piece.QUEEN);
                        else if (obj == promotionMenuKnight)
                            onPromotionSelectExit(Piece.KNIGHT);
                        else if (obj == promotionMenuRook)
                            onPromotionSelectExit(Piece.ROOK);
                        else if (obj == promotionMenuBishop)
                            onPromotionSelectExit(Piece.BISHOP);

                    }
                }
            }
            else if (dragging && Input.GetMouseButtonUp(0))
            {
                dragging = false;
                hideAvailableMoves();
                gameManager.selectPosition();
                currPieceUi.refresh();
                currPieceUi = null;
            }
            else if (dragging)
            {
                Vector3 v3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                v3.z = 0;
                currPieceUi.transform.position = v3;
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, new Vector2(0, 0), 0.2f);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.tag == tileTag)
                    {
                        gameManager.selectedMoveTo = hits[i].collider.gameObject.GetComponent<Tile>().position;
                    }
                }

            }
            else if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, new Vector2(0, 0), 0.2f);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.tag == pieceTag)
                    {
                        dragging = true;
                        currPieceUi = hits[i].collider.gameObject.GetComponent<PieceUI>();
                        gameManager.selectedPiece = currPieceUi.position;
                        showAvailableMoves(currPieceUi.position);

                    }
                }
            }
            else
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, new Vector2(0, 0), 0.2f);
                bool overSquare = false;
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.tag == pieceTag)
                    {
                        if (currPieceUi != hits[i].collider.gameObject.GetComponent<PieceUI>())
                        {
                            currPieceUi = hits[i].collider.gameObject.GetComponent<PieceUI>();
                            showAvailableMoves(currPieceUi.position);
                        }
                        overSquare = true;
                    }

                }
                if (!overSquare)
                {
                    hideAvailableMoves();
                    currPieceUi = null;
                }
            }
        }


    }

    private bool dangerTintedW = false;
    private List<int> dangerTintedListW = new List<int>();
    private bool dangerTintedB = false;
    private List<int> dangerTintedListB = new List<int>();
    private bool isFlipped = false;
    private void checkKeyInput()
    {
        if (gameManager.ended)
        {
            if (Input.GetKeyDown(KeyCode.R))
                gameManager.onResetGame();
            if (Input.GetKeyDown(KeyCode.Escape))
            { gameManager.onStoppingGame(); menuManager.showMainMenu(); }

        }
        if (DevConsoleBehaviour.instance.active)
        {
            if (Input.GetKeyDown(KeyCode.F1))
                DevConsoleBehaviour.instance.ToggleDevConsole();
            return;
        }
        if (Input.inputString == "+")
        {
            if (TestingMenu.instance.showing)
                TestingMenu.instance.Deactivate();
            else
                TestingMenu.instance.Activate();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            showDanger(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            showDanger(false);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            flipCamera();
        }


        if (Input.GetKeyDown(KeyCode.Escape) && !gameManager.ended)
        {
            if (menuManager.showing)
                menuManager.hideMainMenu();
            else
                menuManager.showMainMenu();
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            DevConsoleBehaviour.instance.ToggleDevConsole();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (gameManager.started && !gameManager.isNetworked)
            {
                gameManager.UnmakeMove();
            }
        }
    }

    private int promotionStart;
    private int promotionTarget;
    public void onPromotionSelectEnter(Move move)
    {
        selectingPromotion = true;
        promotionMenu.SetActive(true);
        promotionMenu.transform.position = pieceUI[move.StartSquare].transform.position;
        promotionStart = move.StartSquare;
        promotionTarget = move.TargetSquare;

    }
    public void onPromotionSelectExit(int option)
    {
        selectingPromotion = false;
        promotionMenu.SetActive(false);
        GameManager.instance.selectPromotionMove(option, promotionStart, promotionTarget);
    }
    #endregion

    #region ColorTiles
    public void showDanger(bool white)
    {
        if (!gameManager.started)
            return;




        bool dangerTinted = white ? dangerTintedW : dangerTintedB;
        List<int> dangerTintedList = white ? dangerTintedListW : dangerTintedListB;
        if (dangerTinted)
        {
            foreach (int n in dangerTintedList)
            {
                tiles[n].revertDangerTint();
            }
            if (white)
            {
                dangerTintedW = false;
                dangerTintedListW.Clear();
            }
            else
            {
                dangerTintedB = false;
                dangerTintedListB.Clear();
            }
        }
        else
        {

            if (!GameManager.instance.board.HasGeneratedMoves ||
                (GameManager.instance.board.whiteTurn != white))
                return;
            int i = 0;
            foreach (bool b in ((white) ? gameManager.board.BlackCap : gameManager.board.WhiteCap))
            {
                if (b)
                {
                    tiles[i].dangerTint();
                    dangerTintedList.Add(i);
                }
                i++;
            }
            if (white)
            {
                dangerTintedW = true;
                dangerTintedListW = dangerTintedList;
            }
            else
            {
                dangerTintedB = true;
                dangerTintedList = dangerTintedListB;
            }
        }

    }

    public void hideDanger()
    {
        bool dangerTinted = dangerTintedW || dangerTintedB;
        if (dangerTinted)
        {
            if (dangerTintedW)
            {

                foreach (int n in dangerTintedListW)
                {
                    tiles[n].revertDangerTint();
                }
                dangerTintedW = false;
                dangerTintedListW.Clear();
            }
            if (dangerTintedB)
            {

                foreach (int n in dangerTintedListB)
                {
                    tiles[n].revertDangerTint();
                }

                dangerTintedB = false;
                dangerTintedListB.Clear();
            }
        }
    }

    public int lastMoveStart = -1;
    public int lastMoveTarget = -1;
    public void LastMoveTint(int from, int to)
    {
        if (lastMoveStart != -1 || lastMoveTarget != -1)
        {
            tiles[lastMoveStart].revertLastMoveTint();
            tiles[lastMoveTarget].revertLastMoveTint();
        }
        lastMoveStart = from;
        lastMoveTarget = to;
        tiles[lastMoveStart].lastMoveTint();
        tiles[lastMoveTarget].lastMoveTint();
    }

    #endregion

    #region BoardView
    public void flipCamera()
    {
        if (isFlipped)
        {
            Camera.main.transform.rotation = new Quaternion(0, 0, 0, 0);
            Camera.main.transform.position -= new Vector3(-1, -1, 0);
            for (int i = 0; i < 64; i++)
                if (pieceUI[i] != null)
                    pieceUI[i].transform.rotation = new Quaternion(0, 0, 0, 0);
            isFlipped = false;
        }
        else
        {
            Camera.main.transform.rotation = new Quaternion(0, 0, 180, 0);
            Camera.main.transform.position += new Vector3(-1, -1, 0);
            for (int i = 0; i < 64; i++)
                if (pieceUI[i] != null)
                    pieceUI[i].transform.rotation = new Quaternion(0, 0, 180, 0);
            isFlipped = true;
        }
    }

    public void showAvailableMoves(int pos)
    {
        if (!gameManager.started)
            return;
        if (tintedSquares.Count != 0)
        {
            foreach (int target in tintedSquares)
            {
                tiles[target].revertTint();
            }
            tintedSquares.Clear();
        }
        //Maby prevent nullpointer

        List<Move> moveL = gameManager.board.MoveMap[pos];
        if (moveL == null)
            return;
        foreach (Move move in moveL)
        {
            int target = move.TargetSquare;
            tiles[target].tint();
            tintedSquares.Add(target);
        }
    }

    public void hideAvailableMoves()
    {
        if (!gameManager.started)
            return;
        if (tintedSquares.Count != 0)
            foreach (int target in tintedSquares)
            {
                tiles[target].revertTint();
            }
    }

    public void hideBoard()
    {
        if (!gameManager.started)
        {
            boardContainer.SetActive(false);
            foreach (PieceUI pUI in pieceUI)
                if (pUI != null)
                    pUI.Destroy();
        }
    }
    #endregion

    #region BoardUpdate

    public void movePiece(int from, int to)
    {
        if (pieceUI[to] != null)
        {
            destroyPiece(to);
        }
        else
            playMovePieceSound();
        pieceUI[to] = pieceUI[from];
        pieceUI[from] = null;
        pieceUI[to].position = to;
        pieceUI[to].refresh();
    }

    public void destroyPiece(int pos)
    {
        if (pieceUI[pos] != null)
        {
            destroyedPieceUI.Push(pieceUI[pos]);
            pieceUI[pos].gameObject.SetActive(false);
            pieceUI[pos] = null;
            playDestroyPieceSound();

        }
    }

    public void reinstatePiece(int pos)
    {
        if (destroyedPieceUI.Count != 0)
        {
            Debug.Log("REINSTATED");
            PieceUI obj = destroyedPieceUI.Pop();
            obj.gameObject.SetActive(true);
            pieceUI[pos] = obj;
            playDestroyPieceSound();

        }
    }

    #endregion

    #region Sound

    public void playMovePieceSound()
    {
        audioSource.clip = (internalAudioCounter1++) % 2 == 0 ? movePieceSound1 : movePieceSound2;
        audioSource.Play();
    }

    public void playDestroyPieceSound()
    {
        audioSource.clip = (internalAudioCounter2++) % 2 == 0 ? destroyPieceSound2 : destroyPieceSound2;
        audioSource.Play();
    }

    public void playUndoSound()
    {
        internalAudioCounter2--;
        audioSource.clip = OnUndoSound;
        audioSource.Play();
    }

    #endregion

    #region Score

    public void updateScore(int value, bool isWhite)
    {
        if (isWhite)
        {
            whitePointCounter += value;
            whitePointText.text = "White: " + whitePointCounter;
        }
        else
        {
            blackPointCounter += value;
            blackPointText.text = "Black: " + blackPointCounter;
        }
    }

    public void HideScore()
    {
        pointsCanvas.SetActive(false);
    }

    public void ShowScore()
    {
        pointsCanvas.SetActive(true);
    }

    public void ResetScore()
    {
        whitePointCounter = 0;
        blackPointCounter = 0;
    }

    #endregion

    #region IngameUIToggle

    public void ShowInGameUI()
    {
        //GameHistory
        GameHistoryPanel.instance.activate();
        //GameText/Score
        ShowScore();
        //AI menu
        Debug.Log("Started");
        AIManager.instance.showAIMenu();
        background1.SetActive(false);
        background2.SetActive(true);
        //
    }

    public void ResetInGameUI()
    {
        //GameText/Score
        ResetScore();
        ShowScore();
        //WIN Screen
        winText.text = "";
        //ResetAI
        GameManager.instance.ResetAI();
        AIManager.instance.showAIMenu();
        //GameHistory
        GameHistoryPanel.instance.resetHistory();
        GameHistoryPanel.instance.activate();
        if (lastMoveStart != -1 && lastMoveTarget != -1)
        {
            tiles[lastMoveStart].revertLastMoveTint();
            tiles[lastMoveTarget].revertLastMoveTint();
        }
    }

    public void HideInGameUI()
    {
        GameHistoryPanel.instance.deactivate();
        AIManager.instance.hideAIMenu();
        HideScore();
        winText.text = "";
        Debug.Log("Stopped");
        background1.SetActive(true);
        background2.SetActive(false);
        if (lastMoveStart != -1 && lastMoveTarget != -1)
        {
            tiles[lastMoveStart].revertLastMoveTint();
            tiles[lastMoveTarget].revertLastMoveTint();
        }
        hideBoard();
    }

    #endregion
}
