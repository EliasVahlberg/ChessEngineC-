using System.Threading;
using UnityEngine;

public class EventListenerExample : MonoBehaviour
{
    public Move move;
    private bool isWhite;

    private void Start()
    {
        GameEventSystem.current.onMoveRequest += getMove;
        GameEventSystem.current.onMoveRecieve += setMove;
        GameEventSystem.current.onMoveRecieveUI += setMoveUI;
    }
    private void getMove(bool isWhite)
    {
        this.isWhite = isWhite;
        Debug.Log("Move request");
        Thread.Sleep(500);
        GameEventSystem.current.MoveRequestComplete(new Move(1));
    }
    private void setMove(Move move)
    {
        Debug.Log("Move recived : " + move.MoveValue);
        Thread.Sleep(500);
        GameEventSystem.current.MoveRecieveComplete(new Move(1));
    }
    private void setMoveUI(Move move)
    {
        Debug.Log("Move recived UI: " + move.MoveValue);
        Thread.Sleep(500);
        GameEventSystem.current.MoveRecieveUIComplete(!isWhite);
    }

}