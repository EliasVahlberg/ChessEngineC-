using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUIManager : MonoBehaviour
{
    public static NetworkUIManager instance;
    public GameObject startMenu;
    public InputField userNameField;
    public InputField ipField;
    public bool connectedTCP = false;
    public bool connectedUDP = false;
    // * SINGELTON IMPLEMENTATION
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
    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        userNameField.interactable = false;
        ipField.interactable = false;
        Client.instance.ConnectToServer();

    }
    public void ConnectToServerNewIp()
    {
        ConnectToServer(ipField.text);
    }
    public void ConnectToServer(string ip)
    {
        startMenu.SetActive(false);
        userNameField.interactable = false;
        ipField.interactable = false;
        Client.instance.ConnectToServer(ip);
        MenuManager.instance.hideNetworkMenu(false);
    }
    public void onDisconnect()
    {
        MenuManager.instance.showNetworkMenu();
    }
    public void SendMove()
    { }
    public void SendFen()
    { }

}
