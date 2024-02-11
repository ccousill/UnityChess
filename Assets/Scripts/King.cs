using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    void OnMouseDown(){
        GameManager.Instance.OnChessPieceClicked(this);
    }

    public override void FindAvailableSpots(){
        
    }
}
