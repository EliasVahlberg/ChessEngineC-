using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleHistory : MonoBehaviour
{
    public static ConsoleHistory instance; //* SINGELTON
    public string consoleHistoryTag = "ConsoleHistoryScroll";
    public TextMeshProUGUI logHistoryText;
    private ScrollRect historyScrollRect = null;
    public ScrollRect HistoryScrollRect
    {
        get
        {
            if (historyScrollRect != null)
                return historyScrollRect;
            return historyScrollRect = GameObject.FindGameObjectWithTag(consoleHistoryTag).GetComponent<ScrollRect>();
        }
    }
    private DevConsoleBehaviour devConsoleBehaviour;
    public DevConsoleBehaviour DConsoleBehaviour
    {
        get
        {
            if (devConsoleBehaviour != null)
                return devConsoleBehaviour;
            return devConsoleBehaviour = FindObjectOfType<DevConsoleBehaviour>();
        }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void addLogHistory(string textMes)
    {
        try
        {

            if (logHistoryText.preferredHeight + 50 > logHistoryText.rectTransform.sizeDelta.y)
            {
                logHistoryText.rectTransform.sizeDelta += new Vector2(0, 50);
                Vector3 v3 = logHistoryText.rectTransform.localPosition;
                v3.y += 25f;
                logHistoryText.rectTransform.localPosition = v3;
            }
            logHistoryText.text += "\n" + textMes;

        }
        catch (System.Exception _ex)
        {

            Debug.Log(_ex);
            throw;
        }
    }
}
