using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ChessBoardManager : MonoBehaviour
{

    private Piece[,] pieceBoard = new Piece[8, 8];
    private Tile[,] tileBoard = new Tile[8, 8];
    private Vector2Int[] currentlyAvailableMoves;
    private Piece enPassantablePiece = null;
    public Piece EnPessantablePiece
    {
        get { return enPassantablePiece; }
        set { enPassantablePiece = value; }
    }
    public Vector2Int[] CurrentlyAvailableMoves
    {
        get { return currentlyAvailableMoves; }
        set { currentlyAvailableMoves = value; }
    }

    ParticleManager particleManager;

    public const int BoardSize = 8;
    private Piece currentlySelectedPiece;
    public Piece CurrentlySelectedPiece
    {
        get { return currentlySelectedPiece; }
    }
    public Piece[,] GetPieceBoard()
    {
        return pieceBoard;
    }
    public Tile[,] GetTileBoard()
    {
        return tileBoard;
    }

    void Start()
    {
        particleManager = GetComponent<ParticleManager>();
        InitializeChessboard();
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
    public void SelectPiece(Piece piece)
    {
        currentlySelectedPiece = piece;
        piece.ToggleLift();
        piece.IsSelected = true;
        GameManager.Instance.IsClicked = true;
        currentlySelectedPiece.FindAvailableSpots();
        ShowParticles();
    }

    //deselects a piece which sets it down and unmarks as selected
    public void DeSelectPiece(Piece piece)
    {
        piece.ToggleLift();
        DeleteParticles();
        piece.IsSelected = false;
        GameManager.Instance.IsClicked = false;
        currentlyAvailableMoves = null;
        currentlySelectedPiece = null;
    }

    //activates when a selected piece clicks another location on the board.
    //checks are made whether the click is valid or if a piece is selected at all
    public Piece CompleteTurn(Vector2Int position)
    {
        Piece pieceMoved = currentlySelectedPiece;
        if (currentlySelectedPiece != null)
        {
            if (currentlySelectedPiece.IsValid(position))
            {
                enPassantablePiece = null;
                int forwardDirection = (currentlySelectedPiece.PieceColor == Color.white) ? 1 : -1;
                CheckEnPessant(position, forwardDirection);
                CheckCastle(position);

                if (GetPieceByCoordinates(position) != null && IsTakablePiece(GetPieceByCoordinates(position)))
                {
                    Piece takenPiece = GetPieceByCoordinates(position);
                    if(takenPiece is King){
                        GameManager.Instance.GameOver = true;
                    }
                    TakePiece(takenPiece);
                    GameManager.Instance.GetCurrentPlayer().CapturedPieces.Add(takenPiece);
                }


                UpdateBoard(currentlySelectedPiece, position);
                DeSelectPiece(currentlySelectedPiece);
                GameManager.Instance.EndPlayerTurn();
            }
        }
        return pieceMoved;
    }

    //checks if move that was made is a castle. updates board accordingly by moving rooks in correct positions
    private void CheckCastle(Vector2Int position)
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
    private void CheckEnPessant(Vector2Int position, int forwardDirection)
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

    //plays particles using the currently available moves made from pieces
    public void ShowParticles()
    {
        if (currentlyAvailableMoves != null)
        {
            foreach (Vector2Int position in currentlyAvailableMoves)
            {
                Tile tile = tileBoard[(int)position.x, (int)position.y];
                particleManager.PlayParticles(tile);
            }
        }
    }
    //Deletes particles on the board

    public void DeleteParticles()
    {
        if (currentlyAvailableMoves != null)
        {
            foreach (Vector2Int position in currentlyAvailableMoves)
            {
                Tile tile = tileBoard[(int)position.x, (int)position.y];
                particleManager.DeleteParticles(tile);
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
    public bool IsTakablePiece(Piece piece)
    {
        {
            return currentlySelectedPiece.PieceColor != piece.PieceColor;
        }
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
}
