using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    Vector2Int position = new Vector2Int();
    ChessBoardManager board;
    private Material tileMaterial;

    void Awake(){
        position = new Vector2Int((int)transform.position.x,(int)transform.position.z);
        board = GameManager.Instance.ChessBoard;
    }
    void Start(){
        tileMaterial = GetComponent<Renderer>().material;
    }
    void OnMouseDown(){
        if(GameManager.Instance.IsClicked){
            GameManager.Instance.HandleMove(position);
        }
    }

    public void colorAvailableSpots(){
        if(tileMaterial.color == Color.red){
            tileMaterial.color = Color.white;
        }else{
            tileMaterial.color = Color.red;
        }
    }
}
