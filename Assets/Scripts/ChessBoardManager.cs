using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChessBoardManager : MonoBehaviour
{

    private Piece[,] chessboard = new Piece[8, 8];
    private Tile[,] tiles = new Tile[8,8];
    private Vector2Int[] currentlyAvailableMoves;
    public Vector2Int[] CurrentlyAvailableMoves{
        get{return currentlyAvailableMoves;}
        set{currentlyAvailableMoves = value;}
    }

    ParticleManager particleManager;

    public const int BoardSize = 8;
    Piece currentlySelectedPiece;
    public Piece CurrentlySelectedPiece{
        get{return currentlySelectedPiece;}
    }
    public Piece[,] GetChessBoard(){
        return chessboard;
    }
    public Tile[,] GetTiles(){
        return tiles;
    }

    void Start(){
        particleManager = GetComponent<ParticleManager>();
        InitializeChessboard();
    }

    void InitializeChessboard(){
        Piece[] pieces = FindObjectsOfType<Piece>();
            foreach(Piece piece in pieces){
                int xPosition = piece.currentPosition.x;
                int yPosition = piece.currentPosition.y;
                chessboard[xPosition,yPosition] = piece;
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
        ToggleParticles();
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
        if (currentlySelectedPiece != null)
        {
            if(currentlySelectedPiece.IsValid(position)){
                if(GetPieceByCoordinates(position) != null && IsTakablePiece(GetPieceByCoordinates(position))){
                    Piece takenPiece = GetPieceByCoordinates(position);
                    TakePiece(takenPiece);
                }
                Vector2Int oldPosition = currentlySelectedPiece.currentPosition;
                currentlySelectedPiece.Move(position);
                chessboard[position.x,position.y] = currentlySelectedPiece;
                chessboard[oldPosition.x,oldPosition.y] = null;
                DeSelectPiece(currentlySelectedPiece);
            }
        }
    }

    public void ToggleParticles(){
        if(currentlyAvailableMoves != null){
            foreach(Vector2Int position in currentlyAvailableMoves){
                Tile tile = tiles[(int)position.x,(int)position.y];
                particleManager.PlayParticles(tile);
            }
        }
    }

    public void DeleteParticles(){
        if(currentlyAvailableMoves != null){
            foreach(Vector2Int position in currentlyAvailableMoves){
                Tile tile = tiles[(int)position.x,(int)position.y];
                particleManager.DeleteParticles(tile);
            }
        }
    }
    
    public void TakePiece(Piece takenPiece){
        chessboard[takenPiece.currentPosition.x,takenPiece.currentPosition.y] = null;
        Destroy(takenPiece.gameObject);
    }

    public bool IsTakablePiece(Piece piece){{
        return currentlySelectedPiece.pieceColor != piece.pieceColor;
    }}

    public Piece GetPieceByCoordinates(Vector2Int position){
        return chessboard[position.x,position.y];
    }
}
