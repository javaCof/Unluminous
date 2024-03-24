using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapGenScript))]
public class MapTexturePainter : MonoBehaviour
{
    public Texture2D mapTexture;
    public Transform mapPos;

    public Color emptyColor = Color.black;
    public Color floorColor = Color.white;
    public Color wallColor = Color.red;
    public Color cornerColor = Color.green;
    public Color pillarColor = Color.blue;

    private MapGenScript mapGen;

    private void Awake()
    {
        mapGen = GetComponent<MapGenScript>();
    }

    public void MapPaint(List<RectInt> rooms, List<PathInfo> paths)
    {
        int map_w = mapTexture.width;
        int map_h = mapTexture.height;

        //back
        for (int i = 0; i < map_h; i++)
            for (int j = 0; j < map_w; j++)
                mapTexture.SetPixel(j, i, emptyColor);

        //room
        foreach (RectInt room in rooms)
        {
            int x1 = room.x;
            int y1 = room.y;
            int x2 = room.x + room.width;
            int y2 = room.y + room.height;

            //floor
            for (int i = y1; i < y2; i++)
                for (int j = x1; j < x2; j++)
                    mapTexture.SetPixel(j, i, floorColor);

            //wall
            for (int j = x1; j < x2; j++)
            {
                mapTexture.SetPixel(j, y1, wallColor);
                mapTexture.SetPixel(j, y2 - 1, wallColor);
            }
            for (int i = y1; i < y2; i++)
            {
                mapTexture.SetPixel(x1, i, wallColor);
                mapTexture.SetPixel(x2 - 1, i, wallColor);
            }

            //corner
            mapTexture.SetPixel(x1, y1, cornerColor);
            mapTexture.SetPixel(x1, y2 - 1, cornerColor);
            mapTexture.SetPixel(x2 - 1, y1, cornerColor);
            mapTexture.SetPixel(x2 - 1, y2 - 1, cornerColor);
        }

        //path
        foreach (PathInfo path in paths)
        {
            if (path.line.xAxis)
            {
                DrawMapLine(path.begin, new Vector2Int(path.begin.x, path.line.xy));
                DrawMapLine(path.end, new Vector2Int(path.end.x, path.line.xy));
                DrawMapLine(new Vector2Int(path.begin.x, path.line.xy), new Vector2Int(path.end.x, path.line.xy));
            }
            else
            {
                DrawMapLine(path.begin, new Vector2Int(path.line.xy, path.begin.y));
                DrawMapLine(path.end, new Vector2Int(path.line.xy, path.end.y));
                DrawMapLine(new Vector2Int(path.line.xy, path.begin.y), new Vector2Int(path.line.xy, path.end.y));
            }
        }

        mapTexture.Apply();
        SendMapPaint();
    }

    void DrawMapLine(Vector2Int begin, Vector2Int end)
    {
        if (begin.x == end.x)
        {
            int fromY = Mathf.Min(begin.y, end.y), toY = Mathf.Max(begin.y, end.y);
            for (int y = fromY; y <= toY; y++)
                SetPathPixel(begin.x, y);
        }
        else if (begin.y == end.y)
        {
            int fromX = Mathf.Min(begin.x, end.x), toX = Mathf.Max(begin.x, end.x);
            for (int x = fromX; x <= toX; x++)
                SetPathPixel(x, begin.y);
        }
        else return;
    }

    void SetPathPixel(int x, int y)
    {
        if (mapTexture.GetPixel(x, y) == wallColor) mapTexture.SetPixel(x, y, pillarColor);
        else if (mapTexture.GetPixel(x, y) == emptyColor) mapTexture.SetPixel(x, y, wallColor);
    }

    void SendMapPaint()
    {
        mapGen.Map = mapTexture;
        mapGen.trans = mapPos;
        mapGen.GenerateMap();
    }
}
