using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { set; get; }

    [SerializeField] private GameObject piece;
    [SerializeField] private GameObject board;
    public Sprite lightColor;
    public Sprite darkColor;

    public List<GameObject> ChessmanPrefabs;

    private List<GameObject> ActiveChessmans;

    
    public Chessman[,] Chessmans { set; get; }

    // Currently Selected Chessman
    public Chessman SelectedChessman;
    // Kings
    public Chessman WhiteKing;
    public Chessman BlackKing;
    public Chessman WhiteRook1;
    public Chessman WhiteRook2;
    public Chessman BlackRook1;
    public Chessman BlackRook2;

    public bool[,] allowedMoves;
    // EnPassant move
    public int[] EnPassant { set; get; }  

    // The selected tile
    private int selectionX = -1;
    private int selectionY = -1;

    // Variable to store turn
    public bool isWhiteTurn = true;

    private void Start()
    {
        Instance = this;
        ActiveChessmans = new List<GameObject>();
        Chessmans = new Chessman[8, 8];
        EnPassant = new int[2] { -1, -1 };

        CreatBoard();

        // Spawning all chessmans on the board
        SpawnAllChessmans();
    }

    private PieceBoard[,] pieceBoard;
    private void CreatBoard()
    {
        pieceBoard = new PieceBoard[8, 8];

        for (int file = 0; file < 8; file++)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                bool lightSquare = (file + rank) % 2 != 0;
                var squareColor = (lightSquare) ? lightColor : darkColor;
                var pos = new Vector2(-4.5f + file*1.3f, -4.5f + rank* 1.3f);

                piece.GetComponent<SpriteRenderer>().sprite = squareColor;
                piece.name = "(" + file + "," + rank + ")";

                DrawBoard(file, rank, pos);
            }
        }
    }
    private void DrawBoard(int x, int y, Vector2 pos)
    {
        GameObject g = Instantiate(piece, pos, Quaternion.identity);
        g.name = piece.name;
        g.transform.SetParent(board.transform);
        g.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);

        pieceBoard[x, y] = g.GetComponent<PieceBoard>();
        g.GetComponent<PieceBoard>().x = x;
        g.GetComponent<PieceBoard>().y = y;
    }

    private void SpawnChessman(int index, Vector3 position)
    {
        Vector3 _pos = new Vector3(-4.5f + position.x * 1.3f, -4.5f + position.y * 1.3f, -0.01f);
        GameObject ChessmanObject = Instantiate(ChessmanPrefabs[index], _pos, Quaternion.identity) as GameObject;
        ChessmanObject.transform.SetParent(this.transform);
        ActiveChessmans.Add(ChessmanObject);

        int x = (int)(position.x);
        int y = (int)(position.y);
        Chessmans[x, y] = ChessmanObject.GetComponent<Chessman>();
        Chessmans[x, y].SetPosition(x, y);
    }
    private void SpawnAllChessmans()
    {
        // Spawn White Pieces
        // Rook1
        SpawnChessman(0, new Vector3(0, 0));
        // Knight1
        SpawnChessman(1, new Vector3(1, 0));
        // Bishop1
        SpawnChessman(2, new Vector3(2, 0));
        // King
        SpawnChessman(3, new Vector3(4, 0));
        // Queen
        SpawnChessman(4, new Vector3(3, 0));
        // Bishop2
        SpawnChessman(2, new Vector3(5, 0));
        // Knight2
        SpawnChessman(1, new Vector3(6, 0));
        // Rook2
        SpawnChessman(0, new Vector3(7, 0));
        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(5, new Vector3(i, 1));
        }

        // Spawn Black Pieces
        // Rook1
        SpawnChessman(6, new Vector3(0, 7));
        // Knight1
        SpawnChessman(7, new Vector3(1, 7));
        // Bishop1
        SpawnChessman(8, new Vector3(2, 7));
        // King
        SpawnChessman(9, new Vector3(4, 7));
        // Queen
        SpawnChessman(10, new Vector3(3, 7));
        // Bishop2
        SpawnChessman(8, new Vector3(5, 7));
        // Knight2
        SpawnChessman(7, new Vector3(6, 7));
        // Rook2
        SpawnChessman(6, new Vector3(7, 7));
        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(11, new Vector3(i, 6));
        }

        WhiteKing = Chessmans[4, 0];
        BlackKing = Chessmans[4, 7];
        WhiteRook1 = Chessmans[7, 0];
        WhiteRook2 = Chessmans[0, 0];
        BlackRook1 = Chessmans[7, 7];
        BlackRook2 = Chessmans[0, 7];
    }

    public void SelectChessman(int _x, int _y)
    {
        // if no chessman is on the clicked tile
        if (Chessmans[_x, _y] == null)
        {
            return;
        }

        // if it is not the turn of the selected chessman's team
        if (Chessmans[_x, _y].isWhite != isWhiteTurn)
        {
            return;
        }

        // Selecting chessman with yellow highlight
        SelectedChessman = Chessmans[_x, _y];
        BoardHightLight.Instance.SetTileYellow(_x, _y);

        // Allowed moves highlighted in blue and enemy in Red
        allowedMoves = SelectedChessman.PossibleMoves();
        BoardHightLight.Instance.HighlightPossibleMoves(allowedMoves, isWhiteTurn);
    }

    public void MoveChessman(int x, int y)
    {
        if (allowedMoves[x, y])
        {
            Chessman opponent = Chessmans[x, y];

            if (opponent != null)
            {
                ActiveChessmans.Remove(opponent.gameObject);
                Destroy(opponent.gameObject);
              
            }
            if (EnPassant[0] == x && EnPassant[1] == y && SelectedChessman.GetType() == typeof(Pawn))
            {
                if (isWhiteTurn)
                    opponent = Chessmans[x, y - 1];
                else
                    opponent = Chessmans[x, y + 1];

                ActiveChessmans.Remove(opponent.gameObject);
                Destroy(opponent.gameObject);

            }

            // Reset the EnPassant move
            EnPassant[0] = EnPassant[1] = -1;

            // Set EnPassant available for opponent
            if (SelectedChessman.GetType() == typeof(Pawn))
            {
                //-------Promotion Move Manager------------
                if (y == 7)
                {
                    ActiveChessmans.Remove(SelectedChessman.gameObject);
                    Destroy(SelectedChessman.gameObject);
                    SpawnChessman(4, new Vector3(x, y));
                    SelectedChessman = Chessmans[x, y];
                }
                if (y == 0)
                {
                    ActiveChessmans.Remove(SelectedChessman.gameObject);
                    Destroy(SelectedChessman.gameObject);
                    SpawnChessman(10, new Vector3(x, y));
                    SelectedChessman = Chessmans[x, y];
                }
                //-------Promotion Move Manager Over-------

                if (SelectedChessman.currentY == 1 && y == 3)
                {
                    EnPassant[0] = x;
                    EnPassant[1] = y - 1;
                }
                if (SelectedChessman.currentY == 6 && y == 4)
                {
                    EnPassant[0] = x;
                    EnPassant[1] = y + 1;
                }
            }
            // -------EnPassant Move Manager Over-------

            // -------Castling Move Manager------------
            // If the selectef chessman is King and is trying Castling move which needs two steps
            if (SelectedChessman.GetType() == typeof(King) && System.Math.Abs(x - SelectedChessman.currentX) == 2)
            {
                // King Side (towards (0, 0))
                if (x - SelectedChessman.currentX >= 0)
                {
                    // Moving Rook1
                    Chessmans[x - 1, y] = Chessmans[x + 1, y];
                    Chessmans[x + 1, y] = null;
                    Chessmans[x - 1, y].SetPosition(x - 1, y);
                    Chessmans[x - 1, y].transform.position = new Vector3(-4.5f + (x - 1) * 1.3f, -4.5f + y * 1.3f, -0.01f);
                    Chessmans[x - 1, y].isMoved = true;
                }
                // Queen side (away from (0, 0))
                else
                {
                    // Moving Rook2
                    Chessmans[x + 1, y] = Chessmans[x - 2, y];
                    Chessmans[x - 2, y] = null;
                    Chessmans[x + 1, y].SetPosition(x + 1, y);
                    Chessmans[x + 1, y].transform.position = new Vector3(-4.5f + (x + 1) * 1.3f, -4.5f + y * 1.3f, -0.01f);
                    Chessmans[x + 1, y].isMoved = true;
                }
                // Note : King will move as a SelectedChessman by this function later
            }
            // -------Castling Move Manager Over-------
            SoundManager.Instance.PlayEffect("clickW");

            Chessmans[SelectedChessman.currentX, SelectedChessman.currentY].transform.DOMove(new Vector3(-4.5f + x * 1.3f, -4.5f + y * 1.3f, -0.01f), 0.5f).OnComplete(() =>
            {
                Chessmans[SelectedChessman.currentX, SelectedChessman.currentY].transform.DOKill();

            });
            Chessmans[SelectedChessman.currentX, SelectedChessman.currentY] = null;
            Chessmans[x, y] = SelectedChessman;
            SelectedChessman.SetPosition(x, y);
            SelectedChessman.transform.localPosition = new Vector3(-4.5f + x * 1.3f, -4.5f + y * 1.3f, -0.01f);
            SelectedChessman.isMoved = true;
            isWhiteTurn = !isWhiteTurn;


            // to be deleted
            // printBoard();
        }

        // De-select the selected chessman
        SelectedChessman = null;
        // Disabling all highlights
        BoardHightLight.Instance.DisableAllHighlights();

        // ------- King Check Alert Manager -----------
        // Is it Check to the King
        // If now White King is in Check
        if (isWhiteTurn)
        {
            if (WhiteKing.InDanger())
                BoardHightLight.Instance.SetTileCheck(WhiteKing.currentX, WhiteKing.currentY);
        }
        // If now Black King is in Check
        else
        {
            if (BlackKing.InDanger())
                BoardHightLight.Instance.SetTileCheck(BlackKing.currentX, BlackKing.currentY);
        }
        // ------- King Check Alert Manager Over ----


        // Check if it is Checkmate
        isCheckmate();
    }
    private void isCheckmate()
    {
        bool hasAllowedMove = false;
        foreach (GameObject chessman in ActiveChessmans)
        {
            if (chessman.GetComponent<Chessman>().isWhite != isWhiteTurn)
                continue;

            bool[,] allowedMoves = chessman.GetComponent<Chessman>().PossibleMoves();

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (allowedMoves[x, y])
                    {
                        hasAllowedMove = true;
                        break;
                    }
                }
                if (hasAllowedMove) break;
            }
        }

        if (!hasAllowedMove)
        {
            BoardHightLight.Instance.HighlightCheckmate(isWhiteTurn);
            GameOver.Instance.GameOverMenu();
        }
    }
    public void EndGame()
    {
        if (!isWhiteTurn)
            Debug.Log("White team wins");
        else
            Debug.Log("Black team wins");

        foreach (GameObject go in ActiveChessmans)
            Destroy(go);

        // New Game
        isWhiteTurn = true;
        BoardHightLight.Instance.DisableAllHighlights();
        SpawnAllChessmans();
    }

    public void MoveChessMan_DoTween(Chessman selectedChessman, Vector3 _parent)
    {
        int x = selectedChessman.currentX;
        int y = selectedChessman.currentY;
        Chessmans[x, y].transform.DOMove(_parent, 1f).OnComplete(() =>
        {
            Chessmans[x, y].transform.DOKill();
        });
    }
}
