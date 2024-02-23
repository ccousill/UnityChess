using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    public Piece bestPiece;
    public Vector2Int bestMove;
    private ChessBoardManager chessBoard;
    private Piece[,] pieceBoard;
    public void makeAIMove()
    {
        chessBoard = GameManager.Instance.ChessBoard;
        pieceBoard = chessBoard.GetPieceBoard();
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

        Tuple<Piece, Vector2Int> result = null;
        ChessBoardManager gameState = chessBoard.CloneChessBoardManager();
        result = minimax(gameState, int.MinValue + 1, int.MaxValue - 1, 3, true, null, new Vector2Int());
        gameState.DestroyBoard();
        return result;
    }

    private Tuple<Piece, Vector2Int> minimax(ChessBoardManager gameState, int alpha, int beta, int depth, bool maximizingPlayer, Piece bestPiece, Vector2Int bestMove)
    {
        if (depth == 0)
        {
            return Tuple.Create(bestPiece, bestMove);
        }

        int bestEval = maximizingPlayer ? int.MinValue : int.MaxValue;

        List<Piece> currentPlayersPieces = maximizingPlayer
            ? gameState.getPlayersPieces(GameManager.Instance.GetCurrentPlayer())
            : gameState.getPlayersPieces(GameManager.Instance.GetOtherPlayer());
        string currentPlayerColor = GameManager.Instance.GetCurrentPlayer().PlayerColor;

        foreach (Piece playerPiece in currentPlayersPieces)
        {
            Vector2Int[] currentAvailableMoves = playerPiece.FindAvailableSpots(gameState);
            string moves = currentAvailableMoves.ToString();
            int currentX = playerPiece.CurrentPosition.x;
            int currentY = playerPiece.CurrentPosition.y;
            
            Piece pieceToMove = pieceBoard[currentX, currentY];
            foreach (Vector2Int destination in currentAvailableMoves)
            {
                ChessBoardManager childState = gameState.CloneChessBoardManager();
                Piece currentPieceInGameState = childState.GetPieceBoard()[currentX, currentY];
                childState.UpdateBoard(currentPieceInGameState, destination);
                Tuple<Piece, Vector2Int> result = minimax(childState, alpha, beta, depth - 1, !maximizingPlayer, bestPiece, bestMove);
                int eval = Evaluate(childState);
                if (maximizingPlayer)
                {
                    if (eval > bestEval)
                    {
                        bestEval = eval;
                        bestPiece = pieceToMove;
                        bestMove = destination;
                    }
                    alpha = Mathf.Max(alpha, eval);
                }
                else
                {
                    if (eval < bestEval)
                    {
                        bestEval = eval;
                        bestPiece = pieceToMove;
                        bestMove = destination;
                    }
                    beta = Mathf.Min(beta, eval);
                }

                if (beta <= alpha)
                {
                    childState.DestroyBoard();
                    return Tuple.Create(bestPiece, bestMove);
                }

                childState.DestroyBoard();
            }
        }

        return Tuple.Create(bestPiece, bestMove);
    }

    private int Evaluate(ChessBoardManager gameState)
    {
        int whiteMaterial = 0;
        int blackMaterial = 0;
        List<Piece> allPieces = gameState.getAllPlayersPieces();
        foreach (Piece piece in allPieces)
        {
            if (piece.Owner == GameManager.Instance.Players[0])
            {
                whiteMaterial += piece.pieceValue;
            }
            else
            {
                blackMaterial += piece.pieceValue;
            }
        }
        return blackMaterial - whiteMaterial;
    }
}
