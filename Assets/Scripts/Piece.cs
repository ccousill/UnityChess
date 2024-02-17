using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{   
    private Color pieceColor;
    private List<MeshRenderer> childMeshRenderer = new List<MeshRenderer>();
    private bool isSelected = false;
    private bool hasMoved;
    private Player owner;
    private Vector2Int[] currentAvailableMoves;
    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;
    private Vector2Int currentPosition;
    private Vector2Int initialPosition;
    
    public abstract void FindAvailableSpots();
    public Color PieceColor => pieceColor;
    public bool HasMoved => hasMoved;
    public Vector2Int CurrentPosition => currentPosition;
    public Vector2Int InitialPosition => initialPosition;

    public Player Owner{get;set;}
    public bool IsSelected{
        get{return isSelected;}
        set{isSelected = value;}
    }

    public Vector2Int[] CurrentAvailableMoves{
        get{return currentAvailableMoves;}
        set{currentAvailableMoves = value;}
    }

    protected virtual void Awake(){
        currentPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x),Mathf.RoundToInt(transform.position.z));
        initialPosition = currentPosition;
        hasMoved = false;
    }
    protected virtual void Start(){
        for(int i = 0; i<transform.childCount;i++){
            childMeshRenderer.Add(transform.GetChild(i).GetComponent<MeshRenderer>());
        }
        setColor();
    }

    protected virtual void OnMouseDown()
    {
        if(GameManager.Instance.GetCurrentPlayer().PlayerColor == GameManager.Instance.GetColorName(pieceColor)){
            GameManager.Instance.OnChessPieceClicked(this);
        }else if(GameManager.Instance.IsClicked){
            GameManager.Instance.HandleMove(currentPosition);
        }
        else{
            Debug.Log("It's not your turn!");
        }
        
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
            transform.position  += new Vector3(0,.5f,0);
        }else{
            transform.position -= new Vector3(0,.5f,0);
        }
    }
    private void setColor(){
        if(transform.parent.parent.tag == "Black"){
            pieceColor = Color.black;
            for(int i = 0;i<transform.childCount;i++){
                childMeshRenderer[i].material = blackMaterial; 
            }
            Owner = GameManager.Instance.Players[1];

        }else{
            pieceColor = Color.white; 
            for(int i = 0;i<transform.childCount;i++){
                childMeshRenderer[i].material = whiteMaterial; 
            } 
            Owner = GameManager.Instance.Players[0];
        }
    }
    protected bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < ChessBoardManager.BoardSize && y >= 0 && y < ChessBoardManager.BoardSize;
    }
    public bool IsValid(Vector2Int position){
        if(CurrentAvailableMoves.Contains(position)){
            return true;
        }else{
            return false;
        }
    }
}
