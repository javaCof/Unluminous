using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSizeFitter : MonoBehaviour
{
    public int viewHeight = 1080;

    private GridLayoutGroup grid;

    private void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
    }

    private void Start()
    {
        float heightRate = (float)Screen.height / viewHeight;

        grid.padding.left = (int)(grid.padding.left * heightRate);
        grid.padding.right = (int)(grid.padding.right * heightRate);
        grid.padding.top = (int)(grid.padding.top * heightRate);
        grid.padding.bottom = (int)(grid.padding.bottom * heightRate);

        grid.cellSize *= heightRate;
        grid.spacing *= heightRate;
    }
}
