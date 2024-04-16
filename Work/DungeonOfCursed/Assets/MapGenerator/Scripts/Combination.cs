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
}

public class CombinationRect : Combination
{
    int rect_w;
    Vector2Int offset;

    public CombinationRect(RectInt rect) : base(rect.width * rect.height)
    {
        rect_w = rect.width;
        offset = new Vector2Int(rect.x, rect.y);
    }
    
    public new Vector2Int GetRandom()
    {
        int n = base.GetRandom();
        return new Vector2Int(n % rect_w, n / rect_w) + offset;
    }
}
