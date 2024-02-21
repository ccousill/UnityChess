using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoardManagerClone
{
    private Piece[,] chessboardCopy;

    public ChessBoardManagerClone(ChessBoardManager original)
    {
        chessboardCopy = new Piece[ChessBoardManager.BoardSize, ChessBoardManager.BoardSize];
        Piece[,] originalChessboard = original.GetPieceBoard();
        
        for (int x = 0; x < ChessBoardManager.BoardSize; x++)
        {
            for (int y = 0; y < ChessBoardManager.BoardSize; y++)
            {
                if (originalChessboard[x, y] != null)
                {
                    Piece clonedPiece = originalChessboard[x, y].Clone();
                    chessboardCopy[x, y] = clonedPiece;
                }
            }
        }
    }

    public ChessBoardManagerClone(ChessBoardManagerClone original)
    {
        chessboardCopy = new Piece[ChessBoardManager.BoardSize, ChessBoardManager.BoardSize];
        Piece[,] originalChessboard = original.chessboardCopy;

        for (int x = 0; x < ChessBoardManager.BoardSize; x++)
        {
            for (int y = 0; y < ChessBoardManager.BoardSize; y++)
            {
                if (originalChessboard[x, y] != null)
                {
                    chessboardCopy[x, y] = originalChessboard[x, y];
                }
            }
        }
    }

    public void UpdateBoardState(Piece piece, Vector2Int newPos)
    {
        Vector2Int oldPosition = piece.CurrentPosition;
        chessboardCopy[newPos.x, newPos.y] = piece;
        if (!(oldPosition.x == newPos.x && oldPosition.y == newPos.y))
        {
            chessboardCopy[oldPosition.x, oldPosition.y] = null;
        }
    }
    public List<Piece> getPlayersPieces(Player player)
    {
        List<Piece> result = new List<Piece>();
        foreach (Piece piece in chessboardCopy)
        {
            if (piece != null)
            {
                if (piece.Owner.PlayerColor == player.PlayerColor)
                {
                    result.Add(piece);
                }
            }

        }
        return result;
    }

    public List<Piece> getAllPlayersPieces()
    {
        List<Piece> result = new List<Piece>();
        foreach (Piece piece in chessboardCopy)
        {
            if (piece != null)
            {
                result.Add(piece);
            }

        }
        return result;
    }

}

