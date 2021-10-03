using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _boardRoot;

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private Text _elapsedTimeFromStartText;
    [SerializeField] private AudioSource _removeRow;
    [SerializeField] private Text _ScoreText;
    private const int Width = 6;
    private const int Height = 10;
    private int _score = 0;
    private int Score
    {
        get => _score;
        set
        {
            _score = value;
            _ScoreText.text = _score.ToString();
        }
    }
    private const int MiddleX = Width / 2;
    private const int Top = Height - 1;

    private const float MaxElapsedTimePerStep = 0.3f;
    private const float MaxElapsedTimeFromStart = 60f;

    private float _elapsedTimeFromStart;
    private List<Color> tileColor = new List<Color> { Color.white, Color.red, Color.green, Color.blue,Color.yellow,Color.cyan};
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
        yield return MainMenu();
        while (true)
        {
            yield return PlayGame();
            yield return GameOver();
        }
    }
    private IEnumerator GameOver()
    {
        if (Input.anyKey)
        {
            yield return null;
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
        Score = 0;
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
            if (MovementHorizon < 0 && elapsedTime > nextMove && PauseMenu.GamePaused == false)
            {
                nextMove = elapsedTime + moveDelta;
                MoveTileLeft(tiles, tile);
            }

            if (MovementHorizon > 0 && elapsedTime > nextMove && PauseMenu.GamePaused == false)
            {
                nextMove = elapsedTime + moveDelta;
                MoveTileRight(tiles, tile);
            }

            if (MovementVertical < 0 && elapsedTime > nextMove && PauseMenu.GamePaused == false)
            {
                if (!MoveTileDown(tiles, tile))
                {
                    checkRow(tiles, tile);
                    nextMove = elapsedTime + moveDelta;
                    tile = CreateNewTile(tiles);

                    if (tile == null) break;
                }
            }
            
            if (elapsedTime >= MaxElapsedTimePerStep)
            {
                if (!MoveTileDown(tiles, tile))
                {
                    checkRow(tiles, tile);
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
        var tile = Instantiate(_tilePrefab, _boardRoot);
        var m_color = Random.Range(0, tileColor.Count);
        tile.tileType = m_color;
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
    private bool checkRow(List<Tile>tiles,Tile cur)
    {
        var count = 0;
        var rainbow = 0;
        var rewardScore = 500;
        bool[] checkList = new bool[tileColor.Count];
        checkList[cur.tileType] = true;
        List<int> idx = new List<int>();
        foreach(Tile tile in tiles)
        {
            if(tile.Y == cur.Y)
            {
                if(tile.tileType == cur.tileType)
                    count++;
                else
                {
                    checkList[tile.tileType] = true;
                }
            }
        }
        for(int i=0;i< checkList.Length; i++)
        {
            if (checkList[i])
            {
                rainbow++;
            }
        }
        if(rainbow == Width)
        {
            rewardScore = 300;
            count = Width;
        }
        if(count != Width)
        {
            return false;
        }
        for(int i = 0;i <tiles.Count;i++)
        {
            if(tiles[i].Y == cur.Y)
            {
                Destroy(tiles[i]?.gameObject);
                idx.Add(i);
            }
        }
        Destroy(cur?.gameObject);
        _removeRow.Play();
        foreach(Tile tile in tiles)
        {
            if(tile.Y > cur.Y)
                tile.Y--;
        }
        idx.Reverse();
        foreach(int i in idx)
        {
            tiles.RemoveAt(i);
        }
        Score += rewardScore;
        return true;
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
                var tile = Instantiate(_tilePrefab, _boardRoot);
                tile.X = x;
                tile.Y = y;
            }
        }
    }
}
