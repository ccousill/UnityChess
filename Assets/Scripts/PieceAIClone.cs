using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceAIClone
{
    private Vector2Int currentPosition;
    private Player owner;
    private int pieceValue;

    public Vector2Int CurrentPosition{
        get{return currentPosition;}
        set{currentPosition = value;}
    }

    public Player Owner{
        get{return owner;}
        set{owner = value;}
    }

    public int PieceValue{
        get{return pieceValue;}
        set{pieceValue = value;}
    }

    public PieceAIClone(Piece piece){
        this.currentPosition = piece.CurrentPosition;
        this.owner = piece.Owner;
        this.pieceValue = piece.pieceValue;
    }

    public PieceAIClone(PieceAIClone piece){
        this.currentPosition = piece.CurrentPosition;
        this.owner = piece.Owner;
        this.pieceValue = piece.pieceValue;
    }

    static public PieceAIClone Clone(PieceAIClone pieceAi)
    {
        PieceAIClone newPieceClone = new PieceAIClone(pieceAi);
        return newPieceClone;
    }
}
