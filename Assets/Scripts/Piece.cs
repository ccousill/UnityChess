using System;
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

    public abstract Vector2Int[] FindAvailableSpots();
    public Color PieceColor => pieceColor;
    public bool HasMoved => hasMoved;
    public Vector2Int InitialPosition => initialPosition;
    public Player Owner { get; set; }
    public bool IsSelected
    {
        get { return isSelected; }
        set { isSelected = value; }
    }

    public Vector2Int CurrentPosition
    {
        get { return currentPosition; }
        set { currentPosition = value; }
    }

    public Vector2Int[] CurrentAvailableMoves
    {
        get { return currentAvailableMoves; }
        set { currentAvailableMoves = value; }
    }
    public int pieceValue;


    //initialize Pieces position
    public Piece()
    {
        currentPosition = Vector2Int.zero;
        owner = null;
    }

    protected virtual void Awake()
    {
        currentPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        initialPosition = currentPosition;
        hasMoved = false;
    }

    //initialize piece color by checking parent tag
    protected virtual void Start()
    {

        if (Owner == null)
        {
            Debug.Log("DAmn");
            for (int i = 0; i < transform.childCount; i++)
            {
                childMeshRenderer.Add(transform.GetChild(i).GetComponent<MeshRenderer>());
            }

            SetColorProperties();
        }

    }

    //checks mouse click on piece

    protected virtual void OnMouseDown()
    {
        if (GameManager.Instance.GetCurrentPlayer().PlayerColor == GameManager.Instance.GetColorName(pieceColor))
        {
            GameManager.Instance.OnChessPieceClicked(this);
        }
        else if (GameManager.Instance.IsClicked)
        {
            GameManager.Instance.HandleMove(currentPosition);
        }
        else
        {
            Debug.Log("It's not your turn!");
        }

    }

    //transforms pieces position on the board and updates current position property
    public void Move(Vector2Int newPosition)
    {
        transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.y);
        if (!hasMoved)
        {
            hasMoved = true;
        }
        currentPosition = newPosition;
    }

    //transforms piece by lifting it, giving a visual indicator of was piece is selected
    public void ToggleLift()
    {
        isSelected = !isSelected;
        if (isSelected)
        {
            transform.position += new Vector3(0, .5f, 0);
        }
        else
        {
            transform.position -= new Vector3(0, .5f, 0);
        }
    }

    //method to set the color and owner of the piece
    private void SetColorProperties()
    {
        if (transform.parent.parent.tag == "Black" || (Owner != null && Owner.PlayerColor == "Black"))
        {
            pieceColor = Color.black;
            for (int i = 0; i < transform.childCount; i++)
            {
                childMeshRenderer[i].material = blackMaterial;
            }
            Owner = GameManager.Instance.Players[1];
            pieceValue *= -1;

        }
        else
        {
            pieceColor = Color.white;
            for (int i = 0; i < transform.childCount; i++)
            {
                childMeshRenderer[i].material = whiteMaterial;
            }
            Owner = GameManager.Instance.Players[0];
        }
    }

    //checks if two coordinates are within bounds of the board
    protected bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < ChessBoardManager.BoardSize && y >= 0 && y < ChessBoardManager.BoardSize;
    }

    //checks if the position clicked is in the available moves of a given piece 
    public bool IsValid(Vector2Int position)
    {
        if (CurrentAvailableMoves.Contains(position))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Piece Clone()
    {
        Piece newPieceObject = Instantiate(this);
        newPieceObject.transform.SetParent(transform.parent.parent);
        newPieceObject.Owner = Owner;
        newPieceObject.CurrentPosition = currentPosition;
        newPieceObject.pieceValue = pieceValue;
        newPieceObject.pieceColor = this.PieceColor;
        newPieceObject.gameObject.SetActive(false);
        return newPieceObject;
    }

}
