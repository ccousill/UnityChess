using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ChessBoardManager : MonoBehaviour
{

    private Piece[,] chessboard = new Piece[8, 8];
    private Tile[,] tiles = new Tile[8, 8];
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
    public Piece[,] GetChessBoard()
    {
        return chessboard;
    }
    public Tile[,] GetTiles()
    {
        return tiles;
    }

    void Start()
    {
        particleManager = GetComponent<ParticleManager>();
        InitializeChessboard();
    }

    void InitializeChessboard()
    {
        Piece[] pieces = FindObjectsOfType<Piece>();
        foreach (Piece piece in pieces)
        {
            int xPosition = piece.CurrentPosition.x;
            int yPosition = piece.CurrentPosition.y;
            chessboard[xPosition, yPosition] = piece;
        }

        Tile[] allTiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in allTiles)
        {
            int x = Mathf.RoundToInt(tile.transform.position.x);
            int z = Mathf.RoundToInt(tile.transform.position.z);
            tiles[x, z] = tile;
        }

    }
    public void SelectPiece(Piece piece)
    {
        currentlySelectedPiece = piece;
        piece.ToggleLift();
        piece.IsSelected = true;
        GameManager.Instance.IsClicked = true;
        currentlySelectedPiece.FindAvailableSpots();
        ShowParticles();
    }

    public void DeSelectPiece(Piece piece)
    {
        piece.ToggleLift();
        DeleteParticles();
        piece.IsSelected = false;
        GameManager.Instance.IsClicked = false;
        currentlyAvailableMoves = null;
        currentlySelectedPiece = null;
    }

    public Piece CompleteTurn(Vector2Int position)
    {
        Piece pieceMoved = currentlySelectedPiece;
        enPassantablePiece = null;
        int forwardDirection = (currentlySelectedPiece.PieceColor == Color.white) ? 1 : -1;
        if (currentlySelectedPiece != null)
        {
            if (currentlySelectedPiece.IsValid(position))
            {
                CheckEnPessant(position, forwardDirection);
                CheckCastle(position);

                if (GetPieceByCoordinates(position) != null && IsTakablePiece(GetPieceByCoordinates(position)))
                {
                    Piece takenPiece = GetPieceByCoordinates(position);
                    TakePiece(takenPiece);
                }


                UpdateBoard(currentlySelectedPiece, position);
                DeSelectPiece(currentlySelectedPiece);
            }
        }
        return pieceMoved;
    }

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

    public void ShowParticles()
    {
        if (currentlyAvailableMoves != null)
        {
            foreach (Vector2Int position in currentlyAvailableMoves)
            {
                Tile tile = tiles[(int)position.x, (int)position.y];
                particleManager.PlayParticles(tile);
            }
        }
    }

    public void DeleteParticles()
    {
        if (currentlyAvailableMoves != null)
        {
            foreach (Vector2Int position in currentlyAvailableMoves)
            {
                Tile tile = tiles[(int)position.x, (int)position.y];
                particleManager.DeleteParticles(tile);
            }
        }
    }

    public void TakePiece(Piece takenPiece)
    {
        chessboard[takenPiece.CurrentPosition.x, takenPiece.CurrentPosition.y] = null;
        Destroy(takenPiece.gameObject);
    }

    public bool IsTakablePiece(Piece piece)
    {
        {
            return currentlySelectedPiece.PieceColor != piece.PieceColor;
        }
    }

    public Piece GetPieceByCoordinates(Vector2Int position)
    {
        if (position.x >= 0 && position.x < BoardSize && position.y >= 0 && position.y < BoardSize)
        {
            return chessboard[position.x, position.y];
        }

        return null;
    }

    public void UpdateBoard(Piece piece, Vector2Int newPos)
    {
        Vector2Int oldPosition = piece.CurrentPosition;
        piece.Move(newPos);
        chessboard[newPos.x, newPos.y] = piece;
        if (oldPosition.x == newPos.x && oldPosition.y == newPos.y)
        {

        }
        else
        {
            chessboard[oldPosition.x, oldPosition.y] = null;
        }

    }
}
