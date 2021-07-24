
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
public class ChessNetClient : NetworkBehaviour
{
    public int clientType;
    [SerializeField] public int[] tiles;
    [SerializeField] public int[] move;


    public void submitMoveChangeToServer(int[] move)
    {
        if (!IsOwner) { return; }
        UpdateMoveServerRpc(move);
    }
    [ServerRpc]
    private void UpdateMoveServerRpc(int[] move)
    {
        UpdateMoveClientRpc(move);
    }
    [ClientRpc]
    private void UpdateMoveClientRpc(int[] move)
    {
        Debug.Log(move[0] + ", " + move[1]);

    }
}
