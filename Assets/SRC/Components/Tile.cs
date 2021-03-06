using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    @File Tile.cs
    @author Elias Vahlberg 
    @Date 2021-07
*/
public class Tile : MonoBehaviour
{
    public int position;
    public SpriteRenderer spriteRen;
    public UIManager uiManager;
    public BoxCollider2D collider;
    bool tinted = false;
    bool dangerTinted = false;
    bool lastMoveTinted = false;
    void Start()
    {
        spriteRen = gameObject.GetComponent<SpriteRenderer>();
        collider = gameObject.GetComponent<BoxCollider2D>();
        uiManager = FindObjectOfType<UIManager>();

    }

    public void setPosition(int pos)
    {
        if (spriteRen == null)
            spriteRen = gameObject.GetComponent<SpriteRenderer>();
        position = pos;
        spriteRen.color = ((position % 8 + position / 8) % 2 == 0) ? uiManager.darkColor : uiManager.lightColor;
        spriteRen.sortingLayerName = uiManager.pieceSortingLayer;
        collider = gameObject.GetComponent<BoxCollider2D>();
        collider.tag = gameObject.tag;
    }
    public void tint()
    {
        if (!tinted)
        {
            spriteRen.color += uiManager.tintOffset;
            tinted = true;
        }
    }
    public void revertTint()
    {
        if (tinted)
        {
            spriteRen.color -= uiManager.tintOffset;
            tinted = false;
        }
    }
    public void dangerTint()
    {
        if (!dangerTinted)
        {
            spriteRen.color += uiManager.dangerTintOffset;
            dangerTinted = true;
        }
    }
    public void revertDangerTint()
    {
        if (dangerTinted)
        {
            spriteRen.color -= uiManager.dangerTintOffset;
            dangerTinted = false;
        }
    }
    public void lastMoveTint()
    {
        if (!lastMoveTinted)
        {
            spriteRen.color += uiManager.lastMoveTintOffset;
            lastMoveTinted = true;
        }
    }
    public void revertLastMoveTint()
    {
        if (lastMoveTinted)
        {
            spriteRen.color -= uiManager.lastMoveTintOffset;
            lastMoveTinted = false;
        }
    }
}

