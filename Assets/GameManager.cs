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
        if (tiles.Any(other => other.X == MiddleX && other.Y == Top))
            return null;
        var tile = Instantiate(_tilePrefab[Random.Range(0,_tilePrefab.Length)], _boardRoot);
        var m_color = Random.Range(0, tileColor.Count);
        tile.ChangeTileColor(tileColor[m_color],m_color);
        tile.X = MiddleX;
        tile.Y = Top;
        return tile;
    }
    private static bool CanTileMoveTo(IEnumerable<Tile> tiles, int x, int y, int width)
    {
        if (x < 0 || x >= width || y < 0)
            return false;

        return !tiles.Any(other => other.X == x && other.Y == y);
    }

    private static void MoveTileLeft(IEnumerable<Tile> tiles, Tile tile)
    {
        if (!CanTileMoveTo(tiles, tile.X - 1, tile.Y, Width)) return;
        tile.X--;
    }
    private static void MoveTileRight(IEnumerable<Tile> tiles, Tile tile)
    {
        if (!CanTileMoveTo(tiles, tile.X + 1, tile.Y, Width)) return;
        tile.X++;
    }

    private static bool MoveTileDown(ICollection<Tile> tiles, Tile tile)
    {
        if (CanTileMoveTo(tiles, tile.X, tile.Y - 1, Width))
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
