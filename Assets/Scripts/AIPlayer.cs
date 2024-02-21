using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    public Piece bestPiece;
    public Vector2Int bestMove;
    public void makeAIMove(ChessBoardManager chessBoard)
    {
        bestPiece = null;
        bestMove = new Vector2Int();
        Tuple<Piece, Vector2Int> result = getPieceAndDestinationLocation(chessBoard);
        Debug.Log(result);
        if (result != null)
        {
            Debug.Log(result.Item1);
            Debug.Log(result.Item2);
            result.Item1.CurrentAvailableMoves = result.Item1.FindAvailableSpots(chessBoard);
            chessBoard.CompleteTurn(result.Item1, result.Item2);
        }
    }

    public Tuple<Piece, Vector2Int> getPieceAndDestinationLocation(ChessBoardManager chessBoard)
    {
        if (chessBoard == null)
        {
            return Tuple.Create((Piece)null, new Vector2Int(-1, -1));
        }
        ChessBoardManager gameState = chessBoard.CloneChessBoardManager();
        Tuple<Piece, Vector2Int> result = minimax(gameState, 1, true, null, new Vector2Int());
        gameState.DestroyBoard();
        return result;
    }

    private Tuple<Piece, Vector2Int> minimax(ChessBoardManager gameState, int depth, bool maximizingPlayer, Piece bestPiece, Vector2Int bestMove)
    {
        if (depth == 0)
        {
            return Tuple.Create(bestPiece, bestMove);
        }

        int bestEval = maximizingPlayer ? int.MinValue : int.MaxValue;

        List<Piece> currentPlayersPieces = maximizingPlayer
            ? gameState.getPlayersPieces(GameManager.Instance.GetCurrentPlayer())
            : gameState.getPlayersPieces(GameManager.Instance.GetOtherPlayer());

        foreach (Piece playerPiece in currentPlayersPieces)
        {
            Vector2Int[] currentAvailableMoves = playerPiece.FindAvailableSpots(gameState);
            int currentX = playerPiece.CurrentPosition.x;
            int currentY = playerPiece.CurrentPosition.y;
            foreach (Vector2Int destination in currentAvailableMoves)
            {
                ChessBoardManager childState = gameState.CloneChessBoardManager();
                Piece currentPiece = childState.GetPieceBoard()[currentX,currentY];
                childState.UpdateBoard(currentPiece, destination);
                Tuple<Piece, Vector2Int> result = minimax(childState, depth - 1, !maximizingPlayer, bestPiece, bestMove);
                Debug.Log(childState.GetPieceBoard()[0, 4] + " " + childState.GetPieceBoard()[0, 5]);
                int eval = Evaluate(childState);

                Debug.Log(playerPiece + " " + eval);
                if ((maximizingPlayer && eval > bestEval) || (!maximizingPlayer && eval < bestEval))
                {
                    ChessBoardManager chessBoard = GameManager.Instance.ChessBoard;
                    Piece[,] pieceBoard = chessBoard.GetPieceBoard();
                    bestEval = eval;
                    bestPiece = pieceBoard[currentX, currentY];
                    bestMove = destination;
                }
                childState.DestroyBoard();
            }
        }

        return Tuple.Create(bestPiece, bestMove);
    }

    private int Evaluate(ChessBoardManager gameState)
    {
        int materialAdvantage = CalculateMaterialAdvantage(gameState);
        return materialAdvantage;
    }

    private int CalculateMaterialAdvantage(ChessBoardManager gameState)
    {
        int whiteMaterial = 0;
        int blackMaterial = 0;

        foreach (Piece piece in gameState.getAllPlayersPieces())
        {
            Debug.Log(piece + " " + piece.Owner.PlayerColor + " " + piece.CurrentPosition);
                if (piece.Owner == GameManager.Instance.Players[0])
                {
                    whiteMaterial += piece.pieceValue;
                }
                else
                {
                    blackMaterial += piece.pieceValue;
                }
        }

        Debug.Log(whiteMaterial);
        Debug.Log(blackMaterial);
        return blackMaterial - whiteMaterial;
    }
}
