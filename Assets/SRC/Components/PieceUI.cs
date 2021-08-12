using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    @File PieceUI.cs
    @author Elias Vahlberg 
    @Date 2021-07
*/
public class PieceUI : MonoBehaviour
{
    public int piece;
    public int position;
    public Sprite sprite;
    public SpriteRenderer spriteRen;
    public BoxCollider2D collider;
    public UIManager uiManager;
    public void setSprite(Sprite sprite)
    {
        spriteRen = gameObject.GetComponent<SpriteRenderer>();
        this.sprite = sprite;
        spriteRen.sprite = sprite;
        spriteRen.sortingLayerName = uiManager.pieceSortingLayer;
        spriteRen.sortingOrder = 1;
        gameObject.layer = 1;
        collider = GetComponent<BoxCollider2D>();
        collider.tag = gameObject.tag;
        collider.size = new Vector2(3.3f, 3.3f);
    }
    public void refresh()
    {
        transform.position = new Vector3(
            (Piece.File(position) * uiManager.squareSize - 4 * uiManager.squareSize),
            (Piece.Rank(position) * uiManager.squareSize - 4 * uiManager.squareSize),
            0);
    }
    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }

}
