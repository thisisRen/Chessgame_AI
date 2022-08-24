using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBoard : MonoBehaviour
{
    public int x;
    public int y;
    public PieceBoardColor color;
    public void CallStart(int _x, int _y, PieceBoardColor _color)
    {
        x = _x;
        y = _y;
        color = _color;
    }
}
public enum PieceBoardColor
{
    Green,
    Check,
    Purple,
    Red,
    Yellow
}