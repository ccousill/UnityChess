using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Queen : Piece
{
    void OnMouseDown(){
        GameManager.Instance.OnChessPieceClicked(this);
    }
    
    public override void FindAvailableSpots()
    {
        ChessBoardManager board = GameManager.Instance.ChessBoard;
        List<Vector2Int> moves = new List<Vector2Int>();
        int xPiecePosition = currentPosition.x;
        int yPiecePosition = currentPosition.y;

        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 1, 0); // Check right
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, -1, 0); // Check left
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 0, 1); // Check up
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 0, -1);
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 1, 1); // Check up right
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, -1, 1); // Check up left
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 1, -1); // Check down right
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, -1, -1); //check down left

        // Filter out moves that are outside the board boundaries
        currentAvailableMoves = moves.Where(pos => IsWithinBounds(pos.x, pos.y)).ToArray();
        // Visualize or use the valid moves as needed
        board.CurrentlyAvailableMoves = currentAvailableMoves;
    }

    private void CheckDirection(ChessBoardManager board, List<Vector2Int> moves, int x, int y, int xDirection, int yDirection)
    {
        x += xDirection;
        y += yDirection;

        while (IsWithinBounds(x, y))
        {
            if (board.GetPieceByCoordinates(new Vector2Int(x, y)) == null)
            {
                moves.Add(new Vector2Int(x, y));
                x += xDirection;
                y += yDirection;
            }
            else if (board.IsTakablePiece(board.GetPieceByCoordinates(new Vector2Int(x, y))))
            {
                moves.Add(new Vector2Int(x, y));
                break;
            }
            else
            {
                break;
            }

        }
    }
}
