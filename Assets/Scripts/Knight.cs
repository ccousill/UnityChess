using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    void OnMouseDown(){
        GameManager.Instance.OnChessPieceClicked(this);
    }
    
    public override void FindAvailableSpots(){
        
    }
}
