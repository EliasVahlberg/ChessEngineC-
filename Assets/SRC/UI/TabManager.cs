using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    [SerializeField]
    private GameObject tabsParent;
    [SerializeField]
    private GameObject panelsParent;

    [SerializeField]
    private Button[] tabToggles;
    [SerializeField]
    private GameObject[] tabPanels;
    private Tab[] tabs;
    private int currentlyShowing = 0;
    public bool Showing { get => tabsParent.activeSelf && panelsParent.activeSelf; }
    public int CurrentlyShowing { get => currentlyShowing; }

    void Start()
    {
        tabToggles = tabsParent.GetComponentsInChildren<Button>();
        tabPanels = new GameObject[panelsParent.transform.childCount];

        int jj = 0;
        foreach (Transform child in panelsParent.transform)
        {
            tabPanels[jj] = child.gameObject;
            child.gameObject.SetActive(false);
            jj++;
        }
        tabs = new Tab[Mathf.Min(tabToggles.Length, tabPanels.Length)];
        for (int ii = 0; ii < tabs.Length; ii++)
        {
            Button toggle = tabToggles[ii];

            tabs[ii] = new Tab(tabToggles[ii], tabPanels[ii], ii, this);
        }

    }
    public void Show(int n)
    {

        Debug.Log(n);
        if (tabs.Length > n)
        {
            if (tabs[currentlyShowing].IsShowing)
                tabs[currentlyShowing].hide();
            tabs[n].show();
            currentlyShowing = n;
        }
    }
    public void Activate()
    {
        tabsParent.SetActive(true);
        panelsParent.SetActive(true);
    }
    public void Deactivate()
    {
        tabsParent.SetActive(false);
        panelsParent.SetActive(false);
    }
}
public class Tab
{
    public static readonly Color ColorOffset = new Color(0.5f, 0.5f, 0.5f);
    private Button toggle;
    private GameObject panel;
    private TabManager manager;
    private int tabNumber;

    public bool IsShowing { get => Panel.activeSelf; }
    public Button Toggle { get => toggle; set => toggle = value; }
    public GameObject Panel { get => panel; set => panel = value; }

    public Tab(Button toggle, GameObject panel, int tabNumber, TabManager manager)
    {
        this.Toggle = toggle;
        this.Panel = panel;
        this.tabNumber = tabNumber;
        this.manager = manager;
        Toggle.onClick.AddListener(() => manager.Show(this.tabNumber));
    }
    public void show()
    {
        if (IsShowing)
            return;
        panel.SetActive(true);
        toggle.image.color += ColorOffset;
    }
    public void hide()
    {

        if (!IsShowing)
            return;
        panel.SetActive(false);
        toggle.image.color -= ColorOffset;
    }

}
