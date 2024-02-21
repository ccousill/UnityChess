using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private bool isClicked;
    private int currentPlayerIndex = 0;
    ParticleManager particleManager;
    private List<Player> players = new List<Player>();
    public List<Player> Players => players;
    private Piece lastMovedPiece;

    private ChessBoardManager chessBoard;
    private PawnPromotionUIManager pawnPromotionUI;
    public GameObject rookPrefab;
    public GameObject queenPrefab;
    public GameObject knightPrefab;
    public GameObject bishopPrefab;
    public AIPlayer AI;

    public bool GameOver { get; set; }

    private Piece currentlySelectedPiece;
    public Piece CurrentlySelectedPiece
    {
        get { return currentlySelectedPiece; }
        set { currentlySelectedPiece = value; }
    }
    public ChessBoardManager ChessBoard
    {
        get { return chessBoard; }
    }

    private Vector2Int[] currentlyAvailableMoves;
    public Vector2Int[] CurrentlyAvailableMoves
    {
        get { return currentlyAvailableMoves; }
        set { currentlyAvailableMoves = value; }
    }
    public bool IsClicked
    {
        get { return isClicked; }
        set { isClicked = value; }
    }

    public enum GameState
    {
        Normal,
        PawnPromotion
    }
    private GameState currentGameState = GameState.Normal;


    // Property to access the GameManager instance
    public static GameManager Instance
    {
        get
        {
            // If the instance doesn't exist, find it in the scene
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                // If still not found, log an error
                if (_instance == null)
                {
                    Debug.LogError("GameManager instance not found in the scene.");
                }
            }

            return _instance;
        }
    }
    void Awake()
    {
        Debug.Log("on start");
        isClicked = false;
        GameOver = false;
        chessBoard = FindObjectOfType<ChessBoardManager>();
        Debug.Log(chessBoard);
        pawnPromotionUI = FindObjectOfType<PawnPromotionUIManager>();
        particleManager = GetComponent<ParticleManager>();
        AI = FindAnyObjectByType<AIPlayer>();
        pawnPromotionUI.HidePawnPromotionUI();
        InitializePlayers();
    }

    void InitializePlayers()
    {
        //player 1 will be white
        players.Add(new Player("White"));
        //player 2 will be black
        players.Add(new Player("Black"));
        StartPlayerTurn();
    }

    void StartPlayerTurn()
    {
        Player currentPlayer = GetCurrentPlayer();
        Debug.Log($"It is currently Player {currentPlayer.PlayerColor}'s turn");
        if (currentPlayer.PlayerColor == "Black")
        {
            Debug.Log("AIs Turn!");
            AI.makeAIMove(chessBoard);
        }
        else
        {
            Debug.Log("Players Turn");
        }

    }

    public void EndPlayerTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        StartPlayerTurn();
    }

    public Player GetCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }
    public Player GetOtherPlayer()
    {
        return players[(currentPlayerIndex + 1) % players.Count];
    }

    public void SelectPiece(Piece piece)
    {
        currentlySelectedPiece = piece;
        piece.ToggleLift();
        piece.IsSelected = true;
        GameManager.Instance.IsClicked = true;
        currentlyAvailableMoves = currentlySelectedPiece.FindAvailableSpots(chessBoard);
        ShowParticles();
    }

    //deselects a piece which sets it down and unmarks as selected
    public void DeSelectPiece(Piece piece)
    {
        piece.ToggleLift();
        DeleteParticles();
        piece.IsSelected = false;
        GameManager.Instance.IsClicked = false;
        currentlyAvailableMoves = null;
        currentlySelectedPiece = null;
    }

    //handles logic for when a piece is clicked.
    public void OnChessPieceClicked(Piece clickedPiece)
    {

        if (IsValidSelection(clickedPiece))
        {
            if (ReferenceEquals(clickedPiece, currentlySelectedPiece))
            {
                DeSelectPiece(clickedPiece);
            }
            else
            {
                if (currentlySelectedPiece != null)
                {
                    DeSelectPiece(currentlySelectedPiece);
                }
                SelectPiece(clickedPiece);
            }

        }
    }

    //checks if you are allowed to click a certain piece
    public bool IsValidSelection(Piece clickedPiece)
    {
        if (GetCurrentPlayer().PlayerColor != GetColorName(clickedPiece.PieceColor))
        {
            if (isClicked)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    //handles move sequence 
    public void HandleMove(Vector2Int position)
    {
        if (currentGameState == GameState.Normal)
        {

            Piece pieceMoved = chessBoard.CompleteTurn(currentlySelectedPiece,position);
            lastMovedPiece = pieceMoved;
            if (GameOver)
            {
                GameOverSequence();
            }
            else if (lastMovedPiece is Pawn)
            {
                Pawn piece = (Pawn)pieceMoved;
                if (piece.HasReachedEnd())
                {
                    currentGameState = GameState.PawnPromotion;
                    PawnPromotionUI();
                }
            }
        }
    }

    //calls ui for pawn promotion
    public void PawnPromotionUI()
    {
        pawnPromotionUI.ShowPawnPromotionUI();
    }

    //resets game after a win
    private void GameOverSequence()
    {
        Debug.Log("Game Over!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //promotes pawn when selection is made from promotion UI

    public void PawnPromotion(string selectedPieceType)
    {
        Piece newPiece = InstantiatePiece(selectedPieceType);
        chessBoard.UpdateBoard(newPiece, new Vector2Int(newPiece.CurrentPosition.x, newPiece.CurrentPosition.y));
        Destroy(lastMovedPiece.gameObject);
        currentGameState = GameState.Normal;
        pawnPromotionUI.HidePawnPromotionUI();
    }

    //instantiates a piece on given a string
    private Piece InstantiatePiece(string pieceType)
    {
        GameObject piecePrefab = GetPrefabByType(pieceType);
        string playerColor = GetColorName(lastMovedPiece.PieceColor);
        string parentPath = $"Game/Pieces/{playerColor}/{pieceType}s";
        GameObject parentObject = GameObject.Find(parentPath);
        GameObject newPieceObject = Instantiate(piecePrefab, new Vector3(lastMovedPiece.CurrentPosition.x, .75f, lastMovedPiece.CurrentPosition.y), Quaternion.identity);
        newPieceObject.transform.SetParent(parentObject.transform);
        Piece newPiece = newPieceObject.GetComponent<Piece>();
        return newPiece;
    }

    //gets a prefab given a string
    private GameObject GetPrefabByType(string pieceType)
    {
        switch (pieceType)
        {
            case "Rook":
                return rookPrefab;
            case "Queen":
                return queenPrefab;
            case "Knight":
                return knightPrefab;
            case "Bishop":
                return bishopPrefab;
            // ... other cases for different piece types
            default:
                return null;
        }
    }

    //returns string color given type Color
    public string GetColorName(Color color)
    {
        // Check if the color is closer to white or black based on the tolerance
        if (Mathf.Approximately(color.r, 1f) && Mathf.Approximately(color.g, 1f) && Mathf.Approximately(color.b, 1f) && Mathf.Approximately(color.a, 1f))
        {
            return "White";
        }
        else if (Mathf.Approximately(color.r, 0f) && Mathf.Approximately(color.g, 0f) && Mathf.Approximately(color.b, 0f) && Mathf.Approximately(color.a, 1f))
        {
            return "Black";
        }
        else
        {
            return "Unknown";
        }
    }

    //plays particles using the currently available moves made from pieces
    public void ShowParticles()
    {
        if (currentlyAvailableMoves != null)
        {
            foreach (Vector2Int position in currentlyAvailableMoves)
            {
                Tile tile = chessBoard.GetTileBoard()[(int)position.x, (int)position.y];
                particleManager.PlayParticles(tile);
            }
        }
    }
    //Deletes particles on the board

    public void DeleteParticles()
    {
        if (currentlyAvailableMoves != null)
        {
            foreach (Vector2Int position in currentlyAvailableMoves)
            {
                Tile tile = chessBoard.GetTileBoard()[(int)position.x, (int)position.y];
                particleManager.DeleteParticles(tile);
            }
        }
    }


}
