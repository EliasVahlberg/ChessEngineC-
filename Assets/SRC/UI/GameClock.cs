using UnityEngine;
using TMPro;

public class GameClock : MonoBehaviour
{
    [SerializeField]
    private TMP_Text whiteTimeText;
    [SerializeField]
    private TMP_Text blackTimeText;
    private int startingTimeSeconds = 1;
    private int startingTimeMinutes = 1;
    private float whiteTime;
    private float blackTime;
    private bool whiteTurn = true;
    private bool isActive = false;
    private bool isInitialized = false;
    private void Start()
    {
        GameEventSystem.current.onMoveRequest += switchTimer;
    }
    public void startClock(bool isWhiteFirst, int startingTimeSeconds, int startingTimeMinutes)
    {
        setStartTime(startingTimeSeconds, startingTimeMinutes);
        startClock(isWhiteFirst);
    }
    public void startClock(bool isWhiteFirst)
    {

        blackTime = whiteTime = this.startingTimeSeconds + this.startingTimeMinutes * 60;
        whiteTurn = isWhiteFirst;
        string timeSTR = "";
        timeSTR = ((int)(whiteTime / 60)).ToString("00") + ":" + ((int)(whiteTime % 60)).ToString("00");
        whiteTimeText.text = timeSTR;
        timeSTR = ((int)(blackTime / 60)).ToString("00") + ":" + ((int)(blackTime % 60)).ToString("00");
        blackTimeText.text = timeSTR;
        isInitialized = true;
    }
    public void reset(bool isWhiteFirst)
    {
        blackTime = whiteTime = this.startingTimeSeconds + this.startingTimeMinutes * 60;
        whiteTurn = isWhiteFirst;
        isActive = true;
    }
    public void setStartTime(int startingTimeSeconds, int startingTimeMinutes)
    {
        this.startingTimeSeconds = startingTimeSeconds;
        this.startingTimeMinutes = startingTimeMinutes;
    }

    public void switchTimer(bool isWhiteTurn)
    {
        if (!isInitialized)
        {
            startClock(isWhiteTurn, GameManager.instance.TimeLimitSeconds, GameManager.instance.TimeLimitMinutes);
            Debug.Log("INIT");
            return;
        }
        if (!isActive)
        {
            Debug.Log("START");
            isActive = true;
        }
        whiteTurn = isWhiteTurn;
    }

    public void stop()
    {
        isActive = false;
    }
    private void Update()
    {
        if (isActive)
        {
            updateTimer();
        }
    }
    private void updateTimer()
    {
        string timeSTR = "";
        if (whiteTurn)
        {
            whiteTime -= 1 * Time.deltaTime;
            if (whiteTime <= 0)
            {
                timeSTR = "00:00";
                GameManager.instance.playerTimeIsUp();
                isInitialized = false;
                isActive = false;
            }
            else
                timeSTR = ((int)(whiteTime / 60)).ToString("00") + ":" + ((int)(whiteTime % 60)).ToString("00");
            whiteTimeText.text = timeSTR;
        }
        else
        {
            blackTime -= 1 * Time.deltaTime;
            if (blackTime <= 0)
            {
                timeSTR = "00:00";
                GameManager.instance.playerTimeIsUp();
                isInitialized = false;
                isActive = false;
            }
            else
                timeSTR = ((int)(blackTime / 60)).ToString("00") + ":" + ((int)(blackTime % 60)).ToString("00");
            blackTimeText.text = timeSTR;
        }

    }

}