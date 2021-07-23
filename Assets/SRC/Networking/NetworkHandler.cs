using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class NetworkHandler : MonoBehaviour
{
    void Start()
    {
        if (NetworkManager.Singleton.IsClient)
        {

            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
            {
                var networkClient = networkedClient.PlayerObject.GetComponent<NetworkClient>();
                if (networkClient)
                    return;
                //Debug.Log(networkClient.getMessage());
            }
        }
    }

    void Update()
    {

    }
}
