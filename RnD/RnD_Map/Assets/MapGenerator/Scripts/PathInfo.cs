using UnityEngine;

public struct AxisLine
{
    public int xy;
    public bool xAxis;

    public AxisLine(int xy, bool xAxis)
    {
        this.xy = xy;
        this.xAxis = xAxis;
    }
}

public struct PathInfo
{
    public Vector2Int begin;
    public Vector2Int end;
    public AxisLine line;

    public PathInfo(Vector2Int begin, Vector2Int end, AxisLine line)
    {
        this.begin = begin;
        this.end = end;
        this.line = line;
    }
}