using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class NetworkHost : MonoBehaviour
{
    public NetworkManager networkManager;
    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();

    }

    void Update()
    {

    }
}
