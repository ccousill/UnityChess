using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{   
    public Color pieceColor;
    public Vector2Int currentPosition;
    public Vector2Int[] currentAvailableMoves;
    private bool isSelected = false;
    public abstract void FindAvailableSpots();
    private bool hasMoved;
    public bool HasMoved{
        get{return hasMoved;}
    }
    public bool IsSelected{
        get{return isSelected;}
        set{isSelected = value;}
    }

    void Awake(){
        currentPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x),Mathf.RoundToInt(transform.position.z));
        hasMoved = false;
        setColor();
    }
    public void Move(Vector2Int newPosition){
        transform.position = new Vector3(newPosition.x,transform.position.y,newPosition.y);
        if(!hasMoved){
            hasMoved = true;
        }
        currentPosition = newPosition;
    }

    public void ToggleLift(){
        isSelected = !isSelected;
        if(isSelected){
            Debug.Log("transform");
            transform.position  += new Vector3(0,.5f,0);
        }else{
            transform.position -= new Vector3(0,.5f,0);
        }
    }
    void setColor(){
        if(transform.parent.parent.tag == "Black" || transform.parent.tag == "Black"){
            pieceColor = Color.black;
        }else{
            pieceColor = Color.white;
        }
    }
    public bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < ChessBoardManager.BoardSize && y >= 0 && y < ChessBoardManager.BoardSize;
    }
    public bool IsValid(Vector2Int position){
        if(currentAvailableMoves.Contains(position)){
            return true;
        }else{
            return false;
        }
    }
}
