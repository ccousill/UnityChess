using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rook : Piece
{
    private int value = 50;
    protected override void Awake()
    {
        base.Awake();
        pieceValue = value;
    }
    //finds the legal spots to move on current Rook
    public override Vector2Int[] FindAvailableSpots()
    {
        ChessBoardManager board = GameManager.Instance.ChessBoard;
        List<Vector2Int> moves = new List<Vector2Int>();
        int xPiecePosition = CurrentPosition.x;
        int yPiecePosition = CurrentPosition.y;

        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 1, 0); // Check right
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, -1, 0); // Check left
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 0, 1); // Check up
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 0, -1);//Check down

        CurrentAvailableMoves = moves.Where(pos => IsWithinBounds(pos.x, pos.y)).ToArray();
        return CurrentAvailableMoves;
    }

    //checks one direction to add to available moves
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
            else if (board.IsTakablePiece(this,board.GetPieceByCoordinates(new Vector2Int(x, y))))
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
