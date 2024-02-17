using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    Vector2Int position = new Vector2Int();
    ChessBoardManager board;
    private Player currentPlayer;

    void Awake(){
        position = new Vector2Int((int)transform.position.x,(int)transform.position.z);
        board = GameManager.Instance.ChessBoard;
    }
    void OnMouseDown(){
        if(GameManager.Instance.IsClicked){
            GameManager.Instance.HandleMove(position);
        }
    }
}
