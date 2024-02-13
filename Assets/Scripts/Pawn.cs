using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pawn : Piece
{
    void OnMouseDown(){
        GameManager.Instance.OnChessPieceClicked(this);
    }

    public override void FindAvailableSpots(){
        ChessBoardManager board = GameManager.Instance.ChessBoard;
        Piece[,] pieces = board.GetChessBoard();
        int forwardDirection = (pieceColor == Color.white) ? 1:-1; 
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int forwardOne = new Vector2Int(currentPosition.x, currentPosition.y + forwardDirection);
        Vector2Int forwardTwo = new Vector2Int(currentPosition.x, currentPosition.y + (2*forwardDirection));
        Vector2Int forwardOneLeft = new Vector2Int(currentPosition.x -1, currentPosition.y + forwardDirection);
        Vector2Int forwardOneRight = new Vector2Int(currentPosition.x+ 1, currentPosition.y + forwardDirection);

        if(IsWithinBounds(forwardOne.x,forwardOne.y) && pieces[forwardOne.x,forwardOne.y] == null){
            if(pieces[forwardTwo.x,forwardTwo.y] == null && CanMoveDouble()){
                    moves.Add(forwardTwo);
            }
                moves.Add(forwardOne);
        }

        // Add additional checks for capturing opponent pieces diagonally
        if(IsWithinBounds(forwardOneLeft.x,forwardOneLeft.y) && pieces[forwardOneLeft.x,forwardOneLeft.y] != null && pieces[forwardOneLeft.x,forwardOneLeft.y].pieceColor != pieceColor){
            moves.Add(forwardOneLeft);
        }
        if(IsWithinBounds(forwardOneRight.x,forwardOneRight.y) && pieces[forwardOneRight.x,forwardOneRight.y] != null && pieces[forwardOneRight.x,forwardOneRight.y].pieceColor != pieceColor){
            moves.Add(forwardOneRight);
        }
        // Filter out moves that are outside the board boundaries
        currentAvailableMoves = moves.Where(pos => IsWithinBounds(pos.x, pos.y)).ToArray();
        // Visualize or use the valid moves as needed
        board.CurrentlyAvailableMoves = currentAvailableMoves;
    }

    public bool CanMoveDouble(){
        if(pieceColor == Color.white){
            return currentPosition.y < 2;
        }
        else if(pieceColor == Color.black){
            return currentPosition.y > 5;
        }
        return false;
    }

}
