using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pawn : Piece
{
    private int value = 10;
    protected override void Awake() {
        base.Awake();
        pieceValue = value;
    }

    //finds the legal spots to move on current pawn
    public override Vector2Int[] FindAvailableSpots()
    {
        ChessBoardManager board = GameManager.Instance.ChessBoard;
        Piece[,] pieces = board.GetPieceBoard();
        int forwardDirection = (PieceColor == Color.white) ? 1 : -1;
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int forwardOne = new Vector2Int(CurrentPosition.x, CurrentPosition.y + forwardDirection);
        Vector2Int forwardTwo = new Vector2Int(CurrentPosition.x, CurrentPosition.y + (2 * forwardDirection));
        Vector2Int forwardOneLeft = new Vector2Int(CurrentPosition.x - 1, CurrentPosition.y + forwardDirection);
        Vector2Int forwardOneRight = new Vector2Int(CurrentPosition.x + 1, CurrentPosition.y + forwardDirection);


        if (IsWithinBounds(forwardOne.x, forwardOne.y) && pieces[forwardOne.x, forwardOne.y] == null)
        {
            if (IsWithinBounds(forwardTwo.x, forwardTwo.y) && pieces[forwardTwo.x, forwardTwo.y] == null && CanMoveDouble())
            {
                moves.Add(forwardTwo);
            }
            moves.Add(forwardOne);
        }
        if (board.EnPessantablePiece != null)
        {
            if (board.EnPessantablePiece.CurrentPosition.y == CurrentPosition.y && (CurrentPosition.x == board.EnPessantablePiece.CurrentPosition.x + 1 || CurrentPosition.x == board.EnPessantablePiece.CurrentPosition.x - 1))
            {
                moves.Add(new Vector2Int(board.EnPessantablePiece.CurrentPosition.x, board.EnPessantablePiece.CurrentPosition.y + forwardDirection));
            }
        }

        if (IsWithinBounds(forwardOneLeft.x, forwardOneLeft.y) && pieces[forwardOneLeft.x, forwardOneLeft.y] != null && pieces[forwardOneLeft.x, forwardOneLeft.y].PieceColor != PieceColor)
        {
            moves.Add(forwardOneLeft);
        }
        if (IsWithinBounds(forwardOneRight.x, forwardOneRight.y) && pieces[forwardOneRight.x, forwardOneRight.y] != null && pieces[forwardOneRight.x, forwardOneRight.y].PieceColor != PieceColor)
        {
            moves.Add(forwardOneRight);
        }
        CurrentAvailableMoves = moves.Where(pos => IsWithinBounds(pos.x, pos.y)).ToArray();
        return CurrentAvailableMoves;
    }

    //checks pawn if it has reached the end of the board.
    public bool HasReachedEnd()
    {
        if ((PieceColor == Color.white && CurrentPosition.y == 7) || (PieceColor == Color.black && CurrentPosition.y == 0))
        {
            return true;
        }
        return false;
    }

    //checks if a pawn is legally allowed to move forward twice
    public bool CanMoveDouble()
    {
        if (PieceColor == Color.white)
        {
            return CurrentPosition.y < 2;
        }
        else if (PieceColor == Color.black)
        {
            return CurrentPosition.y > 5;
        }
        return false;
    }


}
