using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminoes : MonoBehaviour
{
    public List<Tile> tiles = new List<Tile>();
    public List<Vector2Int> offset;
    private Vector2Int _pos;
    public Vector2Int pos
    {
        get => _pos;
        set
        {
            _pos = value;
        }
    }
    
    void SetPos()
    {
        for(var i = 0; i < tiles.Count; i++)
        {
            tiles[i].X = _pos.x + offset[i].x;
            tiles[i].Y = _pos.y + offset[i].y;
        }
    }
    void rotate()
    {
        for(int i = 0; i < offset.Count; i++)
        {
            int tmpX = offset[i].x;
            int tmpY = offset[i].y;
            offset[i].Set(-tmpY, tmpX);
        }
    }
}
