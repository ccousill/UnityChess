using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    void OnMouseDown(){
        GameManager.Instance.OnChessPieceClicked(this);
    }

    public override void FindAvailableSpots(){
        
    }
}
