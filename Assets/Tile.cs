using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private int _x;

    [SerializeField] private int _y;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private ParticleSystem _hitEffect;

    [SerializeField] private AudioSource _hitSource;

    [SerializeField] private int _tileType;
    [SerializeField] private List<Vector2Int> _offset;
    public int X
    {
        get => _x;

        set
        {
            _x = value;

            var pos = transform.localPosition;
            pos.x = _x * _spriteRenderer.bounds.size.x;
            transform.localPosition = pos;
        }
    }
    public int Y
    {
        get => _y;

        set
        {
            _y = value;
            var pos = transform.localPosition;
            pos.y = _y * _spriteRenderer.bounds.size.y;
            transform.localPosition = pos;
        }
    }
    public int tileType
    {
        get => _tileType;
        set
        {
            _tileType = 0;
        }


    }
    public List<Vector2Int> offset
    {
        get => _offset;
        set
        {
            _offset = value;
        }
    }
    public void Hit()
    {
        _hitSource.Play();
        _hitEffect.Play();
        StartCoroutine(Shake());
    }
    public void ChangeTileColor(Color m_color,int m_idx)
    {
        _spriteRenderer.color = m_color;
        _tileType = m_idx;
    }
    public bool check(int x,int y)
    {
        Debug.Log(x);
        Debug.Log(y);
        foreach(Vector2Int child in offset)
        {
            if(_x+child.x == x && _y + child.y == y)
            {
                return true;
            }
        }
        return false;
    }
    public bool checkBound(int dx,int dy,int x)
    {
        foreach (Vector2Int child in offset)
        {
            if (_x + child.x + dx< 0 || _y + child.y + dy < 0 || _x +child.x +dx >= x)
            {
                return false;
            }
        }
        return true;
    }
    private IEnumerator Shake()
    {
        var beginTime = Time.realtimeSinceStartup;
        var beginY = _spriteRenderer.transform.localPosition.y;
        var endTime = beginTime + 0.3f;

        var spriteTransform = _spriteRenderer.transform;

        while (Time.realtimeSinceStartup < endTime)
        {
            var diffTime = endTime - Time.realtimeSinceStartup;
            var range = 0.05f * diffTime;
            var pos = spriteTransform.localPosition;
            pos.y = beginY + Random.Range(-range, range);
            spriteTransform.localPosition = pos;
            yield return null;
        }

        {
            var pos = spriteTransform.localPosition;
            pos.y = beginY;
            spriteTransform.localPosition = pos;
        }

    }
}
