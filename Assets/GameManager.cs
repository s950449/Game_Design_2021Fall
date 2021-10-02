using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _boardRoot;

    [SerializeField] private Tile[] _tilePrefab;

    [SerializeField] private Text _elapsedTimeFromStartText;
    public Vector3 rotationPoint;
    private const int Width = 6;
    private const int Height = 10;
    private const int MiddleX = Width / 2;
    private const int Top = Height - 1;

    private const float MaxElapsedTimePerStep = 0.3f;
    private const float MaxElapsedTimeFromStart = 60f;

    private float _elapsedTimeFromStart;

    private List<Color> tileColor = new List<Color> { Color.white, Color.red, Color.green, Color.blue };
    private float ElapsedTimeFromStart
    {
        get => _elapsedTimeFromStart;

        set
        {
            _elapsedTimeFromStart = value;
            _elapsedTimeFromStartText.text = _elapsedTimeFromStart.ToString("F2");
        }
    }

    private IEnumerator Start()
    {
        while (true)
        {
            yield return MainMenu();
            yield return PlayGame();
        }
    }
    private IEnumerator MainMenu()
    {
        if (Input.anyKey)
        {
            Debug.Log("Input Event\n");
            yield return null;
        }
    }
    private IEnumerator PlayGame()
    {
        var tiles = new List<Tile>();
        var tile = CreateNewTile(tiles);

        ElapsedTimeFromStart = 0f;
        var elapsedTime = 0f;
        var nextMove = 0f;
        var moveDelta = 0.5f;
        while (ElapsedTimeFromStart <= MaxElapsedTimeFromStart)
        {
            ElapsedTimeFromStart += Time.deltaTime;
            elapsedTime += Time.deltaTime;
            var MovementHorizon = Input.GetAxisRaw("Horizontal");
            var MovementVertical = Input.GetAxisRaw("Vertical");
            if (MovementHorizon < 0 && elapsedTime > nextMove)
            {
                nextMove = elapsedTime + moveDelta;
                MoveTileLeft(tiles, tile);
            }

            if (MovementHorizon > 0 && elapsedTime > nextMove)
            {
                nextMove = elapsedTime + moveDelta;
                MoveTileRight(tiles, tile);
            }

            if (MovementVertical < 0 && elapsedTime > nextMove)
            {
                if (!MoveTileDown(tiles, tile))
                {
                    nextMove = elapsedTime + moveDelta;
                    tile = CreateNewTile(tiles);

                    if (tile == null) break;
                }
            }
            if(MovementVertical > 0 && elapsedTime > nextMove)
            {
                nextMove = elapsedTime + moveDelta * 1.7f;
                Rotate(tiles, tile);


            }
            
            if (elapsedTime >= MaxElapsedTimePerStep)
            {
                if (!MoveTileDown(tiles, tile))
                {
                    tile = CreateNewTile(tiles);

                    if (tile == null) break;
                }
                nextMove = nextMove - elapsedTime;
                elapsedTime = 0;
                
            }

            yield return null;
        }
    
        // GameOver
        ElapsedTimeFromStart = Mathf.Min(ElapsedTimeFromStart, MaxElapsedTimeFromStart);

        yield return new WaitForSeconds(1f);

        Destroy(tile?.gameObject);

        foreach (var t in tiles)
        {
            Destroy(t.gameObject);
        }
    }
    private Tile CreateNewTile(IEnumerable<Tile> tiles)
    {
        foreach(Tile child in tiles)
        {
           foreach(Vector2Int offset in child.offset)
            {
                if(child.X + offset.x == MiddleX && child.Y + offset.y == Top)
                {
                    return null;
                }
            }
        }
        //if (tiles.Any(other => other.X == MiddleX && other.Y == Top))
            //return null;
        var tile = Instantiate(_tilePrefab[Random.Range(0,_tilePrefab.Length)], _boardRoot);
        var m_color = Random.Range(0, tileColor.Count);

        SpriteRenderer[] m_sprites = tile.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer s in m_sprites)
        {
            s.color = tileColor[m_color];
        }
        //tile.ChangeTileColor(tileColor[m_color], m_color);
        //tile.ChangeTileColor(tileColor[m_color],m_color);
        tile.X = MiddleX;
        tile.Y = Top;
        return tile;
    }
    private bool CanTileMoveTo(IEnumerable<Tile> tiles, Tile tile, int dx, int dy, int width)
    {
        if (!tile.checkBound(dx, dy, width))
        {
            return false;
        }
        //Debug.Log(tile.X + dx);
        //Debug.Log(tile.Y + dy);
        bool ret = true;
        foreach (Vector2Int child in tile.offset)
        {
            
            ret = ret & (!tiles.Any(other => other.check(tile.X + dx + child.x, tile.Y + dy + child.y,width)));
        }
        return ret;
    }
    private  void MoveTileLeft(IEnumerable<Tile> tiles, Tile tile)
    {
        if (!CanTileMoveTo(tiles, tile, -1,0,Width)) return;
        tile.X--;
    }
    private  void MoveTileRight(IEnumerable<Tile> tiles, Tile tile)
    {
        if (!CanTileMoveTo(tiles, tile,1,0, Width)) return;
        tile.X++;
    }
    private  void Rotate(IEnumerable<Tile>tiles,Tile tile)
    {
        if (tile.tileType == -1)
        {
            return;
        }
        Tile tmp = new Tile();
        tmp = tile;
        for (int i = 0; i < tmp.offset.Count; i++)
        {
            int tmpX = tmp.offset[i].x;
            int tmpY = tmp.offset[i].y;
            Debug.Log("A");
            Debug.Log(tmp.offset[i]);
            tmp.offset[i] = new Vector2Int(-tmpY, tmpX);
            Debug.Log("B");
            Debug.Log(tmp.offset[i]);
        }
        foreach (Transform child in tmp.transform)
        {
            var pos = child.localPosition;
            child.localPosition = new Vector3(-pos.y, pos.x);
        }
        if (!CanTileMoveTo(tiles, tmp, 0, 0, Width))
        {
            return;
        }

        tile = tmp;
    }
    private  bool MoveTileDown(ICollection<Tile> tiles, Tile tile)
    {
        if (CanTileMoveTo(tiles, tile,0,-1, Width))
        {
            tile.Y--;
            return true;
        }

        tile.Hit();
        tiles.Add(tile);
        return false;
    }

    public void Fill()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                var tile = Instantiate(_tilePrefab[Random.Range(0, _tilePrefab.Length)], _boardRoot);
                tile.X = x;
                tile.Y = y;
            }
        }
    }
}
