using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combination
{
    List<int> items;

    public Combination(int n)
    {
        items = new List<int>(n);
        for (int i = 0; i < n; i++)
            items.Add(i);
    }
    public int GetRandom()
    {
        int idx = Random.Range(0, items.Count);
        int n = items[idx];
        items.RemoveAt(idx);
        return n;
    }
    public int GetNum(int n)
    {
        return items.Remove(n) ? n : -1;
    }
}

public class CombinationRect : Combination
{
    Vector2Int size;
    Vector2Int offset;

    public CombinationRect(RectInt rect) : base(rect.width * rect.height)
    {
        size = new Vector2Int(rect.width, rect.height);
        offset = new Vector2Int(rect.x, rect.y);
    }
    
    public new Vector2Int GetRandom()
    {
        int n = base.GetRandom();
        return new Vector2Int(n % size.x, n / size.x) + offset;
    }
    public Vector2Int GetPoint(int x, int y)
    {
        int n = base.GetNum(y * size.x + x);
        return (n != -1) ? new Vector2Int(x, y) + offset : Vector2Int.zero;
    }
    public Vector2Int GetCenter()
    {
        Vector2Int point = new Vector2Int(size.x / 2, size.y / 2);
        int n = base.GetNum(point.y * size.x + point.x);

        return (n != -1) ? point + offset : Vector2Int.zero;
    }
}
