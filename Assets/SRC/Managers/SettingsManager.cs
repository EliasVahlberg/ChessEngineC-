using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [HideInInspector]
    public UIManager uiManager;
    [HideInInspector]
    public GameManager gameManager;
    [HideInInspector]
    public MenuManager menuManager;

    #region MenuItems
    public Toggle fullscreenToggle;
    public Slider volumeSlider;
    public Button backButton;
    public GameObject canvas;
    public bool showing = false;

    #endregion
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
        uiManager = UIManager.instance;
        gameManager = GameManager.instance;
        menuManager = MenuManager.instance;
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
            if (menuManager.isInLobby)
                menuManager.showLobby();
            else
                menuManager.showMainMenu();
            showing = false;
        }
    }
}
