using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bishop : Piece
{
    private int value = 30;
    protected override void Awake() {
        base.Awake();
        pieceValue = value;
    }

    //finds the legal spots to move on current Bishop
    public override Vector2Int[] FindAvailableSpots(){
        ChessBoardManager board = GameManager.Instance.ChessBoard;
        List<Vector2Int> moves = new List<Vector2Int>();
        int xPiecePosition = CurrentPosition.x;
        int yPiecePosition = CurrentPosition.y;

        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 1, 1); // Check up right
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, -1, 1); // Check up left
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, 1, -1); // Check down right
        CheckDirection(board, moves, xPiecePosition, yPiecePosition, -1, -1); //check down left

        CurrentAvailableMoves = moves.Where(pos => IsWithinBounds(pos.x, pos.y)).ToArray();
        return CurrentAvailableMoves;
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
