using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class NetChangeCol : NetworkBehaviour
{
    [SerializeField] private GameObject squarePrefab;
    private void Update()
    {
        if (!IsOwner || !Input.GetKeyDown(KeyCode.Space)) { return; }
        ChangeColorServerRpc();
    }
    [ServerRpc]
    private void ChangeColorServerRpc()
    {
        ChangeColorClientRpc();
    }
    [ClientRpc]
    private void ChangeColorClientRpc()
    {
        SpriteRenderer sr = squarePrefab.GetComponent<SpriteRenderer>();
        sr.color = new Color(((int)sr.color.r + 100) % 255, 0, 0);
    }


    [ServerRpc]
    private void TakeDataServerRpc(int data)
    {

    }
}
