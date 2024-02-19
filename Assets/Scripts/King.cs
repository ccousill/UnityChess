using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;

public class King : Piece
{
    bool inCheck = false;
    private int value = 900;
    protected override void Awake() {
        base.Awake();
        pieceValue = value;
    }

    //finds the legal spots to move on current King
    public override Vector2Int[] FindAvailableSpots()
    {
        ChessBoardManager board = GameManager.Instance.ChessBoard;
        List<Vector2Int> moves = new List<Vector2Int>();
        int xPiecePosition = CurrentPosition.x;
        int yPiecePosition = CurrentPosition.y;
        CheckAround(board, xPiecePosition, yPiecePosition, moves);
        if (!HasMoved)
        {
            CheckCastle(board, xPiecePosition, yPiecePosition, moves);
        }
        CurrentAvailableMoves = moves.Where(pos => IsWithinBounds(pos.x, pos.y)).ToArray();
        return CurrentAvailableMoves;
    }

    //checks the legality of spaces around king
    void CheckAround(ChessBoardManager board, int xPiecePosition, int yPiecePosition, List<Vector2Int> moves)
    {
        for (int xOffset = -1; xOffset <= 1; xOffset++)
        {
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                int x = xPiecePosition + xOffset;
                int y = yPiecePosition + yOffset;

                if (IsWithinBounds(x, y))
                {
                    Piece targetPiece = board.GetPieceByCoordinates(new Vector2Int(x, y));

                    // Check if the spot is empty or contains an opponent's piece
                    if (targetPiece == null || board.IsTakablePiece(targetPiece))
                    {
                        // Process the tile or make it available
                        moves.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
    }

    void CheckCastle(ChessBoardManager board, int xPiecePosition, int yPiecePosition, List<Vector2Int> moves)
    {
        bool isBlocked = false;
        int leftRookX = 0;
        int rightRookX = 7;
        for (int i = xPiecePosition - 1; i > leftRookX; i--)
        {
            if (board.GetPieceByCoordinates(new Vector2Int(i, CurrentPosition.y)) != null)
            {
                isBlocked = true;
            }
        }
        Piece leftRook = board.GetPieceByCoordinates(new Vector2Int(leftRookX,CurrentPosition.y));
        if(!isBlocked && leftRook is Rook && !leftRook.HasMoved){
            moves.Add(new Vector2Int(CurrentPosition.x - 2,CurrentPosition.y));
        }

        isBlocked = false;
        for (int i = xPiecePosition + 1; i < rightRookX ; i++)
        {
            if (board.GetPieceByCoordinates(new Vector2Int(i, CurrentPosition.y)) != null)
            {
                isBlocked = true;
            }
        }
        Piece rightRook = board.GetPieceByCoordinates(new Vector2Int(rightRookX,CurrentPosition.y));
        if(!isBlocked && rightRook is Rook && !rightRook.HasMoved){
            moves.Add(new Vector2Int(CurrentPosition.x + 2,CurrentPosition.y));
        }
    }
}
