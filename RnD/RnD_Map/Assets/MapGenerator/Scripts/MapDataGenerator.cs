using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapTexturePainter))]
public class MapDataGenerator : MonoBehaviour
{
    class RoomNode
    {
        public RectInt rect;
        public RectInt room;
        public RoomNode left;
        public RoomNode right;
        public AxisLine line;

        public RoomNode(RectInt rect) => this.rect = rect;
        public Vector2Int Center { get { return new Vector2Int(room.x + room.width / 2, room.y + room.height / 2); } }
    }

    public Vector2Int mapSize;
    public int maxDivideDepth = 5;
    public float minDividePer = 0.3f;
    public float maxDividePer = 0.7f;

    MapTexturePainter painter;
    List<RectInt> rooms;
    List<PathInfo> paths;

    [Header("DEBUG")]
    public bool useDebug = false;
    public LineRenderer lineRnd;
    public LineRenderer rectRnd;

    private void Awake()
    {
        painter = GetComponent<MapTexturePainter>();
    }

    private void Start()
    {
        rooms = new List<RectInt>();
        paths = new List<PathInfo>();
        GenerateMapData();
    }

    public void GenerateMapData()
    {
        RoomNode map = new RoomNode(new RectInt(0, 0, mapSize.x, mapSize.y));
        
        if (useDebug) DrawRect(map.rect, Color.red);

        DivideRect(map);
        CreateRoom(map);
        CreatePath(map);
        SendMapData();
    }
    void DivideRect(RoomNode node, bool hRev = false, bool vRev = false, int n = 0)
    {
        if (n == maxDivideDepth) return;

        RoomNode lRoom, rRoom;
        bool bH = false, bV = false;
        float dividePer = Random.Range(minDividePer, maxDividePer);

        if (node.rect.width >= node.rect.height)
        {   //divide vertical
            int left_w = (int)(node.rect.width * dividePer);
            lRoom = new RoomNode(new RectInt(
                node.rect.x, node.rect.y,
                left_w, node.rect.height
                ));
            rRoom = new RoomNode(new RectInt(
                node.rect.x + left_w, node.rect.y,
                node.rect.width - left_w, node.rect.height
                ));
            node.line = new AxisLine(node.rect.x + left_w, false);

            if (vRev)
            {
                node.left = rRoom;
                node.right = lRoom;
            }
            else
            {
                node.left = lRoom;
                node.right = rRoom;
            }
            bV = true;

            if (useDebug)
            {
                DrawLine(
                new Vector2Int(node.rect.x + left_w, node.rect.y),
                new Vector2Int(node.rect.x + left_w, node.rect.y + node.rect.height),
                Color.red);
            }
        }
        else
        {   //divide horizontal
            int down_h = (int)(node.rect.height * dividePer);
            lRoom = new RoomNode(new RectInt(
                node.rect.x, node.rect.y,
                node.rect.width, down_h
                ));
            rRoom = new RoomNode(new RectInt(
                node.rect.x, node.rect.y + down_h,
                node.rect.width, node.rect.height - down_h
                ));
            node.line = new AxisLine(node.rect.y + down_h, true);

            if (hRev)
            {
                node.left = rRoom;
                node.right = lRoom;
            }
            else
            {
                node.left = lRoom;
                node.right = rRoom;
            }
            bH = true;

            if (useDebug)
            {
                DrawLine(
                new Vector2Int(node.rect.x, node.rect.y + down_h),
                new Vector2Int(node.rect.x + node.rect.width, node.rect.y + down_h),
                Color.red);
            }
        }

        DivideRect(node.left, bH, bV, n + 1);
        DivideRect(node.right, hRev, vRev, n + 1);
    }
    RectInt CreateRoom(RoomNode node, int n = 0)
    {
        RectInt room;

        if (n == maxDivideDepth)
        {
            RectInt rect = node.rect;

            int w = rect.width - 3;//Random.Range(rect.width / 2, rect.width - 2);
            int h = rect.height - 3;// Random.Range(rect.height / 2, rect.height - 2);
            int x = rect.x + rect.width - w-1;
            int y = rect.y + rect.height - h-1;

            room = new RectInt(x, y, w, h);
            rooms.Add(room);
            if (useDebug) DrawRect(room, Color.white);
        }
        else
        {
            node.left.room = CreateRoom(node.left, n + 1);
            node.right.room = CreateRoom(node.right, n + 1);
            room = node.left.room;
        }

        return room;
    }
    void CreatePath(RoomNode node, int n = 0)
    {
        if (n == maxDivideDepth) return;

        paths.Add(new PathInfo(node.left.Center, node.right.Center, node.line));

        if (useDebug)
        {
            if (node.line.xAxis)
            {
                DrawLine(node.left.Center, new Vector2Int(node.left.Center.x, node.line.xy), Color.blue);
                DrawLine(node.right.Center, new Vector2Int(node.right.Center.x, node.line.xy), Color.blue);
                DrawLine(new Vector2Int(node.left.Center.x, node.line.xy), new Vector2Int(node.right.Center.x, node.line.xy), Color.blue);
            }
            else
            {
                DrawLine(node.left.Center, new Vector2Int(node.line.xy, node.left.Center.y), Color.blue);
                DrawLine(node.right.Center, new Vector2Int(node.line.xy, node.right.Center.y), Color.blue);
                DrawLine(new Vector2Int(node.line.xy, node.left.Center.y), new Vector2Int(node.line.xy, node.right.Center.y), Color.blue);
            }
        }

        CreatePath(node.left, n + 1);
        CreatePath(node.right, n + 1);
    }

    void SendMapData()
    {
        painter.MapPaint(rooms, paths);
    }

    /*------------DEBUG------------*/
    void DrawLine(Vector2Int from, Vector2Int to, Color col)
    {
        LineRenderer rnd = Instantiate(lineRnd, transform);
        rnd.material.color = col;

        rnd.SetPosition(0, new Vector2(from.x, from.y));
        rnd.SetPosition(1, new Vector2(to.x, to.y));
    }
    void DrawRect(RectInt rect, Color col)
    {
        LineRenderer rnd = Instantiate(rectRnd, transform);
        rnd.material.color = col;

        rnd.SetPosition(0, new Vector2(rect.x, rect.y));
        rnd.SetPosition(1, new Vector2(rect.x + rect.width, rect.y));
        rnd.SetPosition(2, new Vector2(rect.x + rect.width, rect.y + rect.height));
        rnd.SetPosition(3, new Vector2(rect.x, rect.y + rect.height));
    }
}
