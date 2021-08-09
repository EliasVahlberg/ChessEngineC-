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
    private Stack<GameObject> historyItemStack;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            canvas.gameObject.SetActive(false);
            historyItemStack = new Stack<GameObject>();
        }
        else if (instance != this)
        {
            Debug.Log("SAMEINSTACE ");
            Destroy(this);
        }
    }
    public void activate()
    {
        canvas.SetActive(true);
        showing = true;
    }
    public void deactivate()
    {
        canvas.SetActive(false);
        showing = false;
    }
    public void addHistoryItem(string fen, Move move, bool wasWhite, bool wasCapture)
    {
        HistoryItem item = Instantiate(historyItemPrefab, Vector3.zero, Quaternion.identity, content.transform).GetComponent<HistoryItem>();
        bool wasEP = GameManager.instance.board.lastMove.moveFlag == Move.Flag.EnPassantCapture;
        string msg = (wasCapture ? "<color=red><b>" : "<b>") + (wasWhite ? "White" : "Black") + " Move:" +
            (wasEP ? "(EP)" : "") +
            Piece.PositionRepresentation[move.StartSquare] + " -> " +
            Piece.PositionRepresentation[move.TargetSquare] +
            (wasCapture ? "</b></color>" : "</b>");
        if (move.moveFlag == Move.Flag.Castling)
        {
            int p = wasWhite ? Piece.KING | Piece.WHITE : Piece.KING | Piece.BLACK;
            Sprite king = UIManager.instance.piceSprites[UIManager.instance.pieceTypeToSprite[p]];
            string str = (wasWhite ? "W" : "B") + "Cas" + (move.StartSquare > move.TargetSquare ? "Q" : "K");
            item.init(str, fen, king);
            return;
        }
        int piece = GameManager.instance.board.tiles[move.TargetSquare];
        Sprite moved = UIManager.instance.piceSprites[UIManager.instance.pieceTypeToSprite[piece]];
        if (wasEP)
        {
            Sprite captured = UIManager.instance.piceSprites[UIManager.instance.pieceTypeToSprite[Piece.PAWN | (wasWhite ? Piece.BLACK : Piece.WHITE)]];
            item.init(msg, fen, moved, captured);
        }
        else if (wasCapture)
        {
            Sprite captured = UIManager.instance.piceSprites[UIManager.instance.pieceTypeToSprite[GameManager.instance.board.lastMoveCaptured | (wasWhite ? Piece.BLACK : Piece.WHITE)]];
            item.init(msg, fen, moved, captured);
        }
        else
            item.init(msg, fen, moved);
        historyItemStack.Push(item.gameObject);
    }
    public void removeLast()
    {
        if (historyItemStack.Count != 0)
            GameObject.Destroy(historyItemStack.Pop());
    }
    public void resetHistory()
    {
        while (historyItemStack.Count != 0)
            GameObject.Destroy(historyItemStack.Pop());
    }

}
