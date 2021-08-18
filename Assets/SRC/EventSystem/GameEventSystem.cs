using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
    #region Singelton
    public static GameEventSystem current;
    private void Awake()
    {
        if (current == null)
        {
            current = this;

        }
        else if (current != this)
        {
            Debug.Log("SAMEINSTACE ");
            Destroy(this);
        }
    }
    #endregion

    /*
    *Main Loop
    Request move from player->
    Recieve move from player->
    Update State ->
    if{UI MODE}( Update UI -> )
    if{DELAY}(Wait(delay))
    [REPEAT]
    
    On start
    On exit
    On Reset
    */

    public enum OnNextFrameAction
    {
        NONE,
        REQUEST,
        RECIEVE,
        RECIEVE_UI,
        OTHER
    }
    #region State

    [SerializeField]
    private bool Running = false;
    public bool UsingUI = true;
    public bool MoveRequested = false;
    public bool MoveRecieved = false;
    public bool MoveRecievedUI = false;
    #endregion
    #region Events
    public event Action<bool> onMoveRequest;
    public event Action<Move> onMoveRecieve;
    public event Action<Move> onMoveRecieveUI;
    #endregion

    //*When UI is used
    private OnNextFrameAction nextFrameAction = OnNextFrameAction.NONE;

    //*When no UI is used 
    //TODO IMPLEMENT
    public Action onNextTick;


    /*
    #####   GAME LOOP SUMMARIZED   #####

    ?   START -> MoveRequest -> onMoveRequest -> MoveRequestComplete
    !       <- [Clears callstack and lets one frame/tick pass]
    ?   onNextFrame/onNextTick -> MoveRecieve-> onMoveRecieve 
    *       .
    *       .
    *       .
    *   MoveRecieve-> onMoveRecieve->MoveRecieveComplete
    !       <- [Clears callstack and lets one frame/tick pass]
    *   
    ?   onNextFrame/onNextTick -> MoveRecieveUI-> onMoveRecieveUI 
    *       .
    *       .
    *       .
    *   MoveRecieveCompleteUI
    !       <- [Clears callstack and lets one frame/tick pass]
    ?   onNextFrame/onNextTick -> MoveRequest ...
    
    */
    public void StartGame() { StartGame(true); }
    public void StartGame(bool isWhite)
    {
        Running = true;
        isWhiteNextTurn = isWhite;
        nextFrameAction = OnNextFrameAction.REQUEST;

    }
    public void Stop()
    {
        Running = false;
        nextFrameAction = OnNextFrameAction.NONE;
    }
    private void Update()
    {
        if (nextFrameAction != OnNextFrameAction.NONE && Running)
        {

            switch (nextFrameAction)
            {
                case OnNextFrameAction.REQUEST:
                    nextFrameAction = OnNextFrameAction.NONE;
                    MoveRequest();
                    break;
                case OnNextFrameAction.RECIEVE:
                    nextFrameAction = OnNextFrameAction.NONE;
                    MoveRecieve();
                    break;
                case OnNextFrameAction.RECIEVE_UI:
                    nextFrameAction = OnNextFrameAction.NONE;
                    MoveRecieveUI();
                    break;
                default:
                    nextFrameAction = OnNextFrameAction.NONE;
                    Debug.LogError("UNKNOWN Next frame action");
                    break;
            }
        }
    }

    public void MoveRequest() { MoveRequest(isWhiteNextTurn); }
    public void MoveRequest(bool isWhite)
    {
        Debug.Log("RequestStart");
        if (onMoveRequest != null)
        {
            MoveRecievedUI = false;
            onMoveRequest(isWhite);
            MoveRequested = true;
        }
    }

    private Move requestedMove;
    public void MoveRequestComplete(Move requestedMove)
    {
        Debug.Log("RequestComplete");
        this.requestedMove = requestedMove;
        if (UsingUI)
        {
            nextFrameAction = OnNextFrameAction.RECIEVE;
        }
        else
            onNextTick = MoveRecieve;
    }

    private void MoveRecieve() { MoveRecieve(requestedMove); }
    public void MoveRecieve(Move move)
    {
        Debug.Log("RecieveStart");
        if (onMoveRecieve != null)
        {
            MoveRequested = false;
            onMoveRecieve(move);
            MoveRecieved = true;
        }
    }

    private Move processedMove;
    public void MoveRecieveComplete(Move processedMove)
    {

        this.processedMove = processedMove;
        if (UsingUI)
        {
            nextFrameAction = OnNextFrameAction.RECIEVE_UI;
        }
        else
            onNextTick = MoveRecieveUI;
    }

    private void MoveRecieveUI() { MoveRecieveUI(processedMove); }
    public void MoveRecieveUI(Move move)
    {
        try
        {
            Debug.Log("RecieveUISTART");
            if (onMoveRecieveUI != null)
            {
                MoveRecieved = false;
                onMoveRecieveUI(move);
                MoveRecievedUI = true;
            }
        }
        catch (System.Exception _ex)
        {
            Debug.LogError(_ex);
            throw;
        }
    }

    private bool isWhiteNextTurn;
    public void MoveRecieveUIComplete(bool isWhiteNextTurn)
    {
        Debug.Log("RecieveUIComplete");
        this.isWhiteNextTurn = isWhiteNextTurn;
        if (UsingUI)
        {
            nextFrameAction = OnNextFrameAction.REQUEST;
        }
        else
            onNextTick = MoveRequest;

    }


}
