using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHistoryPanel : MonoBehaviour
{
    public GameObject canvas;
    public GameObject content;
    public GameObject historyItemPrefab;
    public bool showing = false;
    public static GameHistoryPanel instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            instance.gameObject.SetActive(false);
        }
        else if (instance != this)
        {
            Debug.Log("SAMEINSTACE ");
            Destroy(this);
        }
    }
    public void activate()
    {
        gameObject.SetActive(true);
        showing = true;
    }
    public void deactivate()
    {
        gameObject.SetActive(false);
        showing = false;
    }
    public void addHistoryItem(string fen, Move move, bool wasWhite, bool wasCapture)
    {
        HistoryItem item = Instantiate(historyItemPrefab, Vector3.zero, Quaternion.identity, content.transform).GetComponent<HistoryItem>();
        string msg = (wasCapture ? "<color=red><b>" : "<b>") + (wasWhite ? "White" : "Black") + " Move:" +
            Piece.PositionRepresentation[move.StartSquare] + " -> " +
            Piece.PositionRepresentation[move.TargetSquare] +
            (wasCapture ? "</b></color>" : "</b>");
        int piece = GameManager.instance.board.tiles[move.TargetSquare];
        Sprite moved = UIManager.instance.piceSprites[UIManager.instance.pieceTypeToSprite[piece]];
        if (wasCapture)
        {
            Sprite captured = UIManager.instance.piceSprites[UIManager.instance.pieceTypeToSprite[GameManager.instance.board.lastMoveCaptured]];
            item.init(msg, fen, moved, captured);
        }
        else
            item.init(msg, fen, moved);

    }

}
