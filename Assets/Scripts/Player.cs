using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string PlayerColor {get;set;}
    public List<Piece> CapturedPieces {get;set;}

    public Player(string playerColor){
        PlayerColor = playerColor;
        CapturedPieces = new List<Piece>();
    }


}
