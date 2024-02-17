using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Queen : Piece
{
    public override void FindAvailableSpots()
    {
        ChessBoardManager board = GameManager.Instance.ChessBoard;
        List<Vector2Int> moves = new List<Vector2Int>();
        int xPiecePosition = CurrentPosition.x;
        int yPiecePosition = CurrentPosition.y;

        //combine both rook and bishop checks
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 1, 0); 
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, -1, 0); 
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 0, 1); 
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 0, -1);
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 1, 1); 
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, -1, 1); 
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 1, -1); 
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, -1, -1); 

        CurrentAvailableMoves = moves.Where(pos => IsWithinBounds(pos.x, pos.y)).ToArray();
        board.CurrentlyAvailableMoves = CurrentAvailableMoves;
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
