using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private bool isClicked;
    private int currentPlayerIndex = 0;
    public List<Player> players = new List<Player>();

    ChessBoardManager chessBoard;
    public ChessBoardManager ChessBoard{
        get {return chessBoard;}
    }
    public bool IsClicked
    {
        get { return isClicked; }
        set{isClicked = value;}
    }

    Piece currentlySelectedPiece;
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
        isClicked = false;
        chessBoard = FindObjectOfType<ChessBoardManager>();
        currentlySelectedPiece = chessBoard.CurrentlySelectedPiece;
    }

    void Start(){

        //player 1 will be white
        players.Add(new Player("White"));

        //player 2 will be black
        players.Add(new Player("Black"));
    }

    void StartPlayerTurn(){
        Debug.Log("start turn");
    }

    public void EndPlayerTurn(){
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        StartPlayerTurn();
    }


    public void OnChessPieceClicked(Piece clickedPiece)
    {
        currentlySelectedPiece = chessBoard.CurrentlySelectedPiece;
        if (ReferenceEquals(clickedPiece, currentlySelectedPiece))
        {
            chessBoard.DeSelectPiece(clickedPiece);
        }
        else
        {
            if (currentlySelectedPiece != null)
            {
                chessBoard.DeSelectPiece(currentlySelectedPiece);
            }
            chessBoard.SelectPiece(clickedPiece);
        }
    }
    
    public void HandleMove(Vector2Int position)
    {
        chessBoard.CompleteTurn(position);
    }

}
