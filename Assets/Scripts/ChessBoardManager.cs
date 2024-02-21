using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ChessBoardManager : MonoBehaviour
{

    private Piece[,] pieceBoard = new Piece[8, 8];
    private Tile[,] tileBoard = new Tile[8, 8];
    private Piece enPassantablePiece = null;
    public Piece EnPessantablePiece
    {
        get { return enPassantablePiece; }
        set { enPassantablePiece = value; }
    }
    public const int BoardSize = 8;

    public Piece[,] GetPieceBoard()
    {
        return pieceBoard;
    }
    public Tile[,] GetTileBoard()
    {
        return tileBoard;
    }

    private bool tempBoard = false;
    public ChessBoardManager CloneChessBoardManager()
    {
        ChessBoardManager cloneObject = Instantiate(this);
        Piece[,] originalChessboard = pieceBoard;
        cloneObject.tempBoard = true;
        cloneObject.gameObject.SetActive(false);
        cloneObject.enPassantablePiece = enPassantablePiece;
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                if (originalChessboard[x, y] != null)
                {
                    Piece clonedPiece = originalChessboard[x, y].Clone();
                    cloneObject.pieceBoard[x, y] = clonedPiece;
                }
            }
        }

        return cloneObject;
    }

    void Start()
    {
        if (!tempBoard)
        {
            Debug.Log("initializing chess board");
            InitializeChessboard();

        }
    }

    //sets up 2d array of 8x8 pieces that match the chess board
    void InitializeChessboard()
    {
        Piece[] pieces = FindObjectsOfType<Piece>();
        foreach (Piece piece in pieces)
        {

            int xPosition = piece.CurrentPosition.x;
            int yPosition = piece.CurrentPosition.y;
            pieceBoard[xPosition, yPosition] = piece;
        }

        Tile[] allTiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in allTiles)
        {
            int x = Mathf.RoundToInt(tile.transform.position.x);
            int z = Mathf.RoundToInt(tile.transform.position.z);
            tileBoard[x, z] = tile;
        }
    }

    //Selects a piece which lifts, marks it as selected, and shows available moves that can be made for that piece


    //activates when a selected piece clicks another location on the board.
    //checks are made whether the click is valid or if a piece is selected at all
    public Piece CompleteTurn(Piece currentlySelectedPiece, Vector2Int position)
    {
        Piece pieceMoved = currentlySelectedPiece;
        if (currentlySelectedPiece != null)
        {
            if (currentlySelectedPiece.IsValid(position))
            {
                enPassantablePiece = null;
                int forwardDirection = (currentlySelectedPiece.PieceColor == Color.white) ? 1 : -1;
                CheckEnPessant(currentlySelectedPiece, position, forwardDirection);
                CheckCastle(currentlySelectedPiece, position);

                if (GetPieceByCoordinates(position) != null && IsTakablePiece(currentlySelectedPiece, GetPieceByCoordinates(position)))
                {
                    Piece takenPiece = GetPieceByCoordinates(position);
                    if (takenPiece is King)
                    {
                        GameManager.Instance.GameOver = true;
                    }
                    TakePiece(takenPiece);
                    GameManager.Instance.GetCurrentPlayer().CapturedPieces.Add(takenPiece);
                }


                UpdateBoard(currentlySelectedPiece, position);

                if (currentlySelectedPiece.IsSelected)
                {
                    GameManager.Instance.DeSelectPiece(currentlySelectedPiece);

                }

                GameManager.Instance.EndPlayerTurn();
            }
        }
        return pieceMoved;
    }

    //checks if move that was made is a castle. updates board accordingly by moving rooks in correct positions
    private void CheckCastle(Piece currentlySelectedPiece, Vector2Int position)
    {
        if (currentlySelectedPiece is King)
        {
            if ((position.x == currentlySelectedPiece.CurrentPosition.x - 2))
            {
                //handle left castle
                Piece leftRook = GetPieceByCoordinates(new Vector2Int(0, currentlySelectedPiece.CurrentPosition.y));
                UpdateBoard(leftRook, new Vector2Int(position.x + 1, position.y));
            }
            else if ((position.x == currentlySelectedPiece.CurrentPosition.x + 2))
            {
                //handle right castle
                Piece rightRook = GetPieceByCoordinates(new Vector2Int(7, currentlySelectedPiece.CurrentPosition.y));
                UpdateBoard(rightRook, new Vector2Int(position.x - 1, position.y));
            }
        }
    }

    //checks if move that was made creates an enpessant and if an enpessant is used
    private void CheckEnPessant(Piece currentlySelectedPiece, Vector2Int position, int forwardDirection)
    {
        if (currentlySelectedPiece is Pawn)
        {
            if (currentlySelectedPiece.CurrentPosition.y + 2 == position.y || currentlySelectedPiece.CurrentPosition.y - 2 == position.y)
            {
                EnPessantablePiece = currentlySelectedPiece;
            }
            if (GetPieceByCoordinates(position) == null && position.y == currentlySelectedPiece.CurrentPosition.y + forwardDirection && (currentlySelectedPiece.CurrentPosition.x == position.x + 1 || currentlySelectedPiece.CurrentPosition.x == position.x - 1))
            {
                TakePiece(GetPieceByCoordinates(new Vector2Int(position.x, position.y - forwardDirection)));
            }
        }
    }



    //deletes piece from the board
    public void TakePiece(Piece takenPiece)
    {
        pieceBoard[takenPiece.CurrentPosition.x, takenPiece.CurrentPosition.y] = null;
        Destroy(takenPiece.gameObject);
    }

    //checks if a piece is legally allowed to be taken
    public bool IsTakablePiece(Piece thisPiece, Piece otherPiece)
    {
        return thisPiece.PieceColor != otherPiece.PieceColor;
    }

    //gets a piece from the pieceBoard using coordinates of the pieces position

    public Piece GetPieceByCoordinates(Vector2Int position)
    {
        if (position.x >= 0 && position.x < BoardSize && position.y >= 0 && position.y < BoardSize)
        {
            return pieceBoard[position.x, position.y];
        }

        return null;
    }

    //updates board by moving physical piece on the board and update 
    public void UpdateBoard(Piece piece, Vector2Int newPos)
    {
        Vector2Int oldPosition = piece.CurrentPosition;
        piece.Move(newPos);
        pieceBoard[newPos.x, newPos.y] = piece;
        if (!(oldPosition.x == newPos.x && oldPosition.y == newPos.y))
        {
            pieceBoard[oldPosition.x, oldPosition.y] = null;
        }
    }
    public List<Piece> getPlayersPieces(Player player)
    {
        List<Piece> result = new List<Piece>();
        foreach (Piece piece in pieceBoard)
        {
            if (piece != null)
            {
                if (piece.Owner.PlayerColor == player.PlayerColor)
                {
                    result.Add(piece);
                }
            }

        }
        return result;
    }

    public List<Piece> getAllPlayersPieces()
    {
        List<Piece> result = new List<Piece>();
        foreach (Piece piece in pieceBoard)
        {
            if (piece != null)
            {
                result.Add(piece);
            }

        }
        return result;
    }

    public void DestroyBoard(){
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                if (pieceBoard[x, y] != null)
                {
                   pieceBoard[x,y].DestroyPiece();
                }
            }
        }
        Destroy(gameObject);
    }
}
