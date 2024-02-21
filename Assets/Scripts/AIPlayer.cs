using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    public Piece bestPiece;
    public Vector2Int bestMove;
    public void makeAIMove()
    {
        bestPiece = null;
        bestMove = new Vector2Int();
        Debug.Log("Time to move");
        Tuple<Piece, Vector2Int> result = getPieceAndDestinationLocation();
        ChessBoardManager chessBoard = GameManager.Instance.ChessBoard;
        Debug.Log(result);
        if (result != null)
        {
            Debug.Log("result!");
            Debug.Log(result.Item1);
            Debug.Log(result.Item2);
            chessBoard.UpdateBoard(result.Item1, result.Item2);
        }
        GameManager.Instance.EndPlayerTurn();
    }

    public Tuple<Piece, Vector2Int> getPieceAndDestinationLocation()
    {
        ChessBoardManager chessBoard = GameManager.Instance.ChessBoard;

        if (chessBoard == null)
        {
            Debug.LogError("ChessBoard is null");
            return Tuple.Create((Piece)null, new Vector2Int(-1, -1));
        }

        ChessBoardManagerClone gameState = new ChessBoardManagerClone(chessBoard);
        int result = minimax(gameState, 1, true);
        return Tuple.Create(bestPiece, bestMove);
    }

    private int minimax(ChessBoardManagerClone gameState, int depth, bool maximizingPlayer)
{
    if (depth == 0)
    {
        return Evaluate(gameState);
    }

    int bestEval = maximizingPlayer ? int.MinValue : int.MaxValue;

    List<Piece> currentPlayersPieces = maximizingPlayer
        ? gameState.getPlayersPieces(GameManager.Instance.GetCurrentPlayer())
        : gameState.getPlayersPieces(GameManager.Instance.GetOtherPlayer());

    foreach (Piece playerPiece in currentPlayersPieces)
    {
        Vector2Int[] currentAvailableMoves = playerPiece.FindAvailableSpots();

        foreach (Vector2Int destination in currentAvailableMoves)
        {
            ChessBoardManagerClone childState = new ChessBoardManagerClone(gameState);
            childState.UpdateBoardState(playerPiece, destination);
            int eval = minimax(childState, depth - 1, !maximizingPlayer);
            if ((maximizingPlayer && eval > bestEval) || (!maximizingPlayer && eval < bestEval))
                {
                    ChessBoardManager chessBoard = GameManager.Instance.ChessBoard;
                    Piece[,] pieceBoard = chessBoard.GetPieceBoard();
                    bestEval = eval;
                    bestPiece = pieceBoard[playerPiece.CurrentPosition.x, playerPiece.CurrentPosition.y];
                    bestMove = destination;
                }
        }
    }

    return bestEval;
}

    private int Evaluate(ChessBoardManagerClone gameState)
    {
        int materialAdvantage = CalculateMaterialAdvantage(gameState);
        return materialAdvantage;
    }

    private int CalculateMaterialAdvantage(ChessBoardManagerClone gameState)
    {
        int whiteMaterial = 0;
        int blackMaterial = 0;

        foreach (Piece piece in gameState.getAllPlayersPieces())
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

        return whiteMaterial - blackMaterial;
    }
}
