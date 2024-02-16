using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;

public class King : Piece
{
    bool inCheck = false;
    void OnMouseDown()
    {
        GameManager.Instance.OnChessPieceClicked(this);
    }

    public override void FindAvailableSpots()
    {
        ChessBoardManager board = GameManager.Instance.ChessBoard;
        List<Vector2Int> moves = new List<Vector2Int>();
        int xPiecePosition = currentPosition.x;
        int yPiecePosition = currentPosition.y;
        CheckAround(board, xPiecePosition, yPiecePosition, moves);
        if (!HasMoved)
        {
            CheckCastle(board, xPiecePosition, yPiecePosition, moves);
        }
        // Filter out moves that are outside the board boundaries
        currentAvailableMoves = moves.Where(pos => IsWithinBounds(pos.x, pos.y)).ToArray();
        // Visualize or use the valid moves as needed
        board.CurrentlyAvailableMoves = currentAvailableMoves;
    }

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
            if (board.GetPieceByCoordinates(new Vector2Int(i, currentPosition.y)) != null)
            {
                isBlocked = true;
            }
        }
        Piece leftRook = board.GetPieceByCoordinates(new Vector2Int(leftRookX,currentPosition.y));
        if(!isBlocked && leftRook is Rook && !leftRook.HasMoved){
            moves.Add(new Vector2Int(currentPosition.x - 2,currentPosition.y));
        }

        isBlocked = false;
        for (int i = xPiecePosition + 1; i < rightRookX ; i++)
        {
            if (board.GetPieceByCoordinates(new Vector2Int(i, currentPosition.y)) != null)
            {
                isBlocked = true;
            }
        }
        Piece rightRook = board.GetPieceByCoordinates(new Vector2Int(rightRookX,currentPosition.y));
        if(!isBlocked && rightRook is Rook && !rightRook.HasMoved){
            moves.Add(new Vector2Int(currentPosition.x + 2,currentPosition.y));
        }
    }
}
