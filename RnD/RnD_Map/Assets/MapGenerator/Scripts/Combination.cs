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
