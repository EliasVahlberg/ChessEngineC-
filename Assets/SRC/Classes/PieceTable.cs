using UnityEngine;

/*
@File PieceTable.cs
@author Elias Vahlberg 
@Date 2021-07
@Credit Sebastian Lague 
*/
public class PieceTable
{

    // Indices of squares occupied by given piece type (only elements up to Count are valid, the rest are unused/garbage)
    public int[] occupiedSquares;
    // Map [index of a square]->[index in occupiedSquares] (where that square is stored)
    int[] map;
    int numPieces;
    public int Count
    {
        get
        {
            return numPieces;
        }
    }
    public int this[int index] => occupiedSquares[index];
    public PieceTable(int maxPieceCount = 16)
    {
        occupiedSquares = new int[maxPieceCount];
        map = new int[64];
        numPieces = 0;
    }
    public void AddPieceAtSquare(int square)
    {
        occupiedSquares[numPieces] = square;
        map[square] = numPieces;
        numPieces++;
    }
    public void RemovePieceAtSquare(int square)
    {
        try
        {

            int pieceIndex = map[square]; // get the index of this element in the occupiedSquares array
            occupiedSquares[pieceIndex] = occupiedSquares[numPieces - 1]; // move last element in array to the place of the removed element
            map[occupiedSquares[pieceIndex]] = pieceIndex; // update map to point to the moved element's new location in the array
            numPieces--;
        }
        catch (System.Exception ex_)
        {
            Debug.Log("Square =" + square + " map= " + map[square]);
            throw ex_;
        }
    }
    public void MovePiece(int startSquare, int targetSquare)
    {
        int pieceIndex = map[startSquare]; // get the index of this element in the occupiedSquares array
        occupiedSquares[pieceIndex] = targetSquare;
        map[targetSquare] = pieceIndex;
    }

}