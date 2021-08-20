using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/*
    @File SettingsManager.cs
    @author Elias Vahlberg 
    @Date 2021-07
*/
public class SettingsManager : MonoBehaviour
{


    #region MenuItems
    public Toggle fullscreenToggle;
    public Slider volumeSlider;
    public Toggle mute;
    public Slider cursorSlider;
    public Button backButton;
    public Button reloadSceneButton;
    public TabManager tabManager;
    public bool showing = false;

    [SerializeField]
    private Slider minutesSlider;
    [SerializeField]
    private TMP_Text minutesText;
    [SerializeField]
    private Slider secondsSlider;
    [SerializeField]
    private TMP_Text secondsText;


    #endregion
    [SerializeField]
    private Texture2D cursorTexture;
    private int cursorTextureOriginalHeight;
    private int cursorTextureOriginalWidth;
    public static SettingsManager instance;
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

        cursorTextureOriginalHeight = cursorTexture.height;
        cursorTextureOriginalWidth = cursorTexture.width;
        cursorSlider.onValueChanged.AddListener(changeCursorSize);
        volumeSlider.onValueChanged.AddListener(changeVolume);
        fullscreenToggle.onValueChanged.AddListener(changeFullscreen);
        backButton.onClick.AddListener(hideSettingsMenu);
        volumeSlider.SetValueWithoutNotify(AudioListener.volume);
        fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
        tabManager.Deactivate();

        minutesText.text = GameManager.instance.TimeLimitMinutes.ToString("00");
        secondsText.text = GameManager.instance.TimeLimitSeconds.ToString("00");
        minutesSlider.SetValueWithoutNotify(GameManager.instance.TimeLimitMinutes);
        secondsSlider.SetValueWithoutNotify(GameManager.instance.TimeLimitSeconds);
        minutesSlider.onValueChanged.AddListener(setTimeLimitMinutes);
        secondsSlider.onValueChanged.AddListener(setTimeLimitSeconds);
    }

    private void changeVolume(float value)
    {
        PlayerPrefs.SetFloat("volume", value);
        AudioListener.volume = PlayerPrefs.GetFloat("volume", 0.5f);
        volumeSlider.SetValueWithoutNotify(value);
    }
    private bool muted = false;
    private float prev = 0;
    public void Mute()
    {

        if (!muted)
        {
            prev = volumeSlider.value;
            PlayerPrefs.SetFloat("volume", 0);
            AudioListener.volume = PlayerPrefs.GetFloat("volume", 0.5f);
            volumeSlider.SetValueWithoutNotify(0);
            muted = true;
        }
        else
        {
            PlayerPrefs.SetFloat("volume", prev);
            AudioListener.volume = PlayerPrefs.GetFloat("volume", 0.5f);
            volumeSlider.SetValueWithoutNotify(prev);
            muted = false;
        }
    }

    private void changeFullscreen(bool value)
    {
        PlayerPrefs.SetInt("fullscreen", value ? 1 : 0);
        Screen.fullScreen = PlayerPrefs.GetInt("fullscreen", 0) == 1;
        fullscreenToggle.SetIsOnWithoutNotify(value);
    }

    public void showSettingsMenu()
    {
        if (!showing)
        {
            tabManager.Activate();
            tabManager.Show(tabManager.CurrentlyShowing);
            showing = true;
        }
    }

    public void hideSettingsMenu()
    {
        if (showing)
        {
            tabManager.Deactivate();
            if (MenuManager.instance.isInLobby)
                MenuManager.instance.showLobby();
            else
                MenuManager.instance.showMainMenu();
            showing = false;
        }
    }

    public void changeCursorSize(float val)
    {
        CursorMode mode = CursorMode.ForceSoftware;

        int height = (int)(cursorTextureOriginalHeight * val);
        int width = (int)(cursorTextureOriginalHeight * val);
        int xspot = width / 2;
        int yspot = height / 2;
        Vector2 hotSpot = new Vector2(xspot, yspot);
        Cursor.SetCursor(Resize(cursorTexture, width, height), hotSpot, mode);
    }

    private Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
    {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        //result.alphaIsTransparency = true;


        return result;
    }

    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void setTimeLimitMinutes(float value)
    {
        GameManager.instance.TimeLimitMinutes = ((int)value);
        minutesText.text = GameManager.instance.TimeLimitMinutes.ToString("00");
    }
    public void setTimeLimitSeconds(float value)
    {
        GameManager.instance.TimeLimitSeconds = ((int)value);
        secondsText.text = GameManager.instance.TimeLimitSeconds.ToString("00");
    }
}
