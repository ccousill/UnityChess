using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Knight : Piece
{

    //finds the legal spots to move on current knight
    public override void FindAvailableSpots()
    {
        ChessBoardManager board = GameManager.Instance.ChessBoard;
        List<Vector2Int> moves = new List<Vector2Int>();
        int xPiecePosition = CurrentPosition.x;
        int yPiecePosition = CurrentPosition.y;

        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 1, 1); // Check up right
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 1, -1); // Check  down right
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, -1, -1); // Check down left
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, -1, 1); // check up left

        CurrentAvailableMoves = moves.Where(pos => IsWithinBounds(pos.x, pos.y)).ToArray();
        board.CurrentlyAvailableMoves = CurrentAvailableMoves;
    }

    private void CheckDirection(ChessBoardManager board, List<Vector2Int> moves, int x, int y, int xDirection, int yDirection)
    {
        int newX = x + xDirection;
        int newY = y + yDirection * 2;
        AddMove(board, moves, newX, newY);
        newX = x + xDirection * 2;
        newY = y + yDirection;
        AddMove(board, moves, newX, newY);
    }

    private void AddMove(ChessBoardManager board, List<Vector2Int> moves, int newX, int newY)
    {
        if (IsWithinBounds(newX, newY))
        {
            if (board.GetPieceByCoordinates(new Vector2Int(newX, newY)) == null || board.IsTakablePiece(board.GetPieceByCoordinates(new Vector2Int(newX, newY))))
            {
                moves.Add(new Vector2Int(newX, newY));
            }
        }
    }
}
