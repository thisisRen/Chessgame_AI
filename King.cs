using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Chessman
{
    public King()
    {
        value = 900;
    }
    public override bool[,] PossibleMoves()
    {
        bool[,] moves = new bool[8, 8];
        int x = currentX;
        int y = currentY;
        KingMove(x, y - 1, ref moves);

        // Left
        KingMove(x - 1, y, ref moves);

        // Right
        KingMove(x + 1, y, ref moves);

        // Up
        KingMove(x, y + 1, ref moves);

        // DownLeft
        KingMove(x - 1, y - 1, ref moves);

        // DownRight
        KingMove(x + 1, y - 1, ref moves);

        // UpLeft
        KingMove(x - 1, y + 1, ref moves);

        // UpRight
        KingMove(x + 1, y + 1, ref moves);
        if (!isMoved)
        {
            if (isWhite)
            {
                CheckCastlingMoves(BoardManager.Instance.WhiteRook1, BoardManager.Instance.WhiteRook2, ref moves);
            }
            else
            {
                CheckCastlingMoves(BoardManager.Instance.BlackRook1, BoardManager.Instance.BlackRook2, ref moves);
            }
        }
        return moves;
    }

    private void KingMove(int x, int y, ref bool[,] moves)
    {
        if (x >= 0 && y >= 0 && x <= 7 && y <= 7)
        {
            Chessman piece = BoardManager.Instance.Chessmans[x, y];
            if (piece == null)
            {
                if (!KingInDanger(x, y))
                    moves[x, y] = true;
            }
            else if (piece.isWhite != isWhite)
            {
                if (!KingInDanger(x, y))
                    moves[x, y] = true;
            }
        }
    }
    private void CheckCastlingMoves(Chessman Rook1, Chessman Rook2, ref bool[,] moves)
    {
        int x = currentX;
        int y = currentY;
        Chessman[,] Chessmans = BoardManager.Instance.Chessmans;
        bool conditions;
        bool isInCheck = InDanger();

        if (Rook1 != null)
        {
            conditions = (!Rook1.isMoved) && (moves[x + 1, y] && Chessmans[x + 2, y] == null);

            conditions = conditions && !isInCheck;

            SetCastlingMove(x, y, x + 2, ref moves, conditions);

        }

        if (Rook2 != null)
        {

            conditions = (!Rook2.isMoved) && (moves[x - 1, y] && Chessmans[x - 2, y] == null && Chessmans[x - 3, y] == null);

            conditions = conditions && !isInCheck;

            SetCastlingMove(x, y, x - 2, ref moves, conditions);
        }

    }
    private void SetCastlingMove(int x, int y, int newX, ref bool[,] moves, bool conditions)
    {
        if (conditions)
        {
            BoardManager.Instance.Chessmans[x, y] = null;
            BoardManager.Instance.Chessmans[newX, y] = this;
            this.SetPosition(newX, y);

            bool inDanger = false;

            inDanger = InDanger();

            BoardManager.Instance.Chessmans[x, y] = this;
            BoardManager.Instance.Chessmans[newX, y] = null;
            this.SetPosition(x, y);

            if (inDanger == false)
                moves[newX, y] = true;
        }
    }
}
