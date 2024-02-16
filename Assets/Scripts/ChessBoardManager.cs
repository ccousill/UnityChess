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
    Piece currentlySelectedPiece;
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
            int xPosition = piece.currentPosition.x;
            int yPosition = piece.currentPosition.y;
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

    public void CompleteTurn(Vector2Int position)
    {
        enPassantablePiece = null;
        int forwardDirection = (currentlySelectedPiece.pieceColor == Color.white) ? 1 : -1;
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
    }

    private void CheckCastle(Vector2Int position)
    {
        if (currentlySelectedPiece is King)
        {
            if ((position.x == currentlySelectedPiece.currentPosition.x - 2))
            {
                //handle left castle
                Piece leftRook = GetPieceByCoordinates(new Vector2Int(0, currentlySelectedPiece.currentPosition.y));
                UpdateBoard(leftRook, new Vector2Int(position.x + 1, position.y));
            }
            else if ((position.x == currentlySelectedPiece.currentPosition.x + 2))
            {
                //handle right castle
                Piece rightRook = GetPieceByCoordinates(new Vector2Int(7, currentlySelectedPiece.currentPosition.y));
                UpdateBoard(rightRook, new Vector2Int(position.x - 1, position.y));
            }
        }
    }

    private void CheckEnPessant(Vector2Int position, int forwardDirection)
    {
        if (currentlySelectedPiece is Pawn)
        {
            if (currentlySelectedPiece.currentPosition.y + 2 == position.y || currentlySelectedPiece.currentPosition.y - 2 == position.y)
            {
                EnPessantablePiece = currentlySelectedPiece;
            }
            if (GetPieceByCoordinates(position) == null && position.y == currentlySelectedPiece.currentPosition.y + forwardDirection && (currentlySelectedPiece.currentPosition.x == position.x + 1 || currentlySelectedPiece.currentPosition.x == position.x - 1))
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
        chessboard[takenPiece.currentPosition.x, takenPiece.currentPosition.y] = null;
        Destroy(takenPiece.gameObject);
    }

    public bool IsTakablePiece(Piece piece)
    {
        {
            return currentlySelectedPiece.pieceColor != piece.pieceColor;
        }
    }

    public Piece GetPieceByCoordinates(Vector2Int position)
    {
        return chessboard[position.x, position.y];
    }

    private void UpdateBoard(Piece piece, Vector2Int newPos)
    {
        Vector2Int oldPosition = piece.currentPosition;
        piece.Move(newPos);
        chessboard[newPos.x, newPos.y] = piece;
        chessboard[oldPosition.x, oldPosition.y] = null;
    }
}
