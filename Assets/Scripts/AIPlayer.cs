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

    public static class BoardConstants
    {
        public static readonly int[,] PawnTable =
        {
            {8,8,8,8,8,8,8,8},
            {6,6,6,6,6,6,6,6},
            {4,4,4,4,4,4,4,4},
            {2,2,2,2,2,2,2,2},
            {0,0,0,0,0,0,0,0},
            {-2,-2,-2,-2,-2,-2,-2,-2},
            {-4,-4,-4,-4,-4,-4,-4,-4},
            {-6,-6,-6,-6,-6,-6,-6,-6}
};

public static readonly int[,] BishopTable =
    {
        {-4, -2, -2, -2, -2, -2, -2, -4},
        {-2, 0, 0, 0, 0, 0, 0, -2},
        {-2, 0, 2, 2, 2, 2, 0, -2},
        {-2, 2, 2, 4, 4, 2, 2, -2},
        {-2, 0, 2, 4, 4, 2, 0, -2},
        {-2, 2, 0, 0, 0, 0, 2, -2},
        {-2, 0, 0, 0, 0, 0, 0, -2},
        {-4, -2, -2, -2, -2, -2, -2, -4}
    };

public static readonly int[,] KnightTable =
    {
        {-8, -6, -4, -4, -4, -4, -6, -8},
        {-6, -2, 0, 0, 0, 0, -2, -6},
        {-4, 0, 2, 4, 4, 2, 0, -4},
        {-4, 2, 4, 6, 6, 4, 2, -4},
        {-4, 0, 4, 6, 6, 4, 0, -4},
        {-4, 2, 2, 4, 4, 2, 2, -4},
        {-6, -2, 0, 2, 2, 0, -2, -6},
        {-8, -6, -4, -4, -4, -4, -6, -8}
    };

public static readonly int[,] RookTable =
    {
        {0, 0, 0, 2, 2, 0, 0, 0},
        {-2, 0, 0, 0, 0, 0, 0, -2},
        {-2, 0, 0, 0, 0, 0, 0, -2},
        {-2, 0, 0, 0, 0, 0, 0, -2},
        {-2, 0, 0, 0, 0, 0, 0, -2},
        {-2, 0, 0, 0, 0, 0, 0, -2},
        {2, 2, 2, 2, 2, 2, 2, 2},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

// King table (for general evaluation)
public static readonly int[,] KingTableGeneral =
{
    {-4, -4, -4, -4, -4, -4, -4, -4},
    {-4, -4, -4, -4, -4, -4, -4, -4},
    {-4, -4, -4, -4, -4, -4, -4, -4},
    {-4, -4, -4, -4, -4, -4, -4, -4},
    {-4, -4, -4, -4, -4, -4, -4, -4},
    {-4, -4, -4, -4, -4, -4, -4, -4},
    {-4, -4, -4, -4, -4, -4, -4, -4},
    {-4, -4, -4, -4, -4, -4, -4, -4}
};

public static readonly int[,] QueenTable =
    {
        {-4, -2, -2, -1, -1, -2, -2, -4},
        {-2, 0, 0, 0, 0, 0, 0, -2},
        {-2, 0, 1, 1, 1, 1, 0, -2},
        {-1, 0, 1, 1, 1, 1, 0, -1},
        {0, 0, 1, 1, 1, 1, 0, -1},
        {-2, 1, 1, 1, 1, 1, 0, -2},
        {-2, 0, 1, 0, 0, 0, 0, -2},
        {-4, -2, -2, -1, -1, -2, -2, -4}
    };

    }
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
        result = minimax(gameState, int.MinValue + 1, int.MaxValue - 1, 1, true, null, new Vector2Int());
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
        int pieceSquareValue = 0;
        foreach (Piece piece in allPieces)
        {
            if(piece is Pawn){
                pieceSquareValue = BoardConstants.PawnTable[piece.CurrentPosition.x,piece.CurrentPosition.y];
            }
            else if(piece is Knight){
                pieceSquareValue = BoardConstants.KnightTable[piece.CurrentPosition.x,piece.CurrentPosition.y];
            }
            else if(piece is Rook){
                pieceSquareValue = BoardConstants.RookTable[piece.CurrentPosition.x,piece.CurrentPosition.y];
            }
            else if(piece is Bishop){
                pieceSquareValue = BoardConstants.BishopTable[piece.CurrentPosition.x,piece.CurrentPosition.y];
            }
            else if(piece is Queen){
                pieceSquareValue = BoardConstants.QueenTable[piece.CurrentPosition.x,piece.CurrentPosition.y];
            }
            else{
                pieceSquareValue = BoardConstants.KingTableGeneral[piece.CurrentPosition.x,piece.CurrentPosition.y];
            }
            if (piece.Owner == GameManager.Instance.Players[0])
            {
                whiteMaterial = whiteMaterial + piece.pieceValue + pieceSquareValue;
            }
            else
            {
                blackMaterial = blackMaterial + piece.pieceValue + pieceSquareValue;
            }
        }
        return whiteMaterial + blackMaterial;
    }
}
