using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;

public class King : Piece
{
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
        CheckAround(board,xPiecePosition,yPiecePosition,moves);
        // Filter out moves that are outside the board boundaries
        currentAvailableMoves = moves.Where(pos => IsWithinBounds(pos.x, pos.y)).ToArray();
        // Visualize or use the valid moves as needed
        board.CurrentlyAvailableMoves = currentAvailableMoves;
    }

    void CheckAround(ChessBoardManager board,int xPiecePosition,int yPiecePosition, List<Vector2Int> moves)
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
}
