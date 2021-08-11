using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{


    #region MenuItems
    public Toggle fullscreenToggle;
    public Slider volumeSlider;
    public Slider cursorSlider;
    public Button backButton;
    public Button reloadSceneButton;
    public GameObject canvas;
    public bool showing = false;


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
        canvas.SetActive(false);
    }
    private void changeVolume(float value)
    {
        PlayerPrefs.SetFloat("volume", value);
        AudioListener.volume = PlayerPrefs.GetFloat("volume", 0.5f);
        volumeSlider.SetValueWithoutNotify(value);
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
            canvas.SetActive(true);
            showing = true;
        }
    }
    public void hideSettingsMenu()
    {
        if (showing)
        {
            canvas.SetActive(false);
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
}
